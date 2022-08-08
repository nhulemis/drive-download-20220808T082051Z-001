using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
	public Player Player { get; private set; }

	private void Awake()
	{
		Player = GetComponentInChildren<Player>();
		if(Player.name == ShopManager.Instance.CurrentSkin)
		{
			return;
		}

		Player.gameObject.SetActive(false);
		var prefab = ShopManager.Instance.GetSkinModel(ShopManager.Instance.CurrentSkin);
		if(prefab != null)
		{
			var ins = Instantiate(prefab, transform);
			Player = ins.GetComponent<Player>();
		}
	}

	public void Idle()
	{
		if (Player.Dead) return;
		Player.Animator.SetTrigger("idle");
	}

	public void Run()
	{
		if (Player.Dead) return;
		Player.Animator.SetTrigger("run");
	}

	public void Jump()
	{
		if (Player.Dead) return;
		Player.Animator.SetTrigger("jump");
	}
}
