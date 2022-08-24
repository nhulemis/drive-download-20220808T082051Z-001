using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
      int old = Int32.Parse(coinText.text.Replace(",","").Replace(".",""));
		coinText.DOCounter( old,Profile.Instance.Coins, 2);
	}

	int previewNumber;
	void OnPreviewCoin(GameEvent Event_Type, Component Sender, object Param = null)
	{
		coinText.text = Param.ToString();
	}
}
