using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable
{
	private Queue<Action> _queue = new Queue<Action>();
    private bool _active = false;
    private float _timer = 0f;

    protected virtual void Update()
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
                else
                {
                    _active = false;
                }
            }
        }
    }

    protected void Queue(Action action)
	{
		_queue.Enqueue(action);
        if (!_active) { _timer = action.GetDuration(); }
		_active = true;
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
