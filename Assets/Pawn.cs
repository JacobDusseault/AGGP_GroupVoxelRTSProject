using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	private CharacterController _chara;

	private Vector2 _moveTo;
	private bool _indiscriminateAttack;

	// Selection
	[SerializeField] private float _epsilonSelect = 0.1f; // Drag fudging
	private static Vector2 _startSelect;
	private bool _selected;

	void Start ()
	{
		_chara = GetComponent<CharacterController>();
	}
	
	void Update ()
	{
		_chara.Move(Vector3.forward * Time.deltaTime * 3f); // Test movement
		_chara.Move(Vector3.down * Time.deltaTime * 5f); // Dodo fall-off-cliff physics

		// Start select
		if (Input.GetMouseButtonDown(0))
		{
			//_startTimeSelect = Time.time;
			_startSelect = Input.mousePosition;
		}

		// End select
		if (Input.GetMouseButtonUp(0))
		{
			// Drag
			if (Vector3.Distance(_startSelect, Input.mousePosition) > _epsilonSelect)
			{
				
			}
			else // Click
			{
				_selected = false;


			}
			
			// No need to reset prior variables, gets over-written on next click
		}
	}
}
