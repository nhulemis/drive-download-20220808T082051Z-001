using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalCoin : MonoBehaviour
{
	[SerializeField] Text coinText;

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnCoinChange, OnCoinChange);
		EventManager.Instance.AddListener(GameEvent.OnPreviewCoin, OnPreviewCoin);
		OnCoinChange(GameEvent.OnCoinChange, null);
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}
		eventManager.RemoveEvent(GameEvent.OnCoinChange, OnCoinChange);
		eventManager.RemoveEvent(GameEvent.OnPreviewCoin, OnPreviewCoin);
	}

	void OnCoinChange(GameEvent Event_Type, Component Sender, object Param = null)
	{
		coinText.text = Profile.Instance.Coins.ToString();
	}

	int previewNumber;
	void OnPreviewCoin(GameEvent Event_Type, Component Sender, object Param = null)
	{
		coinText.text = Param.ToString();
	}
}
