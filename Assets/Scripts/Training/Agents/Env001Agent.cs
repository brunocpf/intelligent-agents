using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env001Agent : Agent
{

	public Transform goal;
	public float speed = 5.0f;
	public LayerMask goalDetectionMask;
	public Transform floor;
	public SwitchTrigger wallSwitch;
	public float minDistance = 1.5f;
	public Gradient targetGradient;

	private Rigidbody rigidBody;
	private LineRenderer lineRenderer;
	private LineRenderer targetLineRenderer;
	private float floorWidth;
	private float floorHeight;
	private float lastStepDistance = Mathf.Infinity;
	private bool rayHitGoal = false;

	private void Awake()
	{
		var renderers = GetComponentsInChildren<LineRenderer>();
		rigidBody = GetComponent<Rigidbody>();
		lineRenderer = renderers[0];
		targetLineRenderer = renderers[1];
	}

	private void Start()
	{
		Vector3 floorSize =  floor.GetComponent<Renderer>().bounds.size;
		floorWidth = floorSize.x;
		floorHeight = floorSize.z;
	}

	public override void AgentReset()
	{
		float newX, newZ;
		Transform wallSwitchTransform = wallSwitch.transform;
		rigidBody.velocity = Vector3.zero;

		newX = Random.Range(-floorWidth/2, floorWidth/2);
		newX += (newX <= 0.5 && newX >= 0.5) ? Random.Range(-1, 1) : 0;
		newZ = Random.Range(-floorHeight/2, floorHeight/2);
		wallSwitchTransform.localPosition = new Vector3(newX, 0f, newZ);

		newX = Random.Range(-floorWidth/2, floorWidth/2);
		newX += (newX <= 0.5 && newX >= 0.5) ? Random.Range(-1, 1) : 0;
		newZ = Random.Range(-floorHeight/2, floorHeight/2);
		transform.localPosition = new Vector3(newX, 1.1f, newZ);

		if (wallSwitchTransform.localPosition.x < 0)
			newX = Random.Range(1.5f, floorWidth/2);
		else
			newX = Random.Range(-floorWidth/2, -1.5f);
		newZ = Random.Range(-floorHeight/2, floorHeight/2);
		goal.localPosition = new Vector3(newX, 0f, newZ);

		wallSwitch.State = SwitchTrigger.SwitchState.OFF;
		lastStepDistance = Mathf.Infinity;
		rayHitGoal = false;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		float distance = Vector3.Distance(goal.position, transform.position);

		// Reached goal
		if (distance < minDistance)
		{
			Done();
			AddReward(1.0f);
		}

		Transform target = rayHitGoal ? goal : wallSwitch.transform;
		float newDistance = Vector3.Distance(target.position, transform.position);
		float maxDistance = IAUtils.HypotenuseLength(floorWidth, floorHeight);
		
		targetLineRenderer.SetPosition(0, transform.position + Vector3.up);
		targetLineRenderer.SetPosition(1, target.position);
		targetLineRenderer.startColor = targetLineRenderer.endColor = targetGradient.Evaluate(Mathf.InverseLerp(0.0f, maxDistance, newDistance));

		// Getting closer
		if (newDistance < lastStepDistance)
		{
			AddReward(0.1f);
		}
		if (newDistance != lastStepDistance)
		{
			//Debug.Log("Step distance:" + newDistance);
			lastStepDistance = newDistance;
		}

		// Time penalty
		AddReward(-0.05f);

		Vector3 controlSignal = Vector3.zero;
		controlSignal.x = Mathf.Clamp(vectorAction[0], -1, 1);
		controlSignal.z = Mathf.Clamp(vectorAction[1], -1, 1);
		rigidBody.velocity = controlSignal * speed;
	}

	public override void CollectObservations()
	{
		Vector3 goalRelativePosition = goal.position - transform.position;
		Vector3 switchRelativePosition = wallSwitch.transform.position - transform.position;
		
		float maxDistance = IAUtils.HypotenuseLength(floorWidth, floorHeight);

		rayHitGoal = false;
		float distance = Vector3.Distance(goal.position, transform.position);
		float distanceNormalized = Mathf.InverseLerp(0.0f, maxDistance, distance);
		//Debug.Log("Normalized distance:" + distanceNormalized);

		float distanceToSwitch = Vector3.Distance(wallSwitch.transform.position, transform.position);
		float distanceToSwitchNormalized = Mathf.InverseLerp(0.0f, maxDistance, distanceToSwitch);


		RaycastHit hit;
		if (Physics.Raycast(transform.position, goalRelativePosition, out hit, maxDistance, goalDetectionMask))
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

		// Can reach goal
		AddVectorObs(rayHitGoal ? 1.0f : 0.0f);

		// Switch is on
		AddVectorObs(wallSwitch.State == SwitchTrigger.SwitchState.ON ? 1.0f : 0.0f);

		// Relative position to goal
		AddVectorObs(goalRelativePosition.x * 2 / floorWidth);
		AddVectorObs(goalRelativePosition.z * 2 / floorHeight);

		// Relative position to switch
		AddVectorObs(switchRelativePosition.x * 2 / floorWidth);
		AddVectorObs(switchRelativePosition.z * 2 / floorHeight);

		// Own position
		AddVectorObs(transform.localPosition.x * 2 / floorWidth);
		AddVectorObs(transform.localPosition.z * 2 / floorHeight);

		// Distance to goal
		//AddVectorObs(distanceNormalized);

		// Distance to switch
		//AddVectorObs(distanceToSwitchNormalized);
	}

	/* 
	private Env001Academy academy;
	public GameObject goal;
	public float speed;

	private void Awake()
	{
		academy = FindObjectOfType<Env001Academy>(); //cache the academy
	}

	public override void InitializeAgent()
	{
		academy = FindObjectOfType(typeof(BasicAcademy)) as BasicAcademy;
	}

	public override void CollectObservations()
	{
		AddVectorObs(position);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		float movement = vectorAction[0];
		int direction = 0;
		if (movement == 0) { direction = -1; }
		if (movement == 1) { direction = 1; }

		position += direction;
		if (position < minPosition) { position = minPosition; }
		if (position > maxPosition) { position = maxPosition; }

		gameObject.transform.position = new Vector3(position, 0f, 0f);

		AddReward(-0.01f);

		if (position == smallGoalPosition)
		{
			Done();
			AddReward(0.1f);
		}

		if (position == largeGoalPosition)
		{
			Done();
			AddReward(1f);
		}
	}

	public override void AgentReset()
	{
		position = 0;
		minPosition = -10;
		maxPosition = 10;
		smallGoalPosition = -3;
		largeGoalPosition = 7;
		smallGoal.transform.position = new Vector3(smallGoalPosition, 0f, 0f);
		largeGoal.transform.position = new Vector3(largeGoalPosition, 0f, 0f);
	}

	public override void AgentOnDone()
	{

	}

	public void FixedUpdate()
	{
		WaitTimeInference();
	}

	private void WaitTimeInference()
	{
		if (!academy.GetIsInference())
		{
			RequestDecision();
		}
		else
		{
			if (timeSinceDecision >= timeBetweenDecisionsAtInference)
			{
				timeSinceDecision = 0f;
				RequestDecision();
			}
			else
			{
				timeSinceDecision += Time.fixedDeltaTime;
			}
		}
	}
*/
}