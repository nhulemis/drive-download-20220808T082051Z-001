using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
	[SerializeField] GameObject ownedObject;
	[SerializeField] GameObject lockedObject;
	[SerializeField] GameObject selectedObject;
	[SerializeField] Image icon;

	public string Id
	{
		get;
		private set;
	}

	public Action<string> OnSelect;

	private void OnEnable()
	{

	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}
	}

	public void Init(string _id)
	{
		Id = _id;
		icon.sprite = ShopManager.Instance.GetSkinPreview(Id);
		Refresh();
	}

	bool IsOwned()
	{
		return ShopManager.Instance.Owned(Id);
	}

	public bool IsSelected()
	{
		return ShopManager.Instance.CurrentSkin == Id;
	}

	public void Unlock()
	{
		ShopManager.Instance.Unlock(Id, true);
		ShopManager.Instance.CurrentSkin = Id;
		OnSelect?.Invoke(Id);
	}

	public void OnClick()
	{
		if(IsOwned()  && lockedObject.activeSelf == false)
		{
			ShopManager.Instance.CurrentSkin = Id;
			ShopManager.Instance.CurrentSkin = Id;
			OnSelect?.Invoke(Id);
		} 
    
	}

	public void Refresh()
	{
		var showPreview = IsOwned();
		ownedObject.gameObject.SetActive(showPreview);
		lockedObject.gameObject.SetActive(!showPreview);
		selectedObject.gameObject.SetActive(ShopManager.Instance.CurrentSkin == Id);
	}
}
