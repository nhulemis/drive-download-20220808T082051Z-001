using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPart : MonoBehaviour
{
	[SerializeField] GameObject[] themes;

	public void ActiveTheme(EnvironmentTheme theme)
	{
		for(int i = 0; i < themes.Length; i++)
		{
			themes[i].gameObject.SetActive(i == (int)theme);
		}
	}
}

public enum EnvironmentTheme
{
	BlueCity,
	PurpleCity,
	Abstract,
	ConeAbstract,
	Water
}
