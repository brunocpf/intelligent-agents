using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{

	public SwitchHandler[] targets;
	public bool triggerOnCollision = true;

	public enum SwitchState
	{
		OFF,
		ON
	}

	public SwitchState State
	{
		get
		{
			return _state;
		}
		set
		{
			OnStateChange(value);
		}
	}

	private SwitchState _state = SwitchState.OFF;

	private void OnStateChange(SwitchState newValue)
	{
		if (_state != newValue)
		{
			if (newValue == SwitchState.OFF)
			{
				_state = SwitchState.OFF;
				foreach (SwitchHandler target in targets)
				{
					target.OnSwitchOff();
				}
			}
			else if (newValue == SwitchState.ON)
			{
				_state = SwitchState.ON;
				foreach (SwitchHandler target in targets)
				{
					target.OnSwitchTrigger();
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Agent" && State == SwitchState.OFF)
		{
			State = SwitchState.ON;
		}
	}

}