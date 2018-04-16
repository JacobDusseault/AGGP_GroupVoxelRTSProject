using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	private CharacterController _chara;

	private Vector3 _moveTo;
	private bool _indiscriminateAttack;

	// Selection
	[SerializeField] private float _epsilonSelect = 0.1f; // Drag fudging
	private static Vector2 _startSelect;
	private bool _selected;

	void Start ()
	{
		_chara = GetComponent<CharacterController>();
		_moveTo = transform.position;
	}
	
	void Update ()
	{
		// Start select
		if (Input.GetMouseButtonDown(0))
		{
			_startSelect = Input.mousePosition;
		}

		// End select
		if (Input.GetMouseButtonUp(0))
		{
			// Drag
			if (Vector3.Distance(_startSelect, Input.mousePosition) > _epsilonSelect)
			{
				_selected = true; // temp
			}
			else // Click
			{
				_selected = false;


			}
		}

		// Actions
		if (_selected && Input.GetMouseButtonUp(1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				_moveTo = hit.point;
			}
		}

		_moveTo.y = transform.position.y;

		// Move to position
		if (Vector3.Distance(transform.position, _moveTo) > 1f)
		{
			Vector3 dir = _moveTo - transform.position;

			dir.Normalize();

			_chara.Move(dir * Time.deltaTime * 3f);
		}
		
		_chara.Move(Vector3.down * Time.deltaTime * 5f); // Dodo fall-off-cliff physics
	}
}
