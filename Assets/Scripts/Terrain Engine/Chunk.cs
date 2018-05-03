using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	// Mesh generation
	private List<Vector3> _newVerts = new List<Vector3>();
	private List<int> _newTris = new List<int>();
	private List<Vector2> _newUV = new List<Vector2>();
	private List<Color> _newColors = new List<Color>();
	private int _faceCount;

	private Mesh _mesh;
	private MeshCollider _col;
	private bool _updateMesh;
	private bool _clearMesh;

	/*
	 * Fresh - The chunk has been freshly created, but it has no data associated
	 * Prepped - The chunk now has its basic data
	 * Generating - The chunk is actively generating or retrieving block data
	 * Loaded - The chunk has its block data loaded
	 * Rendered - The chunk is actively rendering
	 */
	public enum State { Fresh, Prepped, Generating, Loaded, Rendered };

	private State _state = State.Fresh;

	// Informatics
	private int _chunkSize;
	private Int3 _chunkPos;
	private DataChunk _chunkData;

	public GameObject _townCenter;
	public GameObject _troopCenter;
	public GameObject _archerTower;
	public GameObject _farm;
	public GameObject _goldMine;

	// Debug
	public bool isolateMesh;
	private bool _updateIso;

	public void LoadData(Int3 pos, DataChunk chunkData)
	{
		// One-time only!
		if (_state == State.Fresh)
		{
			_state = State.Prepped;

			_chunkSize = World.GetChunkSize();
			_chunkPos = pos;
			_chunkData = chunkData;
		}
	}

	// We don't want others changing the state,
	// so we'll ping the chunk to update it for itself
	public void UpdateState()
	{
		switch (_state)
		{
			case State.Generating:
				if (_chunkData.IsGenerated())
				{
					_state = State.Loaded;

					GenerateMesh();
				}
				break;
		}
	}

	void Start()
	{
		_mesh = GetComponent<MeshFilter>().mesh;
		_col = GetComponent<MeshCollider>();

		if (_chunkPos.Equals(new Int3(-4, 0, -4)))
		{
			GameObject prefab = Instantiate(_troopCenter, transform);

            // Set location
			Vector3 pos = prefab.transform.localPosition;
			pos.x = 8f;
			pos.y = 7.5f;
			pos.z = 8f;
			prefab.transform.localPosition = pos;

            // Set team
            prefab.GetComponent<TroopCenter>().SetTeam(Selectable.Team.Red);
		}

        if (_chunkPos.Equals(new Int3(4, 0, 4)))
        {
            GameObject prefab = Instantiate(_townCenter, transform);

            // Set location
            Vector3 pos = prefab.transform.localPosition;
            pos.x = 8f;
            pos.y = 7.5f;
            pos.z = 8f;
            prefab.transform.localPosition = pos;

            // Set team
            prefab.GetComponent<TroopCenter>().SetTeam(Selectable.Team.Blue);
        }
    }

	void Update()
	{
		// Should my mesh be updated?
		if (_updateMesh)
		{
			_updateMesh = false;
			UpdateMesh();
		}

		if (isolateMesh != _updateIso)
		{
			_updateIso = isolateMesh;
			GenerateMesh();
		}
	}

	public void GenerateBlocks()
	{
		// Check if data chunk blocks are generated
		if (!_chunkData.IsGenerated())
		{
			_state = State.Generating;
			_chunkData.GenerateBlocks();
		}
	}

	public void GenerateMesh()
	{
		// Iterate through x, y, z
		for (int x = 0; x < _chunkSize; x++)
		{
			for (int y = 0; y < _chunkSize; y++)
			{
				for (int z = 0; z < _chunkSize; z++)
				{
					Atlas.ID block = Block(x, y, z);

					// Generate the mesh and texturize
					if (block != Atlas.ID.Air)
					{
						if (Block(x, y + 1, z) == Atlas.ID.Air)
						{
							CubeUp(x, y, z, block);
						}

						if (Block(x, y - 1, z) == Atlas.ID.Air)
						{
							CubeDown(x, y, z, block);
						}

						if (Block(x + 1, y, z) == Atlas.ID.Air)
						{
							CubeEast(x, y, z, block);
						}

						if (Block(x - 1, y, z) == Atlas.ID.Air)
						{
							CubeWest(x, y, z, block);
						}

						if (Block(x, y, z + 1) == Atlas.ID.Air)
						{
							CubeNorth(x, y, z, block);
						}

						if (Block(x, y, z - 1) == Atlas.ID.Air)
						{
							CubeSouth(x, y, z, block);
						}
					}
				}
			}
		}

		_clearMesh = true;
		_updateMesh = true;
	}

	// Local block to world blocks
	private Atlas.ID Block(int x, int y, int z)
	{
		if (x >= 0 && x < _chunkSize && y >= 0 && y < _chunkSize && z >= 0 && z < _chunkSize)
		{
			// In bounds, we have the data available to us
			return _chunkData.GetBlock(x, y, z);
		}
		else if (isolateMesh)
		{
			return Atlas.ID.Air;
		}
		else
		{
			x += _chunkPos.x * _chunkSize;
			y += _chunkPos.y * _chunkSize;
			z += _chunkPos.z * _chunkSize;

			Int3 pos = World.WhichChunk(new Int3(x, y, z));

			DataColumn column = World.GetColumn(pos);

			return World.GenerateBlock(column, x, y, z);
		}
	}

	private void UpdateMesh()
	{
		if (_clearMesh)
		{
			_clearMesh = false;
			_mesh.Clear();
		}
		_mesh.vertices = _newVerts.ToArray();
		_mesh.uv = _newUV.ToArray();
		_mesh.triangles = _newTris.ToArray();
		_mesh.colors = _newColors.ToArray();
		_mesh.RecalculateNormals();

		_col.sharedMesh = null;
		_col.sharedMesh = _mesh;

		//_newVerts.Clear();
		//_newUV.Clear();
		//_newTris.Clear();
		//_newColors.Clear();

		_faceCount = 0;

		// We are done rendering!...for now!
		_state = State.Rendered;
	}

	private void CubeUp(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x, y, z));

		Atlas.Dir dir = Atlas.Dir.Up;
		Color color = Color.white;

		if (block == Atlas.ID.Grass)
		{
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeDown(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));
		_newVerts.Add(new Vector3(x, y - 1, z + 1));

		Atlas.Dir dir = Atlas.Dir.Down;
		Color color = Color.white;

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeNorth(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x, y - 1, z + 1));

		Atlas.Dir dir = Atlas.Dir.North;
		Color color = Color.white;

		if (false && block == Atlas.ID.Grass && Block(x, y - 1, z + 1) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeSouth(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z));
		_newVerts.Add(new Vector3(x, y, z));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z));

		Atlas.Dir dir = Atlas.Dir.South;
		Color color = Color.white;

		if (false && block == Atlas.ID.Grass && Block(x, y - 1, z - 1) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeEast(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x + 1, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));

		Atlas.Dir dir = Atlas.Dir.East;
		Color color = Color.white;

		if (false && block == Atlas.ID.Grass && Block(x + 1, y - 1, z) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeWest(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z + 1));
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x, y, z));
		_newVerts.Add(new Vector3(x, y - 1, z));

		Atlas.Dir dir = Atlas.Dir.West;
		Color color = Color.white;
		
		if (false && block == Atlas.ID.Grass && Block(x - 1, y - 1, z) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void Cube(Vector2 texturePos)
	{
		_newTris.Add(_faceCount * 4); //1
		_newTris.Add(_faceCount * 4 + 1); //2
		_newTris.Add(_faceCount * 4 + 2); //3
		_newTris.Add(_faceCount * 4); //1
		_newTris.Add(_faceCount * 4 + 2); //3
		_newTris.Add(_faceCount * 4 + 3); //4

		float tUnit = Atlas.tUnit;

		_newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
		_newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
		_newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
		_newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

		_faceCount++;
	}

	public State GetState()
	{
		return _state;
	}
}
