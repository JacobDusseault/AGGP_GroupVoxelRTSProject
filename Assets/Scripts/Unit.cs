using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
	[SerializeField] private float _speed = 5f;
	private CharacterController _chara;
	private Vector3 _moveTo;
	private bool _indiscriminateAttack;

	void Start ()
	{
		_chara = GetComponent<CharacterController>();
		_moveTo = transform.position;
	}
	
	void Update ()
	{
        if (team == Team.Red)
        {
            // Actions
            if (_selected && Input.GetMouseButtonUp(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 8)))
                {
                    _moveTo = hit.point;
                }
            }
        }

		_moveTo.y = transform.position.y;

		// Move to position
		if (Vector3.Distance(transform.position, _moveTo) > 1f)
		{
			Vector3 dir = _moveTo - transform.position;

			dir.Normalize();

			_chara.Move(dir * Time.deltaTime * _speed);
		}
		
		_chara.Move(Vector3.down * Time.deltaTime * 5f); // Dodo fall-off-cliff physics
	}

    protected override void Kill()
    {
        Destroy(gameObject);
    }
}
