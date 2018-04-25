using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	// Selection
	[SerializeField] private float _epsilonSelect = 0.1f; // Drag fudging
	private static Vector2 _startSelect = Vector2.zero;
	private static Vector2 _endSelect = Vector2.zero;
	
	void Update ()
	{
		// Start select
		if (Input.GetMouseButtonDown(0))
		{
			_startSelect = Input.mousePosition;
		}
		_endSelect = Input.mousePosition;

		// End select
		if (Input.GetMouseButtonUp(0))
		{
			Pawn[] pawns = FindObjectsOfType<Pawn>();
			
			// Normalize
			float xS = Mathf.Min(_startSelect.x, _endSelect.x);
			float yS = Mathf.Min(_startSelect.y, _endSelect.y);
			float xE = Mathf.Max(_startSelect.x, _endSelect.x);
			float yE = Mathf.Max(_startSelect.y, _endSelect.y);

			// Drag
			if (Vector2.Distance(_startSelect, _endSelect) > _epsilonSelect)
			{
				for (int i = 0; i < pawns.Length; ++i)
				{
					Vector2 pos = Camera.main.WorldToScreenPoint(pawns[i].transform.position);

					if (pos.x >= xS && pos.x <= xE && pos.y >= yS && pos.y <= yE)
					{
						pawns[i].Select();
					}
					else
					{
						pawns[i].Deselect();
					}
				}
			}
			else // Click
			{
				for (int i = 0; i < pawns.Length; ++i)
				{
					pawns[i].Deselect();
				}

				Ray ray = Camera.main.ScreenPointToRay(_endSelect);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8)))
				{
					Pawn pawn = hit.collider.gameObject.GetComponent<Pawn>();
					
					if (pawn)
					{
						pawn.Select();
					}
				}
			}

			_startSelect = Vector2.zero;
		}
	}

	void OnGUI()
	{
		if (_startSelect != Vector2.zero)
		{
			// Create a rect from both mouse positions
			var rect = Utils.GetScreenRect(_startSelect, _endSelect);
			Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
			Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
		}
	}
}
