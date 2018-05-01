using UnityEngine;
using DG.Tweening;

public class Wall : SwitchHandler
{
	public bool lowerable = true;

	public override void OnSwitchOff()
	{
		this.gameObject.layer = 0;
		transform.localPosition = Vector3.zero;
	}
	public override void OnSwitchTrigger()
	{
		if (lowerable) {
			this.gameObject.layer = 2;
			transform.DOMoveY(-6.9f, 1.0f);
		}
	}
}