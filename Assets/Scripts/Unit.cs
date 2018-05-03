using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
	public GameObject _rock;

	[SerializeField] private float _speed = 5f;
	[SerializeField] private float _attackRate = 1f;
	[SerializeField] protected new int _maxHealth = 100;
	private CharacterController _chara;
	private Vector3 _moveTo;
	private bool _indiscriminateAttack = true;
	private float _attackDelay = 0f;
	private GameObject _attackTarget;

	private float _timer = 0f;

	protected override void Start ()
	{
		//base.Start();
		_health = _maxHealth;

		_chara = GetComponent<CharacterController>();
		_moveTo = transform.position;

		if (GetTeam() == Team.Blue)
		{
			Vector2 circ = Random.insideUnitCircle * 10f;
			Vector3 flatCirc = new Vector3(circ.x, 0, circ.y);

			_moveTo = new Vector3(-40, 0, -60) + flatCirc;
		}
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
					Vector2 circ = Random.insideUnitCircle * 5f;
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
		// Timer stuff
		_timer -= Time.deltaTime;
		if (_timer <= 0)
		{
			FindTarget();
			_timer = 5f;
		}

		_attackDelay -= Time.deltaTime;
		if (_attackDelay <= 0f && _attackTarget != null) // Check if exists
		{
			Attack(_attackTarget);
		}
	}
	
	protected void Attack(GameObject enemy)
	{
		GameObject rock = Instantiate(_rock, transform.position + Vector3.up * 0f, Quaternion.identity);
		Vector3 dir = (enemy.transform.position + Random.insideUnitSphere * 1.5f) - rock.transform.position;
		rock.GetComponent<Rigidbody>().AddForce(dir.normalized * 1500f);
		rock.GetComponentInChildren<Projectile>().SetTeam(GetTeam());

		_attackDelay = _attackRate + Random.Range(-0.2f, 0.2f);
	}

	protected void FindTarget()
	{
		if (_indiscriminateAttack && !_attackTarget)
		{
			Unit[] pawns = FindObjectsOfType<Unit>();

			for (int i = 0; i < pawns.Length; ++i)
			{
				// Opposite team
				if (pawns[i].GetTeam() != GetTeam() && Vector3.Distance(gameObject.transform.position, pawns[i].transform.position) < 45f)
				{
					_attackTarget = pawns[i].gameObject;
					if (GetTeam() == Team.Blue)
					{
						_moveTo = transform.position; //stop moving
					}
					break;
				}
			}

			if (GetTeam() == Team.Blue && !_attackTarget)
			{
				Vector2 circ = Random.insideUnitCircle * 10f;
				Vector3 flatCirc = new Vector3(circ.x, 0, circ.y);

				_moveTo = new Vector3(-40, 0, -60) + flatCirc;
			}
		}
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
