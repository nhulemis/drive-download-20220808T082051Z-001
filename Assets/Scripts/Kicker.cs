using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicker : MonoBehaviour
{
	Collider col;

	private void Start()
	{
		col = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.tag.Equals(GameConst.Data.playertag))
		{
			return;
		}
		col.enabled = false;
		var player = other.GetComponent<PlayerController>();
		if (player == null) return;
		player.Jump();
	}
}
