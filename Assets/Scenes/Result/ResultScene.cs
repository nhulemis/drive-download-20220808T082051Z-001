using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ngagame;
using ngagame.Manager.SceneManager;

public class ResultScene : Scene
{
	[SerializeField] Text gemText;
	[SerializeField] Text globalGemText;
	[SerializeField] Text xGemText;
	[SerializeField] GameObject winGroup;
	[SerializeField] GameObject loseGroup;
	[SerializeField] Button gemButton;
	[SerializeField] Button xGemButton;
	[SerializeField] Text percentText;
	[SerializeField] GameObject gemFX;
	[SerializeField] CollectAnimation collectAnimation;
	[SerializeField] Image fillSkin;

	const int X_GEM = 5;
	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnCoinChange, OnCoinChange);
		OnCoinChange(GameEvent.OnCoinChange, this, null);

		bool win = FinishLine.Finished;
		gemText.text = Gem.Earned.ToString();
		xGemText.text = (Gem.Earned * X_GEM).ToString();

		winGroup.gameObject.SetActive(win);
		loseGroup.gameObject.SetActive(!win);

		gemButton.transform.localScale = Vector3.zero;
		gemButton.transform.DOScale(Vector3.one * 1.0f, 0.2f).SetEase(Ease.OutBack).SetDelay(2f);

		var currentLevel = Profile.Instance.Level;
		var skinInProgress = ShopManager.Instance.SkinInProgress(Profile.Instance.UnlockSkinIndex);
		var preview = ShopManager.Instance.GetSkinPreview(skinInProgress);
		

		try
		{
			if (preview != null)
			{
				fillSkin.sprite = preview;
				fillSkin.transform.parent.GetComponent<Image>().sprite = preview;
			}
		} catch { };
			
		var startPercent = Profile.Instance.SkinProgress;
		Profile.Instance.SkinProgress += 1f / 6;
		Profile.Instance.SkinProgress = Mathf.Min(Profile.Instance.SkinProgress, 1f);

		DOVirtual.Float(startPercent, Profile.Instance.SkinProgress, 1.5f, (value) =>
		{
			fillSkin.fillAmount = value;
			percentText.text = (Mathf.RoundToInt(value * 100)) + "%";
		}).OnComplete(delegate
		{
			if(Profile.Instance.SkinProgress >= 1)
			{
				percentText.text = ShopManager.Instance.IsFullSkin ? "COMING SOON" : "NEW SKIN";
			}
		});
		gemFX.gameObject.SetActive(false);

		SoundManager.Instance.PlaySFX(win ? "win" : "lose", 1);

		if(win)
		{
			Profile.Instance.Level++;
		}
	}

	public void OnDisable()
	{
		if (EventManager.Instance != null)
		{
			EventManager.Instance.RemoveEvent(GameEvent.OnCoinChange, OnCoinChange);
		}
	}

	void OnCoinChange(GameEvent Event_Type, Component Sender, object Param = null)
	{
		globalGemText.text = Profile.Instance.Coins.ToString();
	}

	public void OnCloseClicked()
	{
		gemFX.gameObject.SetActive(true);
		gemButton.interactable = false;
		xGemButton.interactable = false;

		DOVirtual.DelayedCall(0.5f, delegate
		{
			ads_go.Instance.ShowInterstitial();
			Profile.Instance.Coins += Gem.Earned;
			Close();
		});
	}

	public void OnReplayClicked()
	{
		gemButton.interactable = false;
		xGemButton.interactable = false;
		ads_go.Instance.ShowInterstitial();
		Profile.Instance.Coins += Gem.Earned;
		SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		SceneMaster.Instance.CloseScene(SceneID.Result);
	}

	public void OnXGemClicked()
	{
		ads_go.Instance.ShowRewarded((value) =>
		{
			if(!value)
			{
				return;
			}
			
			Runner.Instance.AddEventCallBack(() =>
      {
        xGemButton.interactable = false;
        gemButton.interactable = false;
        var earnedGem = Gem.Earned * X_GEM;
        collectAnimation.Collect(
          delegate
          {
            Profile.Instance.Coins += earnedGem;
            Close();
          },
          (percent) =>
          {
            globalGemText.text = Mathf.CeilToInt(Mathf.Lerp(Profile.Instance.Coins, Profile.Instance.Coins + earnedGem, percent)).ToString();
          });
      });
		});
	}

	public void OnSkipLevelClicked()
	{
		ads_go.Instance.ShowRewarded((value) =>
		{
			if (!value)
			{
				return;
			}
			Runner.Instance.AddEventCallBack(() =>
      {
        Profile.Instance.Level++;
        Close();
      });
		});
	}

	void Close()
	{
		if (Profile.Instance.SkinProgress >= 1 && !ShopManager.Instance.IsFullSkin)
		{
			SceneMaster.Instance.OpenScene(SceneID.RewardSkin);
		} else
		{
			SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		}
		
		SceneMaster.Instance.CloseScene(SceneID.Result);
	}
}
