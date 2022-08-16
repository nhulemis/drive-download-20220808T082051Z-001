using System.Collections.Generic;
using System.Linq;
using ngagame.Manager.SceneManager;
using UnityEngine;
using UnityEngine.UI;

public class ShopScene : Scene
{
	ShopItemView[] itemViews;
	[SerializeField] int rewardCoinAmount = 500;
	[SerializeField] Button unlockButton;
	[SerializeField] Button getCoinButton;
	[SerializeField] Text priceText;
	[SerializeField] Transform previewTransform;

	List<string> playerSkins = new List<string>(9);
	Dictionary<string, GameObject> previewModels = new Dictionary<string, GameObject>();

	private void OnEnable()
	{
		itemViews = GetComponentsInChildren<ShopItemView>();

		playerSkins = ShopManager.Instance.Data.skinList.ToList();
		for (int i = 0; i < Mathf.Min(playerSkins.Count, itemViews.Length); i++)
		{
			var skin = ShopManager.Instance.GetSkinModel(playerSkins[i]);
			var model = skin != null ? Instantiate(skin, previewTransform) : null;

			if (model != null)
			{
				model.gameObject.name = playerSkins[i].ToString();
				previewModels.Add(playerSkins[i].ToString(), model.gameObject);
			}
		}
		ngagame.Utils.SetLayerRecursively(previewTransform.gameObject, GameConstanst.UILayer);

		for (int i = 0; i < Mathf.Min(itemViews.Length, previewTransform.childCount); i++)
		{
			itemViews[i].Init(previewTransform.GetChild(i).gameObject.name);
			itemViews[i].OnSelect = OnItemSelected;
			if(itemViews[i].Id == ShopManager.Instance.CurrentSkin)
			{
				ShowPreview(itemViews[i].Id);
			}
		}

		priceText.text = ShopManager.UNLOCK_RANDOM_PRICE.ToString();
		RefreshUI(GameEvent.OnCoinChange, null);
		InitScroll();
		EventManager.Instance.AddListener(GameEvent.OnCoinChange, RefreshUI);
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if(eventManager != null)
		{
			eventManager?.RemoveEvent(GameEvent.OnCoinChange, RefreshUI);
		}
	}

	void OnItemSelected(string id)
	{
		for (int i = 0; i < itemViews.Length; i++)
		{
			itemViews[i].Refresh();
			if(ShopManager.Instance.CurrentSkin == itemViews[i].Id)
			{
				ShowPreview(id);
			}
		}
	}

	void ShowPreview(string id)
	{
		foreach (var preview in previewModels)
		{
			preview.Value.gameObject.SetActive(false);
		}
		if (previewModels.ContainsKey(id))
		{
			previewModels[id].gameObject.SetActive(true);
		}
	}

	public void UnlockRandom()
	{
		if(Profile.Instance.Coins < ShopManager.UNLOCK_RANDOM_PRICE)
		{
			return;
		}
		List<string> unlockList = new List<string>();
		for (int i = 0; i < ShopManager.Instance.Data.skinList.Length; i++)
		{
			var item = ShopManager.Instance.Data.skinList[i].ToString();
			if (!ShopManager.Instance.Owned(item))
			{
				unlockList.Add(item);
			}
		}
		if(unlockList.Count <= 0)
		{
			return;
		}
		var unlockItem = unlockList[Random.Range(0, unlockList.Count)];
		
		foreach( var itemView in itemViews)
		{
			if(itemView.Id == unlockItem)
			{
				Profile.Instance.Coins -= ShopManager.UNLOCK_RANDOM_PRICE;
				itemView.Unlock();
			}
		}

		RefreshUI(GameEvent.OnCoinChange, null);
	}

	public void GetCoin()
	{
		ads_go.Instance.ShowRewarded((value) =>
		{
			if(value)
			{
				Profile.Instance.Coins += rewardCoinAmount;
			}
		});
	}

	public void Close()
	{
		SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		SceneMaster.Instance.CloseScene(SceneID.Shop);
	}

	void RefreshUI(GameEvent Event_Type, Component Sender, object Param = null)
	{
		unlockButton.interactable = Profile.Instance.Coins >= ShopManager.UNLOCK_RANDOM_PRICE;

		// Auto scroll
		Canvas.ForceUpdateCanvases();

		foreach (var item in itemViews)
		{
			if (item.IsSelected())
			{
				float page = Mathf.Round(scroll.content.transform.InverseTransformPoint(item.transform.position).x / scroll.content.sizeDelta.x);

				scroll.horizontalNormalizedPosition = Mathf.Clamp01(page);
				break;
			}
		}
	}

	#region Scrollview

	[SerializeField] ScrollRect scroll;
	[SerializeField] GameObject[] dots;
	[SerializeField] GameObject[] navs;
	[SerializeField] float pageWidth = 1080;
	float contentWidth, interval;
	int pageCount = 1;

	void InitScroll()
	{
		scroll.onValueChanged.AddListener(OnValueChange);
		contentWidth = scroll.content.sizeDelta.x;
		pageCount = scroll.content.childCount;
		interval = 1f / (pageCount - 1);
		OnValueChange(scroll.normalizedPosition);
	}

	void OnValueChange(Vector2 pos)
	{

	}

	public bool IsPressed()
	{
		return Input.GetMouseButton(0);
	}

	public bool WasReleased()
	{
		return Input.GetMouseButtonUp(0);
	}

	[SerializeField] float snapVelocity = 50;
	[SerializeField] float snapForce = 50;
	private void Update()
	{
		if(IsPressed() || scroll.velocity.magnitude > snapVelocity)
		{
			return;
		}

		if(WasReleased())
		{
			scroll.velocity = Vector2.zero;
		}
		
		scroll.horizontalNormalizedPosition = Mathf.Lerp(
		scroll.horizontalNormalizedPosition,
		Mathf.Round(scroll.horizontalNormalizedPosition / interval) * interval,
		snapForce * Time.deltaTime
		);

		int page = Mathf.RoundToInt(scroll.horizontalNormalizedPosition * (pageCount - 1));
		for (int i = 0; i < dots.Length; i++)
		{
			dots[i].gameObject.SetActive(i == page);
		}
		navs[0].gameObject.SetActive(scroll.horizontalNormalizedPosition > 1f / pageCount);
		navs[1].gameObject.SetActive(scroll.horizontalNormalizedPosition < 1 - (1f / pageCount));
	}

	#endregion
}
