using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
	public GameObject _rock;

	[SerializeField] private float _speed = 5f;
	[SerializeField] private float _attackRate = 1f;
	private CharacterController _chara;
	private Vector3 _moveTo;
	private bool _indiscriminateAttack = true;
	private float _attackDelay = 0f;
	private GameObject _attackTarget;

	void Start ()
	{
		_chara = GetComponent<CharacterController>();
		_moveTo = transform.position;
	}
	
	void Update ()
	{
        if (GetTeam() == Team.Red)
        {
            // Actions
            if (GetSelect() && Input.GetMouseButtonUp(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 8)))
                {
					Vector2 circ = Random.insideUnitCircle * 5;
					Vector3 flatCirc = new Vector3(circ.x, 0, circ.y);

					_moveTo = hit.point + flatCirc;
                }
            }
        }

		_moveTo.y = transform.position.y;

		// Move to position
		if (Vector3.Distance(transform.position, _moveTo) > 0.5f)
		{
			Vector3 dir = _moveTo - transform.position;

			dir.Normalize();

			_chara.Move(dir * Time.deltaTime * _speed);
		}
		
		_chara.Move(Vector3.down * Time.deltaTime * 5f); // Dodo fall-off-cliff physics

		// Attacking
		if (_indiscriminateAttack && !_attackTarget)
		{
			Unit[] pawns = FindObjectsOfType<Unit>();

			for (int i = 0; i < pawns.Length; ++i)
			{
				// Opposite team
				if (pawns[i].GetTeam() != GetTeam())
				{
					_attackTarget = pawns[i].gameObject;
					break;
				}
			}
		}

		_attackDelay -= Time.deltaTime;
		if (_attackTarget != null && _attackDelay <= 0f) // Check if exists
		{
			Attack(_attackTarget);
		}
	}
	
	protected void Attack(GameObject enemy)
	{
		GameObject rock = Instantiate(_rock, transform.position + Vector3.up * 2f, Quaternion.identity);
		Vector3 dir = enemy.transform.position - rock.transform.position;
		rock.GetComponent<Rigidbody>().AddForce(dir * 150f);

		_attackDelay = _attackRate;
	}

    protected override void SetColor(Color c)
    {
        GetComponent<Renderer>().material.color = c;
    }

    protected override void Kill()
    {
        Destroy(gameObject);
    }
}
