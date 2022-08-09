using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEditor;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ads_go : Singleton<ads_go> {
	private BannerView bannerView;
	private InterstitialAd interstitial;
	private RewardedAd rewardedAd;
	private GameObject Rew;

	[Space]
	[Header("ADMOB ID:")]
	[Space]
	[Header("   *******************************************")]
	[Header("      Assets -> Google Mobile Ads -> Settings...")]
	[Header("   Do not forget to change the ADMOB APP ID in menu:")]
	[Header("   *******************************************")]

		//public string appId;

		public string adUnitIdBanner;
		public string adUnitIdInter;
		public string adUnitIdReward;
    [Space] public string adUnitOpenApp;

#region AdOpenApp

private UnityAction<bool> startAppCallBack;
    private AppOpenAd ad;

    private bool isShowingAd = false;

    private bool IsAdAvailable
    {
      get
      {
        return ad != null;
      }
    }

    public void LoadAd()
    {
      AdRequest request = new AdRequest.Builder().Build();

      // Load an app open ad for portrait orientation
      AppOpenAd.LoadAd(adUnitOpenApp, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
      {
        if (error != null)
        {
          // Handle the error.
          Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
          return;
        }

        // App open ad is loaded.
        ad = appOpenAd;
      }));
    }
    
    public void ShowAdIfAvailable(UnityAction<bool> callback = null)
    {
      if (!IsAdAvailable || isShowingAd)
      {
        Log.Debug("not available");
        callback?.Invoke(true);
        return;
      }

      startAppCallBack = callback;
      ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
      ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
      ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
      ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
      ad.OnPaidEvent += HandlePaidEvent;

      ad.Show();
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
      Debug.Log("Closed app open ad");
      // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
      ad = null;
      isShowingAd = false;
      LoadAd();
      startAppCallBack?.Invoke(true);
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
      Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
      // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
      ad = null;
      LoadAd();
      startAppCallBack?.Invoke(true);

    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
      Debug.Log("Displayed app open ad");
      isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
      Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
      Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
        args.AdValue.CurrencyCode, args.AdValue.Value);
    }

#endregion
    
		[Space]
	[Space]
		public bool showBanner;
		public float delayADS;
	[Space]
	[Space]
	public string redirectToScene;

	private float gameTimer;
	private string paramReward;

	private int loads;
  [SerializeField] private LoadNextScene _loadNextScene;

	void checkOtherADS(){
		gameObject.name = "qwe";

		if (GameObject.Find("ADS")){
			Destroy(gameObject);
		}else{
			gameObject.name = "ADS";
		}

	}

  private void Awake()
  {
    DontDestroyOnLoad(this);
  }

  void Start () {

		// Debug.Log("alo");
		// checkOtherADS();
		//
		// DontDestroyOnLoad(this.gameObject);
		//
		// List<string> deviceIds = new List<string>();
		// deviceIds.Add("BC82D570192ECB14959E0F901038C49A");
		// RequestConfiguration requestConfiguration = new RequestConfiguration
		// 		.Builder()
		// 	.SetTestDeviceIds(deviceIds)
		// 	.build();
		// MobileAds.SetRequestConfiguration(requestConfiguration);
		//
		// MobileAds.Initialize(init =>
		// {
		// 	Debug.Log("Init ads Done");
		// });
		//
		//
		// this.rewardedAd = new RewardedAd(adUnitIdReward);
		//
		// // Called when an ad request has successfully loaded.
		// this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		// // Called when an ad request failed to load.
		// this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		// // Called when an ad is shown.
		// this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
		// // Called when an ad request failed to show.
		// this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		// // Called when the user should be rewarded for interacting with the ad.
		// this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		// // Called when the ad is closed.
		// this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
		//
		//
		//
		// RequestInterstitial();
		// RequestRewardBasedVideo();
	}

	private void HandleRewardedAdClosed(object sender, EventArgs e)
	{
		Debug.Log("HandleRewardedAdClosed");
    RewardCallback?.Invoke(true);
    RewardCallback = null;
    SceneMaster.Instance.HideLoading();
		RequestRewardBasedVideo();
	}

	private void HandleUserEarnedReward(object sender, Reward e)
	{
		Debug.Log("HandleUserEarnedReward");
	}

	private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
	{
		Debug.Log("HandleRewardedAdFailedToShow");
	}

	private void HandleRewardedAdOpening(object sender, EventArgs e)
	{
		Debug.Log("HandleRewardedAdOpening");
	}

	private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log("HandleRewardedAdFailedToLoad");
	}

	private void HandleRewardedAdLoaded(object sender, EventArgs e)
	{
		Debug.Log("HandleRewardedAdLoaded");
	}


	void Update()
	{
		gameTimer += Time.deltaTime;
		if (gameTimer > delayADS && loads == 0)
		{
			loads = 1;
			if (showBanner == true)
			{
				if (PlayerPrefs.GetInt("noads") != 1)
				{
					this.RequestBanner();
					bannerView.Show();
				}
			}
		}

	}

	public void ShowRandom()
	{
		var r = Random.Range(0, 100);
		if (r % 2== 0)
		{
			ShowInterstitial();
		}

		else
		{
			ShowRewarded();
		}
	}

	public void ShowInterstitial()
	{
		if (PlayerPrefs.GetInt("noads") != 1)
		{
			if (this.interstitial.IsLoaded())
			{
        Debug.Log("Show");
        //SceneMaster.Instance.ShowLoading(1);
				gameTimer = 0;
				this.interstitial.Show();
				RequestInterstitial();
			}
		}


	}
  
  
  public float InterstitialTime { get; set; } = 0;
  public int InterstitialIntervalTime { get; set; } = 35;	
  UnityAction<bool> RewardCallback;

  
  public void ShowRewardedAd(UnityEngine.Events.UnityAction<bool> callback, string placementName = "")
  {
    if(!ngagame.Utils.MobilePlatform)
    {
      callback?.Invoke(true);
      return;
    }
#if GOOGLE_ADS_ENABLE
    if (this.rewardedAd.IsLoaded())
    {
      this.rewardedAd.Show();
      RewardCallback = callback;
      InterstitialTime = Time.realtimeSinceStartup;
    }
    else
    {
      RequestRewardBasedVideo();
      callback?.Invoke(false);
      Analytics.Instance.LogEvent("attemp_request_reward");
      ngagame.Utils.Toast("No AD available");
    }
#endif
  }

	public void ShowRewarded()
	{

		if (rewardedAd.IsLoaded ()) {
			rewardedAd.Show ();
		}
	}

	public void ShowBanner()
	{
		if (showBanner == true)
		{
			if (PlayerPrefs.GetInt("noads") != 1)
			{
				gameTimer = 0;
				loads = 0;
			}
		}
	}

	void HideBanner()
	{
		if (PlayerPrefs.GetInt("noads") != 1){
			bannerView.Hide();
			RequestBanner();
		}
	}

	private void RequestBanner()
	{
		this.bannerView = new BannerView(adUnitIdBanner, AdSize.Banner, AdPosition.Bottom);
		AdRequest request = new AdRequest.Builder().Build();
		bannerView.LoadAd(request);
		bannerView.Hide();

	}
	private void RequestInterstitial()
	{
		
		// Initialize an InterstitialAd.
		this.interstitial = new InterstitialAd(adUnitIdInter);

		// Called when an ad request has successfully loaded.
		this.interstitial.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		// Called when an ad is shown.
		this.interstitial.OnAdOpening += HandleOnAdOpening;
		// Called when the ad is closed.
		this.interstitial.OnAdClosed += HandleOnAdClosed;
    
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		this.interstitial.LoadAd(request);

	}
	
	private void HandleOnAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLoaded event received");
	}

	private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
		                    + args);
		if (showBanner == true)
		{
			if (PlayerPrefs.GetInt("noads") != 1)
			{
				this.RequestBanner();
				bannerView.Show();
			}
		}
    _loadNextScene?.SendMessage("LoadNext");

	}

	private void HandleOnAdOpening(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdOpening event received");
	}

	private void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed event received");
    _loadNextScene?.SendMessage("LoadNext");
	}
	
	private void RequestRewardBasedVideo()
  {
    this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the rewarded ad with the request.
		this.rewardedAd.LoadAd(request);
	}


}
