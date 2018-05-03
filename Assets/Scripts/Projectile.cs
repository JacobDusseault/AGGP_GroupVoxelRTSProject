using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float _life = 5f;
	private bool _active = true;

	void Start()
	{
		Destroy(gameObject, _life);
	}

	void Update()
	{

	}

	void OnCollisionEnter(Collision collision)
	{

	}
}