using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineController : MonoBehaviour
{
	[SerializeField] Vector3 start;
	[SerializeField] Vector3 end;
	[Range(0, 1f)]
	public float tall;



	private void LateUpdate()
	{
		transform.localPosition = Vector3.LerpUnclamped(start, end, tall);
	}
}
