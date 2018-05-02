using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopCenter : Building
{
    public GameObject _soldier;

    protected new const int _maxHealth = 100;

    protected override void Update()
	{
        base.Update();

        if (GetSelect() && Input.GetKeyDown("space"))
		{
            Queue(new Action("Soldier", 1f));
		}
	}

	protected override void ActOn(string action)
	{
        switch (action)
        {
            case "Soldier":
                GameObject unit = Instantiate(_soldier, transform.position + Vector3.back * 10f, Quaternion.identity);

                unit.GetComponent<Selectable>().SetTeam(Selectable.Team.Red);
                break;

            default:
                Debug.Log("Unknown action!");
                break;
        }

        Debug.Log("Action completed: " + action);
	}
}
