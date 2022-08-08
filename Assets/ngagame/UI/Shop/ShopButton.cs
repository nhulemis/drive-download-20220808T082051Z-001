using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
	[SerializeField] Animation noticeAnimation;

	private void OnEnable()
	{
		UpdateNotice(GameEvent.OnCoinChange, null);
		EventManager.Instance.AddListener(GameEvent.OnCoinChange, UpdateNotice);
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}
	}

	void UpdateNotice(GameEvent Event_Type, Component Sender, object Param = null)
	{
		if(noticeAnimation != null)
			noticeAnimation.gameObject.SetActive(ShopManager.Instance.HasNew());
	}

	public void OpenShop()
	{
		SceneMaster.Instance.ShowLoading(1.5f, true);
		SceneMaster.Instance.OpenScene(SceneID.Shop);
	}
}
