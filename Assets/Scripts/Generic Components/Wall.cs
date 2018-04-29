using UnityEngine;
using DG.Tweening;

public class Wall : SwitchHandler
{
	public bool lowerable = true;

	public override void OnSwitchOff()
	{
		transform.localPosition = Vector3.zero;
	}
	public override void OnSwitchTrigger()
	{
		if (lowerable)
			transform.DOMoveY(-6.9f, 1.0f);
	}
}