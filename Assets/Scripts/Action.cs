using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Action
{
	private string _name;
	private float _duration;

	public Action(string name, float duration)
	{
		_name = name;
		_duration = duration;
	}

	public string GetName()
	{
		return _name;
	}

	public float GetDuration()
	{
		return _duration;
	}
}
