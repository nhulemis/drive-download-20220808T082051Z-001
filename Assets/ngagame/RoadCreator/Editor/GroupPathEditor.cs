using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RoadCreator
{
	[CustomEditor(typeof(SolidRoad))]
	[CanEditMultipleObjects]
	public class GroupPathEditor : Editor
	{
		SolidRoad groupPath;

		private void OnSceneGUI()
		{
			Draw();
		}

		void Draw()
		{
			Handles.color = Color.yellow;
			for (int i = 0; i < groupPath.points.Length; i++)
			{
				Vector3 newPos = Handles.DoPositionHandle(groupPath.points[i], Quaternion.identity);
				if (newPos != groupPath.points[i])
				{
					Undo.RecordObject(groupPath, "Move point");
					groupPath.points[i] = newPos;
					groupPath.CreateRoad();
					groupPath.RefreshElements();
				}
			}
		}


		public override void OnInspectorGUI()
		{
			this.DrawDefaultInspector();
			EditorGUILayout.LabelField("Length: ", groupPath.Length.ToString());
			//this.Repaint();
		}

		private void OnEnable()
		{
			groupPath = (SolidRoad)target;
			if(groupPath == null)
			{
				return;
			}
			groupPath.InitEditor();
		}
	}
}