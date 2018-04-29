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
}