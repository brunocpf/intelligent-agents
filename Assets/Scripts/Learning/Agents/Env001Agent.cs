using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env001Agent : Agent
{

	public Transform goal;
	public Transform ground;
	public SwitchTrigger wallSwitch;
	public float speed = 5.0f;
	public float minDistance = 1.5f;
	public LayerMask objectOverlapLayerMask;
	public LayerMask goalDetectionMask;
	public Gradient targetGradient;

	private RayPerception rayPerception;

	private Rigidbody rigidBody;
	private Renderer groundRenderer;
	private LineRenderer lineRenderer;
	private LineRenderer targetLineRenderer;
	private float floorWidth;
	private float floorHeight;
	private float lastStepDistance = Mathf.Infinity;
	private bool rayHitGoal = false;

	private float GroundWidth
	{
		get
		{
			return groundRenderer.bounds.size.x;
		}
	}

	private float GroundHeight
	{
		get
		{
			return groundRenderer.bounds.size.x;
		}
	}

	private Transform CurrentTarget
	{
		get
		{
			return rayHitGoal ? goal : wallSwitch.transform;
		}
	}

	private enum AgentMoveDirection
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}

	private void Awake()
	{
		groundRenderer = ground.GetComponent<Renderer>();
		rayPerception = GetComponent<RayPerception>();

		var renderers = GetComponentsInChildren<LineRenderer>();
		rigidBody = GetComponent<Rigidbody>();
		lineRenderer = renderers[0];
		targetLineRenderer = renderers[1];
	}

	public override void AgentReset()
	{
		Transform wallSwitchTransform = wallSwitch.transform;
		rigidBody.velocity = Vector3.zero;

		wallSwitch.transform.PlaceRandomly(ground, objectOverlapLayerMask, wallSwitch.transform.position.y);

		transform.PlaceRandomly(ground, objectOverlapLayerMask, 2.0f, transform.position.y);

		do
		{
			goal.PlaceRandomly(ground, objectOverlapLayerMask, 2.0f, goal.position.y);
		} while (Mathf.Sign(goal.localPosition.x) == Mathf.Sign(wallSwitch.transform.localPosition.x));

		wallSwitch.State = SwitchTrigger.SwitchState.OFF;
		lastStepDistance = Mathf.Infinity;
		rayHitGoal = false;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		float distance = Vector3.Distance(goal.position, transform.position);
		float switchDistance = Vector3.Distance(wallSwitch.transform.position, transform.position);
		float targetDistance = Vector3.Distance(CurrentTarget.position, transform.position);


		MoveAgent((AgentMoveDirection) Mathf.FloorToInt(vectorAction[0]));

		// Reached goal
		if (distance < minDistance)
		{
			Done();
			SetReward(1.0f);
		}
		//Time penalty
		else
		{
			//AddReward(-1.0f / agentParameters.maxStep);
		}

		if (!rayHitGoal && switchDistance < minDistance)
		{
			AddReward(0.5f);
		}
		// Getting closer
		else if (targetDistance < lastStepDistance)
		{
			AddReward(1.0f / agentParameters.maxStep);
		}

		Monitor.Log("Reward", Mathf.Sign(GetReward()), MonitorType.slider);
		lastStepDistance = targetDistance;
	}

	private void MoveAgent(AgentMoveDirection dir)
	{
		/*Vector3 controlSignal = Vector3.zero;
		controlSignal.x = Mathf.Clamp(vectorAction[0], -1, 1);
		controlSignal.z = Mathf.Clamp(vectorAction[1], -1, 1);
		rigidBody.velocity = controlSignal * speed;*/
		Vector3 vel;
		switch (dir)
		{
			case AgentMoveDirection.UP:
				vel = Vector3.forward * speed;
				break;
			case AgentMoveDirection.DOWN:
				vel = Vector3.back * speed;
				break;
			case AgentMoveDirection.LEFT:
				vel = Vector3.left * speed;
				break;
			case AgentMoveDirection.RIGHT:
				vel = Vector3.right * speed;
				break;
			default:
				vel = Vector3.zero;
				break;
		}
		rigidBody.velocity = vel;
	}

	public override void CollectObservations()
	{

		Vector3 goalRelativePosition = transform.position.RelativePosition(goal.position);
		Vector3 switchRelativePosition = transform.position.RelativePosition(wallSwitch.transform.position);
		Vector3 positionOnGround = ground.position.RelativePosition(transform.position);
		float distanceToGoal = Vector3.Distance(transform.position, goal.position);
		float distanceToSwitch = Vector3.Distance(transform.position, goal.position);

		float widthFactor = 1.0f / GroundWidth;
		float heightFactor = 1.0f / GroundHeight;
		float diagonalLength = IAUtils.HypotenuseLength(GroundWidth, GroundHeight);

		UpdateRays();

		float[] obs = {
			// Goal position
			goalRelativePosition.x * widthFactor,
			goalRelativePosition.z * heightFactor,

			// Switch position
			switchRelativePosition.x * widthFactor,
			switchRelativePosition.z * heightFactor,

			// Position on ground
			positionOnGround.x * widthFactor,
			positionOnGround.z * heightFactor,

			// Distance to goal
			distanceToGoal / diagonalLength,

			// Distance to switch
			distanceToSwitch / diagonalLength,

			// Can reach goal
			rayHitGoal ? 1 : 0,

			// Switch was triggered,
			wallSwitch.State == SwitchTrigger.SwitchState.ON ? 1 : 0,

			// Step count
			//(float) GetStepCount() / (float) agentParameters.maxStep
		};

		AddVectorObs(obs);
	}

	private void UpdateRays()
	{
		float distance = Vector3.Distance(CurrentTarget.position, transform.position);
		float maxDistance = IAUtils.HypotenuseLength(GroundWidth, GroundHeight);

		rayHitGoal = false;

		RaycastHit hit;
		if (Physics.Linecast(transform.position, goal.position, out hit, goalDetectionMask))
		{
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, hit.point);
			lineRenderer.startColor = lineRenderer.endColor = Color.red;
			if (hit.transform == goal)
			{
				lineRenderer.startColor = lineRenderer.endColor = Color.green;
				rayHitGoal = true;
			}
		}

		targetLineRenderer.SetPosition(0, transform.position + Vector3.up);
		targetLineRenderer.SetPosition(1, CurrentTarget.position);
		targetLineRenderer.startColor = targetLineRenderer.endColor = targetGradient.Evaluate(Mathf.InverseLerp(0.0f, maxDistance, distance));
	}
}