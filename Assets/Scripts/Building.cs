using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable
{
	protected Queue<Action> _queue;
	protected bool _active = false;
	protected float _timer = 0f;
    
	void Update ()
	{
		// Are there things in the queue that need dequeueing?
		if (_active)
		{
			// Timer stuff
			_timer -= Time.deltaTime;
			if (_timer <= 0)
			{
				ActOn(_queue.Dequeue().GetName());

				// Check if there's more to do
				if (_queue.Count > 0)
				{
					_timer = _queue.Peek().GetDuration();
				}
			}
		}
	}

	protected void Queue(Action action)
	{
		_queue.Enqueue(action);
		_active = true;
		_timer = action.GetDuration();
	}

	public void Activate()
	{
		_active = true;
	}

	public void Deactivate()
	{
		_active = false;
	}
	
	protected virtual void ActOn(string action)
	{
		// Override
	}
}
