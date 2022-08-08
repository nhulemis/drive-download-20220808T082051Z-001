using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEditor;
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

	void checkOtherADS(){
		gameObject.name = "qwe";

		if (GameObject.Find("ADS")){
			Destroy(gameObject);
		}else{
			gameObject.name = "ADS";
		}

	}

	void Start () {

		Debug.Log("alo");
		checkOtherADS();

		DontDestroyOnLoad(this.gameObject);

		List<string> deviceIds = new List<string>();
		deviceIds.Add("0E32786D7B4A68EB4B2A5C9DCF763E66");
		RequestConfiguration requestConfiguration = new RequestConfiguration
				.Builder()
			.SetTestDeviceIds(deviceIds)
			.build();
		MobileAds.SetRequestConfiguration(requestConfiguration);
		
		MobileAds.Initialize(init =>
		{
			Debug.Log("Init ads Done");
		});

		
		this.rewardedAd = new RewardedAd(adUnitIdReward);

		// Called when an ad request has successfully loaded.
		this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		// Called when an ad request failed to load.
		this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		// Called when an ad is shown.
		this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
		// Called when an ad request failed to show.
		this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		// Called when the user should be rewarded for interacting with the ad.
		this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		// Called when the ad is closed.
		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		

		RequestInterstitial();
		RequestRewardBasedVideo();
	}

	private void HandleRewardedAdClosed(object sender, EventArgs e)
	{
		Debug.Log("HandleRewardedAdClosed");
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

				gameTimer = 0;
				this.interstitial.Show();
				RequestInterstitial();
			}
		}


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
    FindObjectOfType<LoadNextScene>()?.LoadNext();

	}

	private void HandleOnAdOpening(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdOpening event received");
	}

	private void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed event received");
		FindObjectOfType<LoadNextScene>()?.LoadNext();
	}
	
	private void RequestRewardBasedVideo()
	{
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the rewarded ad with the request.
		this.rewardedAd.LoadAd(request);
	}


}
