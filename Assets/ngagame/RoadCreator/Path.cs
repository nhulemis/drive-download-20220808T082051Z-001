using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoadCreator
{
	public abstract class Path
	{
		protected float spacing = 0.2f; 
		protected float resolution = 1;
		protected List<Vector3> evenlySpacedPoints = new List<Vector3>(2);
		protected List<Vector3> tangents = new List<Vector3>(2);
		protected List<Vector3> normals = new List<Vector3>(2);
		public Vector3[] points = new Vector3[2];
		public abstract Vector3 GetPointAtTime(float t);
		protected abstract void CalculateEvenlySpacedPoints();
		public abstract void Create(Vector3[] points, float spacing, float resolution = 1);

		public List<Vector3> EvenlyPoints => evenlySpacedPoints;
		public List<Vector3> Tangents => tangents;
		public List<Vector3> Normals => normals;
	}
}