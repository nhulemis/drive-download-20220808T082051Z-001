using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoadCreator
{
	public class CurvePath : Path
	{
		public override void Create(Vector3[] points, float spacing, float resolution = 1)
		{
			this.points = points;
			this.spacing = spacing;
			this.resolution = resolution;
			CalculateEvenlySpacedPoints();
		}

		public Vector3 GetPointOnCurve(float t)
		{
			return Mathf.Pow(1 - t, 2) * points[0] + 2 * (1 - t) * t * points[1] + t * t * points[2];
		}

		Vector3 EvaluateCurveDerivative(float t)
		{
			return 2 * (1 - t) * (points[1] - points[0]) + 2 * t * (points[2] - points[1]);
		}
		
		private List<float> times = new List<float>(2);
		private Vector3 previousPoints;
		private float distanceSinceLastEvenPoint = 0;
		protected override void CalculateEvenlySpacedPoints()
		{
			if(points.Length < 3)
			{
				return;
			}
			evenlySpacedPoints = new List<Vector3>() { points[0] };
			tangents = new List<Vector3>() { EvaluateCurveDerivative(0) };
			normals.Add(Vector3.up);
			previousPoints = points[0];
			distanceSinceLastEvenPoint = 0;
			Vector3 centroid = new Vector3(
				points[0].x + points[1].x + points[2].x,
				points[0].y + points[1].y + points[2].y,
				points[0].z + points[1].z + points[2].z
				);
			float estimatedLength = Vector3.Distance(points[0], centroid) + Vector3.Distance(points[2], centroid);
			int divisions = Mathf.CeilToInt(estimatedLength * resolution * 10);

			for (int i = 1; i <= divisions; i++)
			{
				Vector3 pointOnCurve = GetPointOnCurve((float)i / divisions);
				distanceSinceLastEvenPoint += Vector3.Distance(previousPoints, pointOnCurve);
				while (distanceSinceLastEvenPoint >= spacing)
				{
					float overshootDistance = distanceSinceLastEvenPoint - spacing;
					Vector3 newEvenlySpacedPoint = pointOnCurve + (previousPoints - pointOnCurve).normalized * overshootDistance;
					evenlySpacedPoints.Add(newEvenlySpacedPoint);
					tangents.Add(EvaluateCurveDerivative((((float)i / divisions) + (overshootDistance / spacing))).normalized);
					distanceSinceLastEvenPoint = overshootDistance;
					previousPoints = newEvenlySpacedPoint;
				}
				previousPoints = pointOnCurve;
			}

			// Calc normals
			Vector3 lastRotationAxis = Vector3.up;
			for (int i = 0; i < evenlySpacedPoints.Count; i++)
			{
				if (i == 0)
				{
					normals.Add(Vector3.Cross(lastRotationAxis, tangents[0]).normalized);
				}
				else
				{
					// First reflection
					Vector3 offset = (evenlySpacedPoints[i] - evenlySpacedPoints[i - 1]);
					float sqrDst = offset.sqrMagnitude;
					Vector3 r = lastRotationAxis - offset * 2 / sqrDst * Vector3.Dot(offset, lastRotationAxis);
					Vector3 t = tangents[i - 1] - offset * 2 / sqrDst * Vector3.Dot(offset, tangents[i - 1]);

					// Second reflection
					Vector3 v2 = tangents[i] - t;
					float c2 = Vector3.Dot(v2, v2);

					Vector3 finalRot = r - v2 * 2 / c2 * Vector3.Dot(v2, r);
					Vector3 n = Vector3.Cross(finalRot, tangents[i]).normalized;
					normals.Add(n);
					lastRotationAxis = finalRot;
				}
			}
		}

		private float segmentLength;
		private float percentBetweenIndices;
		private int indexAtTime;
		//public override Vector3 GetPointAtTime(float t)
		//{
		//	segmentLength = 1f / evenlySpacedPoints.Count;
		//	indexAtTime = Mathf.CeilToInt(t / segmentLength);
		//	if (indexAtTime > evenlySpacedPoints.Count - 2)
		//	{
		//		return evenlySpacedPoints[evenlySpacedPoints.Count - 1];
		//	}
		//	percentBetweenIndices = (t - ((indexAtTime - 1) * segmentLength)) / segmentLength;
		//	return Vector3.Lerp(evenlySpacedPoints[indexAtTime], evenlySpacedPoints[indexAtTime + 1], percentBetweenIndices);
		//}

		//public override Vector3 GetPointAtTime(float t)
		//{
		//	int prevIndex = 0;
		//	int nextIndex = evenlySpacedPoints.Count - 1;
		//	int i = Mathf.RoundToInt(t * (evenlySpacedPoints.Count - 1)); // starting guess

		//	// Starts by looking at middle vertex and determines if t lies to the left or to the right of that vertex.
		//	// Continues dividing in half until closest surrounding vertices have been found.
		//	while (true)
		//	{
		//		// t lies to left
		//		if (t <= times[i])
		//		{
		//			nextIndex = i;
		//		}
		//		// t lies to right
		//		else
		//		{
		//			prevIndex = i;
		//		}
		//		i = (nextIndex + prevIndex) / 2;

		//		if (nextIndex - prevIndex <= 1)
		//		{
		//			break;
		//		}
		//	}

		//	float abPercent = Mathf.InverseLerp(times[prevIndex], times[nextIndex], t);
		//	//return new TimeOnPathData(prevIndex, nextIndex, abPercent);
		//	return Vector3.Lerp(evenlySpacedPoints[prevIndex], evenlySpacedPoints[nextIndex], abPercent);
		//}

		public override Vector3 GetPointAtTime(float t)
		{
			return GetPointOnCurve(t);
		}

		[SerializeField] private float time = 0;
		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(GetPointAtTime(Mathf.Clamp(time, 0f, 1f)), 0.2f);

			for (int i = 0; i < evenlySpacedPoints.Count; i++)
			{
				Gizmos.DrawSphere(evenlySpacedPoints[i], 0.1f);
			}
		}

		[SerializeField] GameObject go;
		private void Update()
		{
			time += Time.deltaTime * 0.5f;
			go.transform.position = GetPointAtTime(time);
			if (time > 1)
			{
				time = 0;
			}
		}
	}
}