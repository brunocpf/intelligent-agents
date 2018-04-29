using UnityEngine;

public abstract class SwitchHandler : MonoBehaviour
{
	public abstract void OnSwitchOff();

	public abstract void OnSwitchTrigger();
}