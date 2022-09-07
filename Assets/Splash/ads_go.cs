using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class ads_go : MonoBehaviour
{
    public static ads_go Instance;

    [ReadOnly]
    public string maxSDKKey = "DpnBL7fWuMl3Nosbg-cTaQH2dulNafTa_pg7xL4tz7z6DnC1RgfhWQFATzZ-AOqRfTEqvNemGvfH_SoiTHQaal";
    [Space] private int coutLoseGame = 0;

    public int timesLoseGameToShowInterAds = 5;
    public string maxAdInterId = "77fecb0549079116";
    public string maxAdInterIOS = "91dbea6644760da9";

    [Space] public bool isShowBanner;

    public float threshold = 3;
    public string maxAdBannerId = "6630b4634b55e6a7";
    public string maxAdBannerIOS = "0a692aa5cb5aa5df";

    [Space] public string maxAdRewardId = "b5ca2de9cc42441b";
    public string maxAdRewardIOS = "17dfb961a8d14eec";

    void checkOtherADS()
    {
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        MaxSdk.SetSdkKey(maxSDKKey);
        MaxSdk.InitializeSdk();

        StartCoroutine(AutoShowBanner());

        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardAdReceived;
        MaxSdkCallbacks.OnSdkInitializedEvent += OnAdInitSuccess;
    }

    private void OnAdInitSuccess(MaxSdkBase.SdkConfiguration obj)
    {
        IsVideoRewardReady();
    }

    private void OnDestroy()
    {
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardAdReceived;
    }


    #region Inter Ads

    [Button]
    public void ShowInterstitial()
    {
        Debug.Log("ShowInter");
        coutLoseGame++;
        string adUnit = "";
#if UNITY_IOS
        adUnit = maxAdInterIOS;
#elif UNITY_ANDROID || UNITY_EDITOR
        adUnit = maxAdInterId;
#endif
        if (MaxSdk.IsInterstitialReady(adUnit))
        {
            if (coutLoseGame >= timesLoseGameToShowInterAds)
            {
                coutLoseGame = 0;
                MaxSdk.ShowInterstitial(adUnit);
            }
        }
        else
        {
            MaxSdk.LoadInterstitial(adUnit);
        }
    }

    #endregion

    #region VIDEO ADS

    private Action<bool> videoRewardCallback;

    [Button]
    public void ShowVideoReward()
    {
        ShowRewarded(result =>
        {
            if (result)
            {
                Debug.Log("Show success");
            }
            else
            {
                Debug.Log("Load video ads");
            }
        });
    }

    public bool IsVideoRewardReady()
    {
        string adUnit = "";
#if UNITY_IOS
        adUnit = maxAdRewardIOS;
#elif UNITY_ANDROID || UNITY_EDITOR
        adUnit = maxAdRewardId;
#endif
        
        if (MaxSdk.IsRewardedAdReady(adUnit))
        {
            return true;
        }

        MaxSdk.LoadRewardedAd(adUnit);
        return false;
    }

    public void ShowRewarded(Action<bool> callback = null)
    {
        Debug.Log("Reward");

        string adUnit = "";
#if UNITY_IOS
        adUnit = maxAdRewardIOS;
#elif UNITY_ANDROID || UNITY_EDITOR
        adUnit = maxAdRewardId;
#endif
        videoRewardCallback = callback;
        if (MaxSdk.IsRewardedAdReady(adUnit))
        {
            MaxSdk.ShowRewardedAd(adUnit);
        }
        else
        {
            videoRewardCallback?.Invoke(false);
            MaxSdk.LoadRewardedAd(adUnit);
        }
    }

    private void OnRewardAdReceived(string arg1, MaxSdkBase.Reward arg2, MaxSdkBase.AdInfo arg3)
    {
        videoRewardCallback?.Invoke(true);
        Invoke("IsVideoRewardReady",2);
    }

    #endregion

    #region BANNER ADS

    [Button]
    public void ShowBanner()
    {
        string adUnit = "";
#if UNITY_IOS
        adUnit = maxAdBannerIOS;
#elif UNITY_ANDROID || UNITY_EDITOR
        adUnit = maxAdBannerId;
#endif
        
        MaxSdk.CreateBanner(adUnit, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.ShowBanner(adUnit);
    }


    IEnumerator AutoShowBanner()
    {
        yield return new WaitForSecondsRealtime(threshold);
        if (isShowBanner)
        {
            ShowBanner();
        }
    }

    #endregion

#if UNITY_EDITOR
    [Button]
    public void SetupPass(string pass = "K01202757498")
    {
        PlayerSettings.keystorePass = pass;
        PlayerSettings.keyaliasPass = pass;
        Debug.Log("Setup pass done");
    }
#endif
}