using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
	public enum Team { Neutral, Red, Blue };

	public Team team = Team.Neutral;

	protected bool _selected = false;

    private int _health;
    protected const int _maxHealth = 0;

    void Start()
    {
        _health = _maxHealth;
    }

    public void Select()
    {
        if (team == Team.Red)
        {
            _selected = true;
            GetComponent<Renderer>().material.color = Color.red;
        }
	}

	public void Deselect()
    {
        if (team == Team.Red)
        {
            _selected = false;
            GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.5f);
        }
	}

    public void SetTeam(Team t)
    {
        team = t;
    }

    public int GetHealth()
    {
        return _health;
    }

    public void Hurt(int damage)
    {
        _health -= damage;
        if (_health < 0)
        {
            Kill();
        }
    }

    public void Heal(int health)
    {
        _health += health;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }

    protected virtual void Kill()
    {
        // Override
    }
}
