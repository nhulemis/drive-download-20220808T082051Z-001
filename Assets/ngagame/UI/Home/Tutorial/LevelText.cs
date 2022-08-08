using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
	[SerializeField] Text levelText;

	private void OnEnable()
	{
		levelText.text = $"Level {Profile.Instance.Level}";
	}
}
