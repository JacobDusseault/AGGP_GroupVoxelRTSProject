using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopCenter : Building
{
    public GameObject _soldier;

    protected new const int _maxHealth = 1000;

	private int _gold = 60;
	private int _food = 600;

	protected override void Start()
	{
		//base.Start();
		_health = _maxHealth;
		InvokeRepeating("Resources", 1f, 1f);
	}

	protected override void Update()
	{
        base.Update();

		if (GetTeam() == Selectable.Team.Red)
		{
			if (GetSelect())
			{
				if (Input.GetKeyDown(KeyCode.Space) && _food > 50)
				{
					Queue(new Action("Soldier", 1f));
					_food -= 50;
				}
			}
		}
		else
		{
			if (Random.Range(0, 240) == 0 && _food > 50)
			{
				Queue(new Action("Soldier", 1f));
				_food -= 50;
			}
		}
	}

	private void Resources()
	{
		_gold += 1;
		_food += 10;
	}

	protected override void ActOn(string action)
	{
		GameObject unit;

		switch (action)
        {
            case "Soldier":
                unit = Instantiate(_soldier, transform.position + Vector3.back * 10f + Vector3.up * 5f, Quaternion.identity);

                unit.GetComponent<Selectable>().SetTeam(GetTeam());
                break;

			default:
                Debug.Log("Unknown action!");
                break;
        }

        //Debug.Log("Action completed: " + action);
	}
}
