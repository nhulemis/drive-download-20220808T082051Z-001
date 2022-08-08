using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoadCreator
{
	[CustomEditor(typeof(Road))]
	[CanEditMultipleObjects]
	public class RoadEditor : Editor
	{
		private void OnEnable()
		{
			var level = (Road)target;
			var groupPaths = level.GetComponentsInChildren<SolidRoad>();
			foreach(var gp in groupPaths)
			{
				gp.InitEditor();
			}
		}
	}
}
