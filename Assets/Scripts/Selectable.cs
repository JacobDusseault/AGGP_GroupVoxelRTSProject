using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Selectable : MonoBehaviour
{
    public Image healthBar;
	public enum Team { Neutral, Red, Blue };

	[SerializeField] protected int _maxHealth = 0;
	protected int _health;

	private bool _selected = false;
    private Team _team = Team.Neutral;

    protected virtual void Start()
    {
        _health = _maxHealth;
    }

    public bool GetSelect()
    {
        return _selected;
    }

    protected virtual void SetColor(Color c)
    {
        // Override
    }

    public void Select()
    {
        if (_team == Team.Red)
        {
            _selected = true;
            SetColor(Color.red);
        }
	}

	public void Deselect()
    {
        if (_team == Team.Red)
        {
            _selected = false;
            SetColor(new Color(1f, 0.5f, 0.5f));
        }
	}

    public Team GetTeam()
    {
        return _team;
    }

    public void SetTeam(Team t)
    {
        if (_team == Team.Neutral)
        {
            _team = t;

            if (t == Team.Red) { SetColor(new Color(1f, 0.5f, 0.5f)); }
            else if (t == Team.Blue) { SetColor(new Color(0.5f, 0.5f, 1f)); }
        }
    }

    public int GetHealth()
    {
        return _health;
    }

    public void Hurt(int damage)
    {
        _health -= damage;

        healthBar.fillAmount = (float) _health / (float) _maxHealth;

        if (_health <= 0)
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
