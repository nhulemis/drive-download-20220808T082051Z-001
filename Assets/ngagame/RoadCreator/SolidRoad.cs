using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoadCreator
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class SolidRoad : RoadPart
	{
		private const float RESOLUTION = 10f;
		private const float SPACING = 0.5f;

		[SerializeField]
		[Range(0.0f, 0.5f)]
		private float smooth = 0.2f;
		public Vector3[] points;
		public override Vector3 EndPoint => transform.TransformPoint(points[points.Length - 1]);
		public override bool IsAirLine => false;
		[SerializeField] float thickness = 5;

		private List<Path> paths = new List<Path>(2);
		private float length = 0;
		private List<Vector3> evenlySpacedPoints = new List<Vector3>(2);
		private List<Vector3> tangents = new List<Vector3>(2);
		private List<Vector3> normals = new List<Vector3>(2);
		private Vector3 leftPoint, rightPoint;
		private bool startPoint = true;
		private bool endPoint = false;
		private Vector3 currentDirection;
		private MeshFilter meshFilter;
		private Element[] elements = null;

		public bool Initialized { private set; get; } = false;

		private void Awake()
		{
			meshFilter = GetComponent<MeshFilter>();
		}

		public override float Length => length;

		public override Vector3 OutDirection =>
			points.Length >= 2 ?
			(transform.TransformDirection(points[points.Length - 1]) - transform.TransformDirection(points[points.Length - 2])).normalized :
			Vector3.forward;

		public void CalculateEvenlySpacedPoints()
		{
			if (points.Length < 2)
			{
				return;
			}
			evenlySpacedPoints = new List<Vector3>(points.Length);
			tangents = new List<Vector3>(points.Length);
			normals = new List<Vector3>(points.Length);

			for (int i = 0; i < points.Length - 1; i++)
			{
				startPoint = i == 0;
				endPoint = i == points.Length - 2;
				rightPoint = Vector3.Lerp(points[i], points[i + 1], smooth);

				if (!startPoint)
				{
					//leftPoint = Vector3.Lerp(points[i], points[i - 1], turnCurvePercent);
					leftPoint = evenlySpacedPoints[evenlySpacedPoints.Count - 1] + currentDirection * SPACING;
					// Create curve
					CurvePath curvePath = new CurvePath();
					curvePath.Create(new Vector3[] { leftPoint, points[i], rightPoint }, SPACING, RESOLUTION);
					evenlySpacedPoints.AddRange(curvePath.EvenlyPoints);
					tangents.AddRange(curvePath.Tangents);
					paths.Add(curvePath);
					if (evenlySpacedPoints.Count > 1)
					{
						currentDirection = (evenlySpacedPoints[evenlySpacedPoints.Count - 1] - evenlySpacedPoints[evenlySpacedPoints.Count - 2]).normalized;
					}
				}

				// Create right linear path
				LinearPath rightPath = new LinearPath();
				rightPath.Create(new Vector3[] {
					startPoint ? points[i] : evenlySpacedPoints[evenlySpacedPoints.Count - 1] + currentDirection * SPACING,
					endPoint ? points[i + 1] : Vector3.Lerp(points[i + 1], points[i], smooth)
				},
				SPACING,
				RESOLUTION
				);

				evenlySpacedPoints.AddRange(rightPath.EvenlyPoints);
				tangents.AddRange(rightPath.Tangents);
				paths.Add(rightPath);
				if (evenlySpacedPoints.Count > 1)
				{
					currentDirection = (evenlySpacedPoints[evenlySpacedPoints.Count - 1] - evenlySpacedPoints[evenlySpacedPoints.Count - 2]).normalized;
				}
			}
			length = evenlySpacedPoints.Count * SPACING;
		}

		public void CreateRoad()
		{
			CalculateEvenlySpacedPoints();
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			meshFilter.sharedMesh = CreateRoadMesh();
		}

		private float segmentLength;
		private float percentBetweenIndices;
		private int indexAtTime;

		public override Vector3 GetPointAtTime(float t)
		{
			if(evenlySpacedPoints.Count <= 0)
			{
				return Vector3.zero;
			}
			if (t == 1)
			{
				return evenlySpacedPoints[evenlySpacedPoints.Count - 1];
			}
			segmentLength = 1f / (evenlySpacedPoints.Count - 1);
			indexAtTime = Mathf.FloorToInt(t / segmentLength);
			percentBetweenIndices = (t - (indexAtTime) * segmentLength) / segmentLength;
			return transform.TransformPoint(Vector3.Lerp(evenlySpacedPoints[indexAtTime], evenlySpacedPoints[indexAtTime + 1], percentBetweenIndices));
		}

		private Vector3 _dirStart;
		private Vector3 _dirEnd;
		public override Quaternion GetDirectionAtTime(float t)
		{
			segmentLength = 1f / tangents.Count;
			indexAtTime = Mathf.CeilToInt(t / segmentLength);
			if (indexAtTime > tangents.Count - 2)
			{
				indexAtTime = tangents.Count - 2;
			}
			percentBetweenIndices = (t - ((indexAtTime - 1) * segmentLength)) / segmentLength;
			Vector3 tangent = Vector3.Lerp(tangents[indexAtTime], tangents[indexAtTime + 1], percentBetweenIndices);
			Vector3 normal = Vector3.Lerp(normals[indexAtTime], normals[indexAtTime + 1], percentBetweenIndices);
			return Quaternion.LookRotation(transform.TransformDirection(tangent), transform.TransformDirection(normal));
		}

		private Mesh CreateRoadMesh()
		{
			Vector3[] verts = new Vector3[evenlySpacedPoints.Count * 8 + 4]; // 4 vert o front face
			Vector2[] uvs = new Vector2[verts.Length];
			Vector3[] normals = new Vector3[verts.Length];

			int numTris = 2 * (evenlySpacedPoints.Count - 1);
			int[] roadTriangles = new int[numTris * 3 + 6]; // 6 tris of front face
															//int[] underRoadTriangles = new int[numTris * 3];
			int[] sideOfRoadTriangles = new int[numTris * 2 * 3 + 6];

			int vertIndex = 0;
			int triIndex = 0;

			// Vertices for the top of the road are layed out:
			// 0  1
			// 8  9
			// and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
			int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
			int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

			for (int i = 0; i < evenlySpacedPoints.Count; i++)
			{
				Vector3 forward = Vector3.zero;
				if (i < evenlySpacedPoints.Count - 1)
				{
					forward += evenlySpacedPoints[(i + 1) % evenlySpacedPoints.Count] - evenlySpacedPoints[i];
				}
				if (i > 0)
				{
					forward += evenlySpacedPoints[i] - evenlySpacedPoints[(i - 1 + evenlySpacedPoints.Count) % evenlySpacedPoints.Count];
				}

				forward.Normalize();

				Vector3 left = Vector3.Cross(forward, Vector3.up).normalized;
				Vector3 normal = -Vector3.Cross(forward, left).normalized;
				this.normals.Add(normal);
				Vector3 localUp = normal;
				Vector3 localRight = -left;

				// Find position to left and right of current path vertex
				Vector3 vertSideA = evenlySpacedPoints[i] - localRight * Mathf.Abs(Width);
				Vector3 vertSideB = evenlySpacedPoints[i] + localRight * Mathf.Abs(Width);

				// Add top of road vertices
				verts[vertIndex + 0] = vertSideA;
				verts[vertIndex + 1] = vertSideB;
				// Add bottom of road vertices
				verts[vertIndex + 2] = vertSideA - localUp * thickness;
				verts[vertIndex + 3] = vertSideB - localUp * thickness;

				// Duplicate vertices to get flat shading for sides of road
				verts[vertIndex + 4] = verts[vertIndex + 0];
				verts[vertIndex + 5] = verts[vertIndex + 1];
				verts[vertIndex + 6] = verts[vertIndex + 2];
				verts[vertIndex + 7] = verts[vertIndex + 3];

				// Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
				uvs[vertIndex + 0] = new Vector2(0, (float)i / evenlySpacedPoints.Count);
				uvs[vertIndex + 1] = new Vector2(1, (float)i / evenlySpacedPoints.Count);

				// Top of road normals
				normals[vertIndex + 0] = localUp;
				normals[vertIndex + 1] = localUp;
				// Bottom of road normals
				normals[vertIndex + 2] = -localUp;
				normals[vertIndex + 3] = -localUp;
				// Sides of road normals
				normals[vertIndex + 4] = -localRight;
				normals[vertIndex + 5] = localRight;
				normals[vertIndex + 6] = -localRight;
				normals[vertIndex + 7] = localRight;

				// Set triangle indices
				if (i < evenlySpacedPoints.Count - 1)
				{
					for (int j = 0; j < triangleMap.Length; j++)
					{
						roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
						// reverse triangle map for under road so that triangles wind the other way and are visible from underneath
						//underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
					}
					// 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5
					for (int j = 0; j < sidesTriangleMap.Length; j++)
					{
						sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
					}

				}

				vertIndex += 8;
				triIndex += 6;
			}

			// Add front face
			int[] frontFaceTriangles = new int[6];
			verts[vertIndex + 0] = verts[0];
			verts[vertIndex + 1] = verts[1];
			verts[vertIndex + 2] = verts[2];
			verts[vertIndex + 3] = verts[3];
			normals[vertIndex + 0] = (verts[8] - verts[0]).normalized;
			normals[vertIndex + 1] = normals[vertIndex + 0];
			normals[vertIndex + 2] = normals[vertIndex + 0];
			normals[vertIndex + 3] = normals[vertIndex + 0];
			roadTriangles[triIndex - 6] = vertIndex + 0;
			roadTriangles[triIndex - 5] = vertIndex + 1;
			roadTriangles[triIndex - 4] = vertIndex + 2;
			roadTriangles[triIndex - 3] = vertIndex + 1;
			roadTriangles[triIndex - 2] = vertIndex + 3;
			roadTriangles[triIndex + -1] = vertIndex + 2;


			Mesh mesh = new Mesh
			{
				vertices = verts,
				uv = uvs,
				normals = normals,
				subMeshCount = 3
			};
			mesh.SetTriangles(roadTriangles, 0);
			mesh.SetTriangles(sideOfRoadTriangles, 1);
			mesh.SetTriangles(frontFaceTriangles, 2);
			mesh.RecalculateBounds();
			return mesh;
		}

		
		public override void Init()
		{
			CreateRoad();
			RefreshElements();
			for (int i = 0; i < elements.Length; i++)
			{
				elements[i].Init();
			}
			Initialized = true;
		}

		[ContextMenu("Manual Init")]
		public void InitEditor()
		{
			CreateRoad();
			Initialized = true;
			RefreshElements();
		}

		public void RefreshElements()
		{
			if (Length == 0)
			{
				return;
			}

			var groupElements = GetComponentsInChildren<GroupElement>();
			for (int i = 0; i < groupElements.Length; i++)
			{
				groupElements[i].Init();
			} 

			elements = GetComponentsInChildren<Element>();
			for (int i = 0; i < elements.Length; i++)
			{
				if(elements[i].Position >= 0)
				{
					float percent = Mathf.Clamp01(elements[i].Position / Length);
					elements[i].transform.position = GetPointAtTime(percent);
					elements[i].transform.rotation = GetDirectionAtTime(percent);
				}
			}

			
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if(!isActiveAndEnabled)
			{
				return;
			}
			InitEditor();
		}
#endif
	}
}