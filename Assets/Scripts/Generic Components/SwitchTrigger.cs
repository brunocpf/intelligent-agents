using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SwitchTrigger : MonoBehaviour
{

	public SwitchHandler[] targets;
	public bool triggerOnCollision = true;
	public SwitchState defaultState = SwitchState.OFF;
	public bool stateChangePermanent = true;
	public Material onMaterial;
	public Material offMaterial;

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

	private new Renderer renderer;
	private TextMeshPro textMesh;

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
		textMesh = GetComponentInChildren<TextMeshPro>();
	}

	private void Start()
	{
		State = defaultState;
	}

	private void OnStateChange(SwitchState newValue)
	{
		if (_state != newValue)
		{
			_state = newValue;
			UpdateState();
			if (newValue == SwitchState.OFF)
			{
				foreach (SwitchHandler target in targets)
				{
					target.OnSwitchOff();
				}
			}
			else if (newValue == SwitchState.ON)
			{
				foreach (SwitchHandler target in targets)
				{
					target.OnSwitchTrigger();
				}
			}
		}
	}

	private void UpdateState()
	{
		switch (State)
		{
			case SwitchState.OFF:
				renderer.material = offMaterial;
				textMesh.text = "OFF";
				transform.DOMoveY(0.0f, 1.0f);
				break;
			case SwitchState.ON:
				renderer.material = onMaterial;
				textMesh.text = "ON";
				transform.DOMoveY(-0.4f, 1.0f);
				break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Agent" && State == SwitchState.OFF)
		{
			State = SwitchState.ON;
		}
	}

	private void ToggleState()
	{
		State = State == SwitchState.OFF ? SwitchState.ON : SwitchState.OFF;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Agent" && State == SwitchState.ON && !stateChangePermanent)
		{
			State = SwitchState.OFF;
		}
	}

}