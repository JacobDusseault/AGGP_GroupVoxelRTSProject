using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
	public enum Team { Neutral, Red, Blue };

	public Team team = Team.Neutral;

	protected bool _selected = false;

	public void Select()
	{
		_selected = true;
		GetComponent<Renderer>().material.color = Color.red;
	}

	public void Deselect()
	{
		_selected = false;
		GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.5f);
	}
}
