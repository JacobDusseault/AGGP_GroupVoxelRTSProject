using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	private CharacterController _chara;

	private Vector2 _moveTo;
	private bool _indiscriminateAttack;

	// Selection
	[SerializeField] private float _epsilonSelect = 0.1f; // How long must mouse be held to consider drag selection
	private float _startTimeSelect;
	private static Vector2 _startPosSelect;
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
			_startTimeSelect = Time.time;
			_startPosSelect = Input.mousePosition;
		}

		// End select
		if (Input.GetMouseButtonUp(0))
		{
			// Click
			if (Time.time - _startTimeSelect < _epsilonSelect)
			{

			}
			else // Drag
			{
				// _startPosSelect
			}
			
			// No need to reset prior variables, gets over-written on next click
		}
	}
}
