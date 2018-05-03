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

        if (GetSelect())
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Queue(new Action("Soldier", 1f));
			}
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				Queue(new Action("Enemy", 1f));
			}
		}
	}

	protected override void ActOn(string action)
	{
		GameObject unit;

		switch (action)
        {
            case "Soldier":
                unit = Instantiate(_soldier, transform.position + Vector3.back * 10f, Quaternion.identity);

                unit.GetComponent<Selectable>().SetTeam(Selectable.Team.Red);
                break;

			case "Enemy":
				unit = Instantiate(_soldier, transform.position + Vector3.back * 10f, Quaternion.identity);

				unit.GetComponent<Selectable>().SetTeam(Selectable.Team.Blue);
				break;

			default:
                Debug.Log("Unknown action!");
                break;
        }

        Debug.Log("Action completed: " + action);
	}
}
