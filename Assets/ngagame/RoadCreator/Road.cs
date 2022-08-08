using RoadCreator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Road : MonoBehaviour
{
	[SerializeField] RoadPart[] roadParts;
	[SerializeField] EnvironmentTheme theme = EnvironmentTheme.PurpleCity;
	public float Length { private set; get; } = 0f;

	private void Start()
	{
		Init();
	}

	EnvironmentPart[] environmentParts;
	public void Init()
	{
		Length = 0;
		roadParts = GetComponentsInChildren<RoadPart>();
		for (int i = 0; i < roadParts.Length; i++)
		{
			roadParts[i].Init();
			Length += roadParts[i].Length;
		}

		RefreshEnvironment();
		for (int i = 0; i < roadParts.Length; i++)
		{
			roadParts[i].SetActiveEnvironment(i < 2);
		}
	}

	public void RefreshEnvironment()
	{
		environmentParts = GetComponentsInChildren<EnvironmentPart>(true);
		for(int i = 0; i < environmentParts.Length; i++)
		{
			environmentParts[i].ActiveTheme(theme);
		}
	}

	public void ValidateLevelPart()
	{
		roadParts = GetComponentsInChildren<RoadPart>();
		for (int i = 0; i < roadParts.Length; i++)
		{
			if (i < roadParts.Length - 1)
			{
				roadParts[i].Next = roadParts[i + 1];
			}
			if (i > 0)
			{
				roadParts[i].Backward = roadParts[i - 1];
				var dir = roadParts[i].transform.position - roadParts[i - 1].EndPoint;
				roadParts[i].transform.rotation = Quaternion.LookRotation(roadParts[i - 1].OutDirection, Vector3.up);
				roadParts[i].transform.position = roadParts[i - 1].EndPoint;
			}
		}
		RefreshEnvironment();
	}

	public void RandomTheme()
	{
		try
		{
			var themes = System.Enum.GetValues(typeof(EnvironmentTheme));
			theme = (EnvironmentTheme)Random.Range(0, themes.Length - 1);
		} catch { }
	}

#if UNITY_EDITOR
	[ContextMenu("Manual Init")]
	public void ManualInit()
	{
		ValidateLevelPart();
	}

	private void OnValidate()
	{
		if (!isActiveAndEnabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			return;
		}

		ValidateLevelPart();
	}
#endif
}
