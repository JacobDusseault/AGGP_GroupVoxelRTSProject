using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopCenter : Building
{
    protected new const int _maxHealth = 100;
	
	void Update ()
	{
		if (Input.GetKeyDown("space"))
		{
			ActOn("Test");
		}
	}

	protected override void ActOn(string action)
	{
		Debug.Log("Space pressed!");
	}
}
