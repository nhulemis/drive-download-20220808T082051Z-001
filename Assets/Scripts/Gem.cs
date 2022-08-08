using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
	public static int Earned = 0;
	[SerializeField] GameObject model;
	[SerializeField] GameObject fx;
	Collider col;

	private void Start()
	{
		fx.gameObject.SetActive(false);
		col = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.tag.Equals(GameConst.Data.playertag))
		{
			return;
		}
		Earned += 5;
		col.enabled = false;
		model.gameObject.SetActive(false);
		fx.gameObject.SetActive(true);
		SoundManager.Instance.PlaySFX("collect");
	}
}
