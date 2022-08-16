// using System;
// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.Networking;
// using System.Collections;
// using System.Diagnostics.Contracts;
// using DG.Tweening;
// using GoogleMobileAds.Api;
// using UnityEngine.Events;
//
// public class Ads : Singleton<Ads>
// {
// 	public static bool Showing = false;
//
// 	UnityAction<bool> RewardCallback;
// 	//[SerializeField]AdsSetting adsSetting;
// 	public bool initialized = false;
//
//   public float InterstitialTime { get; set; } = 0;
// 	public int InterstitialIntervalTime { get; set; } = 35;
//
// 	public void Init()
// 	{
// 		if (initialized)
// 		{
// 			return;
// 		}
//
// 		StartCoroutine(InternalInit());
// 	}
//
// 	IEnumerator InternalInit()
// 	{
// 		//adsSetting = Resources.Load<AdsSetting>("Adsseting");
//     
// #if GOOGLE_ADS_ENABLE
// 		// Initialize the Mobile Ads SDK.
// 		MobileAds.Initialize((initStatus) =>
// 		{
//       Debug.Log("Init ads Done");
// 			RequestInterstitial();
// 			RequestRewardAd();
// 			
//
// 			Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
// 			foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
// 			{
// 				string className = keyValuePair.Key;
// 				AdapterStatus status = keyValuePair.Value;
// 				switch (status.InitializationState)
// 				{
// 					case AdapterState.NotReady:
// 						// The adapter initialization did not complete.
// 						Debug.Log("Adapter: " + className + " not ready.");
// 						break;
// 					case AdapterState.Ready:
// 						// The adapter was successfully initialized.
// 						Debug.Log("Adapter: " + className + " is initialized.");
// 						break;
// 				}
// 			}
// 			initialized = true; // Load one time
// 		});
// #endif
// 		InterstitialTime = Time.realtimeSinceStartup;
//     yield return null;
//   }
//
// 	public void ShowRewardedAd(UnityEngine.Events.UnityAction<bool> callback, string placementName = "")
// 	{
// 		if(!ngagame.Utils.MobilePlatform)
// 		{
// 			callback?.Invoke(true);
// 			return;
// 		}
// #if GOOGLE_ADS_ENABLE
// 	if (this.rewardedAd.IsLoaded())
// 		{
// 			this.rewardedAd.Show();
// 			RewardCallback = callback;
// 			InterstitialTime = Time.realtimeSinceStartup;
// 		}
// 		else
// 		{
// 			RequestRewardAd();
// 			callback?.Invoke(false);
// 			Analytics.Instance.LogEvent("attemp_request_reward");
// 			ngagame.Utils.Toast("No AD available");
// 		}
// #endif
// 	}
//
// 	public void ShowInterstitial(string placement = "none" , bool show = false)
// 	{
// 		if (Profile.Instance.VIP)
// 		{
// 			Debug.LogError("ngagame VIP");
// 			return;
// 		}
// 		SceneMaster.Instance.ShowLoading();
// 		DOVirtual.DelayedCall(2f, () =>
// 		{
//
// #if GOOGLE_ADS_ENABLE
// 			if (this.interstitial.IsLoaded())
// 			{
// 				SceneMaster.Instance.ShowLoading(1);
// 				if (ngagame.Utils.MobilePlatform)
// 				{
// 					this.interstitial.Show();
// 				}
// 			}
// 			else
// 			{
// 				if (Application.internetReachability != NetworkReachability.NotReachable)
// 				{
// 					isShowAfterRequest = show;
// 					RequestInterstitial();
// 					Analytics.Instance.LogEvent("attemp_request_interstitial");
// 				}
//
// 			}
// #endif
// 		});
//
// 	}
// #if GOOGLE_ADS_ENABLE
// 	InterstitialAd interstitial;
//   private bool isShowAfterRequest;
// #endif
// 	private void RequestInterstitial()
// 	{
// #if UNITY_ANDROID
//     string adUnitId = ads_go.Instance.adUnitIdInter; 
// #elif UNITY_IPHONE
//         string adUnitId = adsSetting.googleInterstitialUnitIdIphone;
// #else
//         string adUnitId = "unexpected_platform";
// #endif
//
// #if GOOGLE_ADS_ENABLE
// 		// Initialize an InterstitialAd.
// 		this.interstitial = new InterstitialAd(adUnitId);
// 		// Called when an ad is shown.
// 		this.interstitial.OnAdOpening += HandleOnAdOpened;
// 		this.interstitial.OnAdClosed += HandleOnAdClosed;
// 		this.interstitial.OnAdFailedToShow += OnAdFailedToShow;
// 		this.interstitial.OnAdFailedToLoad += OnAdFailedToLoad;
//     this.interstitial.OnAdLoaded += OnAdInterLoaded;
// 		// Create an empty ad request.
// 		AdRequest request = new AdRequest.Builder().Build();
// 		// Load the interstitial with the request.
// 		this.interstitial.LoadAd(request);
//     Debug.Log("Init inter Ads : " + adUnitId);
//
// #endif
// 	}
//
//   private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
//   {
//     SceneMaster.Instance.HideLoading();
//     Debug.Log("Load next on Fail Load");
//
//   }
//
//   private void OnAdInterLoaded(object sender, EventArgs e)
//   {
//     Debug.Log("Loaded");
//   }
//
//   public void HandleOnAdOpened(object sender, EventArgs args)
// 	{
// 		SceneMaster.Instance.HideLoading();
// 		InterstitialTime = Time.realtimeSinceStartup;
// 	}
//
// 	public void HandleOnAdClosed(object sender, EventArgs args)
// 	{
// 		SceneMaster.Instance.HideLoading();
// 		RequestInterstitial();
//   }
//
// 	public void OnAdFailedToShow(object sender, EventArgs args)
// 	{
// 		SceneMaster.Instance.HideLoading();
//     Debug.Log("Load next on fail");
//     FindObjectOfType<LoadNextScene>()?.LoadNext();
//
// 	}
//
// #if GOOGLE_ADS_ENABLE
// 	RewardedAd rewardedAd;
// #endif
// 	private void RequestRewardAd()
// 	{
// 		string adUnitId;
// #if UNITY_ANDROID
// 		adUnitId = ads_go.Instance.adUnitIdReward;
// #elif UNITY_IPHONE
//             adUnitId = adsSetting.googleRewardUnitIdIphone;
// #else
//             adUnitId = "unexpected_platform";
// #endif
//
// #if GOOGLE_ADS_ENABLE
// 		this.rewardedAd = new RewardedAd(adUnitId);
//
// 		// Called when the ad is closed.
// 		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
//
// 		// Create an empty ad request.
// 		AdRequest request = new AdRequest.Builder().Build();
// 		// Load the rewarded ad with the request.
// 		this.rewardedAd.LoadAd(request);
// #endif
// 	}
//
// 	public void HandleRewardedAdClosed(object sender, EventArgs args)
// 	{
// 		RewardCallback?.Invoke(true);
// 		RewardCallback = null;
// 		SceneMaster.Instance.HideLoading();
// 		RequestRewardAd();
// 	}
//
// 	IEnumerator GetCountryCode(Action<string> onComplete)
// 	{
// 		string uri = "http://ip-api.com/json";
// 		string countryCode = string.Empty;
// 		using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
// 		{
// 			// Request and wait for the desired page.
// 			yield return webRequest.SendWebRequest();
// 			var receivedText = string.Empty;
// 			switch (webRequest.result)
// 			{
// 				case UnityWebRequest.Result.ConnectionError:
// 				case UnityWebRequest.Result.DataProcessingError:
// 					Debug.Log("Error: " + webRequest.error);
// 					break;
// 				case UnityWebRequest.Result.ProtocolError:
// 					Debug.Log("HTTP Error: " + webRequest.error);
// 					break;
// 				case UnityWebRequest.Result.Success:
// 					receivedText = webRequest.downloadHandler.text;
// 					break;
// 			}
//
// 			if (!string.IsNullOrEmpty(receivedText))
// 			{
// 				var dict = SimpleJSON.JSON.Parse(receivedText);
//
// 				if (dict != null && dict.HasKey("countryCode"))
// 				{
// 					countryCode = dict["countryCode"];
// 					Debug.Log("dungnv: countryCode: " + countryCode);
// 				}
// 			}
// 			onComplete?.Invoke(countryCode.ToLower());
// 		}
// 	}
// }
