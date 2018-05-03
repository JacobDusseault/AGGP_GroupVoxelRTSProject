using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Priority_Queue;

public class World : MonoBehaviour
{
	[SerializeField] private GameObject _chunkPrefab;
	private static Dictionary<Int3, DataChunk> _chunks = new Dictionary<Int3, DataChunk>();
	private static Dictionary<Int2, DataColumn> _columns = new Dictionary<Int2, DataColumn>();
	private SimplePriorityQueue<Chunk> _loadQueue = new SimplePriorityQueue<Chunk>();
	private bool _rendering;

	[SerializeField] private static int _chunkSize = 16;
	[SerializeField] private static int _viewRangeHorizontal = 3;
	[SerializeField] private static int _viewRangeVertical = 3;
	private static Int3 _playerPos;

	private static float _offset = 0f;

	void Start()
	{
		_offset = UnityEngine.Random.Range(0f, 1000f);

		_playerPos = new Int3(Camera.main.transform.position / _chunkSize);

		GenerateChunks();

		#if UNITY_EDITOR
		UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
		#endif
	}

	private void RenderThread()
	{
		while (_loadQueue.Count > 0)
		{
			Chunk newChunkScript = _loadQueue.Dequeue();

			if (newChunkScript != null)
			{
				// Errors in threads need to be manually caught and sent to the main thread
				try
				{
					newChunkScript.GenerateBlocks();
				}
				catch(Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		_rendering = false;
	}

	private void GenerateChunks()
	{
		// Which direction is the player pointing in?
		Vector3 pov = Camera.main.transform.rotation * Vector3.forward;
		pov.y = 0; // Flatten it as we want it to be horizontal

		// Iterate through x, y, z
		for (int x = -5; x <= 5; ++x)
		{
			for (int z = -5; z <= 5; ++z)
			{
				Int2 grid = new Int2(x, z);

				DataColumn newDataColumn;

				// Does column exist?
				if (!_columns.ContainsKey(grid))
				{
					// Create new data column
					newDataColumn = new DataColumn(grid);

					// Store in map
					_columns[grid] = newDataColumn;
				}
				else
				{
					newDataColumn = _columns[grid];
				}

				for (int y = 0; y <= 2; ++y)
				{
					Int3 pos = new Int3(x, y, z);
					
                    // Does chunk exist?
					if (!_chunks.ContainsKey(pos))
					{
						// Create new chunk and get corresponding script
						GameObject newChunk = Instantiate(_chunkPrefab, new Vector3(x * _chunkSize, y * _chunkSize, z * _chunkSize), Quaternion.identity);
						Chunk newChunkScript = newChunk.GetComponent<Chunk>();

						DataChunk newDataChunk;

						// Create new data chunk
						newDataChunk = new DataChunk(pos, newChunkScript, newDataColumn);

						// Let chunk know its corresponding data chunk and position
						newChunkScript.LoadData(pos, newDataChunk);

						// Get angle difference between vectors
						Vector3 dir = pos.Vector() * _chunkSize - Camera.main.transform.position;
						float dist = dir.magnitude;
						float diff = Vector3.Angle(pov, dir);
						float final = dist + diff;
						if (dist < _chunkSize * 2f) // Prioritize chunks immediately closest
						{
							final = dist;
						}

						// Queue chunk for generation
						_loadQueue.Enqueue(newChunkScript, final);

						// Store in map
						_chunks[pos] = newDataChunk;
					}
				}
			}
		}

		// Are there chunks that need generation?
		if (!_rendering && _loadQueue.Count > 0)
		{
			_rendering = true;
			new Thread(RenderThread).Start();
		}
	}

	private void DestroyChunk(Int3 pos)
	{
		Destroy(_chunks[pos].GetChunk()); // Delete corresponding gameobject
		//_offloadChunks[pos] = _chunks[pos]; //Move chunk data to offload—technically should be disk or something
		_chunks.Remove(pos); // Remove chunk from main list
	}

	public static float CubeDistance(Int3 one, Int3 two)
	{
		return Mathf.Max(Mathf.Abs(one.x - two.x), Mathf.Abs(one.y - two.y), Mathf.Abs(one.z - two.z));
	}

	public static float Distance(Int3 one, Int3 two)
	{
		return Mathf.Pow(Mathf.Pow(one.x - two.x, 2f) + Mathf.Pow(one.y - two.y, 2f) + Mathf.Pow(one.z - two.z, 2f), 1f/3f);
	}

	// This gets blocks that have already been generated in the past
	public static Atlas.ID GetBlock(Int3 pos, int x, int y, int z)
	{
		return _chunks[pos].GetBlock(x, y, z);
	}

	// This is the main world generation function per block
	public static Atlas.ID GenerateBlock(DataColumn column, int x, int y, int z)
	{
		// Topology
		float stone = column.GetSurface(x, z);
		float dirt = 1;
		
		if (y <= stone)
		{
			/*// Caves
			float caves = PerlinNoise(x, y * 2, z, 40, 12, 1);
			caves += PerlinNoise(x, y, z, 30, 8, 0);
			caves += PerlinNoise(x, y, z, 10, 4, 0);
			
			if (caves > 16)
			{
				return Atlas.ID.Air; // Generating caves
			}

			// Underground ores
			float coal = PerlinNoise(x, y, z, 20, 20, 0);

			if (coal > 18)
			{
				return Atlas.ID.Coal;
			}*/

			return Atlas.ID.Stone; // Stone layer
		}
		else if (y <= stone + dirt)
		{
			return Atlas.ID.Dirt; // Dirt cover
		}
		else if (y <= stone + dirt + 1)
		{
			return Atlas.ID.Grass; // Grass cover
		}
		else
		{
			return Atlas.ID.Air; // Open Air
		}
	}

	public static int GenerateTopology(int x, int z)
	{
		// Topology
		float stone = PerlinNoise(x, 500, z, 100, 10, 1f);
		stone += PerlinNoise(x, 300, z, 50, 4, 1f);
		//stone += PerlinNoise(x, 0, z, 20, 2, 1f);
		
		// "Plateues"
		if (false && PerlinNoise(x, 100, z, 100, 10, 1f) >= 9f)
		{
			stone += 10;
		}

		//stone += Mathf.Clamp(PerlinNoise(x, 0, z, 50, 10, 5f), 0, 10); // Craters?

		return (int) stone;
	}

	public static float PerlinNoise(float x, float y, float z, float scale, float height, float power)
	{
		y += _offset;

		float rValue;
		rValue = Noise.Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
		rValue *= height;

		if (power != 0)
		{
			rValue = Mathf.Pow(rValue, power);
		}

		return rValue;
	}

	public static int GetChunkSize()
	{
		return _chunkSize;
	}

	public static int GetViewRange()
	{
		return _viewRangeHorizontal;
	}

	public static DataChunk GetChunk(Int3 chunkPos)
	{
		DataChunk chunk;
		_chunks.TryGetValue(chunkPos, out chunk);
		return chunk;
	}

	public static DataColumn GetColumn(Int3 chunkPos)
	{
		DataColumn column;
		Int2 chunkPos2 = new Int2(chunkPos.x, chunkPos.z);
		_columns.TryGetValue(chunkPos2, out column);
		return column;
	}

	// This gives the chunk that the world block resides in
	public static Int3 WhichChunk(Int3 blockPos)
	{
		int x = (int) Mathf.Floor(blockPos.x / (float) _chunkSize);
		int y = (int) Mathf.Floor(blockPos.y / (float) _chunkSize);
		int z = (int) Mathf.Floor(blockPos.z / (float) _chunkSize);

		return new Int3(x, y, z);
	}

	// This clamps the world block to a local block
	public static Int3 ClampBlock(Int3 blockPos)
	{
		int x = blockPos.x % _chunkSize;
		int y = blockPos.y % _chunkSize;
		int z = blockPos.z % _chunkSize;

		return new Int3(x, y, z);
	}
}
