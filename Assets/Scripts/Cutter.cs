using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{
	[SerializeField] string calculation;
	[SerializeField] SkinnedMeshRenderer fx;
	Collider col;

	private void Start()
	{
		fx.gameObject.SetActive(false);
		col = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.tag.Equals(GameConst.Data.playertag))
		{
			return;
		}
		
		var player = other.gameObject.GetComponentInChildren<Player>();
		if (player == null) return;
		var playerBone = player.GetBone();
		fx.SetBlendShapeWeight(0, playerBone.GetBlendShapeWeight(0));
		fx.material = playerBone.sharedMaterial;
		var contact = other.ClosestPoint(player.Head.transform.position);
		contact.y = fx.transform.position.y;
		fx.transform.position = contact - fx.transform.forward * 0.5F;
		//fx.transform.localEulerAngles = fx.transform.localEulerAngles + Random.onUnitSphere * 20;
		fx.gameObject.SetActive(true);
		Debug.LogError("cut");
		player.CalculateTall(calculation);
		SoundManager.Instance.PlaySFX("tearing");
		col.enabled = false;
	}
}
