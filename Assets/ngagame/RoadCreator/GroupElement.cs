using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadCreator;

public class GroupElement : Element
{
    public override void Init()
    {
		var elements = GetComponentsInChildren<Element>();
		for (int i = 0; i < elements.Length; i++)
		{
			elements[i].UpdatePosition();
		}
	}

    public override void OnHit(GameObject collider)
    {

    }

	public float GetLength()
	{
		var childs = GetComponentsInChildren<Element>();
		return childs.Length > 0 ? childs[childs.Length - 1].Position : 0;
	}
}
