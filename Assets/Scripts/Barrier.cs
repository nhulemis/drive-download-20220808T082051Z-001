using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
	[SerializeField] string calculation;
	[SerializeField] Transform leftDirection;
	[SerializeField] Transform rightDirection;
	[SerializeField] float force = 100;
	Rigidbody rb;
	Collider col;
	bool active = true;
	static bool pushLeft = true;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!active || !other.gameObject.tag.Equals(GameConst.Data.playertag))
		{
			return;
		}
		col.isTrigger = false;
		
		if (rb != null)
		{
			rb.useGravity = true;
			rb.AddForceAtPosition(
				force * (pushLeft ? leftDirection.forward : rightDirection.forward),
				transform.position + Random.onUnitSphere);
			gameObject.layer = GameConst.Data.brokenLayer;
		}
		pushLeft = !pushLeft;
		active = false;
		var player = other.gameObject.GetComponentInChildren<Player>();
		if (player == null) return;
		if(player.Weight > player.Tall)
		{
			player.CalculateWeight(calculation);
		}
		 else
		{
			player.CalculateTall(calculation);
		}
		SoundManager.Instance.PlaySFX("punch");
	}
}
