using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float _life = 5f;
	[SerializeField] private int _damage = 10;
	private bool _active = true;
	private Selectable.Team _team = Selectable.Team.Neutral;

	void Start()
	{
		Destroy(gameObject, _life);
	}

	void Update()
	{

	}

	public void SetTeam(Selectable.Team t)
	{
		if (_team == Selectable.Team.Neutral)
		{
			_team = t;

			if (t == Selectable.Team.Red) { SetColor(new Color(1f, 0.75f, 0.75f)); }
			else if (t == Selectable.Team.Blue) { SetColor(new Color(0.75f, 0.75f, 1f)); }
		}
	}

	private void SetColor(Color c)
	{
		GetComponentInParent<Renderer>().material.color = c;
	}

	void OnTriggerEnter(Collider col)
	{
		if (_active)
		{
			Selectable pawn = col.gameObject.GetComponent<Selectable>();
			
			if (pawn && pawn.GetTeam() != _team)
			{
				pawn.Hurt(_damage);
				_active = false;
			}
		}
	}
}