using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoadCreator
{
	public class LinearPath : Path
	{
		public override void Create(Vector3[] points, float spacing, float resolution = 1)
		{
			this.points = points;
			this.spacing = spacing;
			this.resolution = resolution;
			CalculateEvenlySpacedPoints();
		}

		public override Vector3 GetPointAtTime(float t)
		{
			return points[0] + t * (points[1] - points[0]);
		}

		protected override void CalculateEvenlySpacedPoints()
		{
			evenlySpacedPoints = new List<Vector3>() { points[0] };
			tangents.Add((points[1] - points[0]).normalized);
			Vector3 left = Vector3.Cross(tangents[tangents.Count - 1], Vector3.up).normalized;
			Vector3 normal = -Vector3.Cross(tangents[tangents.Count - 1], left).normalized;
			normals.Add(Vector3.up);
			int divisions = Mathf.CeilToInt(Vector3.Distance(points[0], points[1]) / spacing);
			for(int i = 1; i <= divisions; i++)
			{
				evenlySpacedPoints.Add(Vector3.Lerp(points[0], points[1], (float)i / divisions));
				tangents.Add((points[1] - points[0]).normalized);
				left = Vector3.Cross(tangents[tangents.Count - 1], Vector3.up).normalized;
				normal = -Vector3.Cross(tangents[tangents.Count - 1], left).normalized;
				normals.Add(normal);
			}
		}
	}
}