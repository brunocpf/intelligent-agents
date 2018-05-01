using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IAUtils
{
	public static float NormalizeValue(float value, float minValue, float maxValue)
	{
		return Mathf.InverseLerp(minValue, maxValue, value);
	}

	public static float NormalizeValue(float value, float minValue, float maxValue, float a, float b)
	{
		return (b - a) * ((value - minValue) / (maxValue - minValue)) + a;
	}

	public static float HypotenuseLength(float sideALength, float sideBLength)
	{
		return Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength);
	}

	public static Vector3 RelativePosition(this Vector3 origin, Vector3 targetPosition)
	{
		return origin - targetPosition;
	}

	public static Vector3 RelativePositionNormalized(this Vector3 origin, Vector3 targetPosition, Vector3 maxDistance)
	{
		Vector3 relativePosition = origin.RelativePosition(targetPosition);
		return new Vector3(relativePosition.x * 2 / maxDistance.x,
			relativePosition.y * 2 / maxDistance.y,
			relativePosition.z * 2 / maxDistance.z);
	}

	public static void PlaceRandomly(
		this Transform objectTransform,
		Transform groundTransform,
		LayerMask overlapLayerMask,
		float overlapRadius = 1.0f,
		float yPos = 0.0f)
	{
		Vector3 groundSize = groundTransform.GetComponent<Renderer>().bounds.size;
		Vector3 newPos;

		do
		{
			newPos = new Vector3(
				Random.Range(-groundSize.x / 2, groundSize.x / 2),
				yPos,
				Random.Range(-groundSize.z / 2, groundSize.z / 2)
			);
			newPos = groundTransform.position.RelativePosition(newPos);
			newPos.y = yPos;
			objectTransform.position = newPos;
			break;
		} while (Physics.CheckSphere(objectTransform.position, overlapRadius, overlapLayerMask));
	}
}