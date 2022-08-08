using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
	[SerializeField] string calculationTall;
	[SerializeField] string calculationWeight;
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
		col.enabled = false;
		model.gameObject.SetActive(false);
		fx.gameObject.SetActive(true);
		var player = other.GetComponentInChildren<Player>();
		if (player == null) return;
		player.CalculateWeight(calculationWeight);
		player.CalculateTall(calculationTall);
		SoundManager.Instance.PlaySFX("hit-03");
	}
}
