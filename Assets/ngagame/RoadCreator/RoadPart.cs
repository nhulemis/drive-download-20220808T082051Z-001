using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoadPart : MonoBehaviour
{
	public float Width = 3;
	public RoadPart Backward;
	public RoadPart Next;

	public abstract void Init();
	public abstract Vector3 GetPointAtTime(float t);
	public abstract Quaternion GetDirectionAtTime(float t);
	public abstract float Length { get; }
	public abstract Vector3 EndPoint { get; }
	public abstract Vector3 OutDirection { get; }
	public abstract bool IsAirLine { get; }

	public virtual void Enter()
	{

	}

	public virtual void Exit()
	{

	}

	public void SetActiveEnvironment(bool _active)
	{
		var environmentParts = GetComponentsInChildren<EnvironmentPart>(true);
		foreach (var e in environmentParts)
		{
			e.gameObject.SetActive(_active);
		}
	}
}
