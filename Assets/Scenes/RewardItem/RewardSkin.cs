using System.Collections;
using System.Collections.Generic;
using ngagame.Manager.SceneManager;
using UnityEngine;
using UnityEngine.UI;

public class RewardSkin : Scene
{
    [SerializeField] Text globalGemText;
    [SerializeField] GameObject skinWraper;

    string skinInProgress;

    private void OnEnable()
    {
        EventManager.Instance.AddListener(GameEvent.OnCoinChange, OnCoinChange);
        OnCoinChange(GameEvent.OnCoinChange, this, null);

        skinInProgress = ShopManager.Instance.SkinInProgress(Profile.Instance.UnlockSkinIndex);
        var skinPrefab = ShopManager.Instance.GetSkinModel(skinInProgress);
        var skinModel = Instantiate(skinPrefab, skinWraper.transform);
        if (skinModel == null)
        {
            return;
        }

        ngagame.Utils.SetLayerRecursively(skinModel.gameObject, GameConstanst.UILayer);
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

    public void OnClaimClick()
    {
        ads_go.Instance.ShowRewarded((value) =>
        {
            if (value)
            {
                Runner.Instance.AddEventCallBack(() =>
                {
                    ShopManager.Instance.Unlock(skinInProgress.ToString(), true);
                    Close();
                });
            }
        });
    }

    public void OnSkipClick()
    {
        ads_go.Instance.ShowInterstitial();
        Close();
    }

    void Close()
    {
        SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
        SceneMaster.Instance.CloseScene(SceneID.RewardSkin);
        Profile.Instance.CreateNewSkinProgress();
    }
}