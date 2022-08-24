#if USE_GA
using GameAnalyticsSDK;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analytics : Singleton<Analytics>
{
	private bool Initialized = false;

	public void Init()
	{
		if(Initialized)
		{
			return;
		}
		RequestConversion();

#if USE_GA
		GameAnalyticsSDK.GameAnalytics.Initialize();
		GameAnalyticsSDK.GameAnalytics.OnRemoteConfigsUpdatedEvent += OnRemoteConfigsUpdatedEvent;
#endif

#if USE_FIREBASE
		if(FirebaseInitialized)
		{
			return;
		}
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				var app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here to indicate whether Firebase is ready to use by your app.
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
#endif

		Initialized = true;
	}

    private void Start()
    {
		Debug.Log("SubscribeIronSourceImpressions");
	}

    void OnRemoteConfigsUpdatedEvent()
	{
		try
		{
			int InterstitialIntervalTime = 0;
			bool useTimer = true;
			int levelVariant = 0;
#if USE_GA
			int.TryParse(GameAnalytics.GetRemoteConfigsValueAsString("fs_time", "60"), out InterstitialIntervalTime);
			bool.TryParse(GameAnalytics.GetRemoteConfigsValueAsString("use_timer", "true"), out useTimer);
			GameAnalytics.SetCustomDimension01(levelVariant > 0 ? "B" : "A");
			Debug.Log("dungnv level_ab: " + levelVariant);
#endif
			//ads_go.Instance.InterstitialIntervalTime = InterstitialIntervalTime;
		} catch { }
	}

	public void LogEvent(string name)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(name);

		if (Initialized)
		{
#if USE_FIREBASE
			Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
#endif
		}

#if USE_GA
		GameAnalytics.NewDesignEvent(name);
#endif
		
	}

	public void LogEvent(string name, string param, string value)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(name, new Dictionary<string, object>
			{
				{param, value}
			});
		
#if USE_GA
		GameAnalytics.NewDesignEvent(name, new Dictionary<string, object>
		{
			{param, value}
		});
#endif
	}

#region Funnel

	public void LevelStart()
	{
		LogEvent($"level_{Profile.Instance.Level}_start");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void LevelComplete()
	{
		LogEvent($"level_{Profile.Instance.Level}_win");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void LevelFail()
	{
		LogEvent($"level_{Profile.Instance.Level}_lose");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void LevelRevive()
	{
		LogEvent($"level_{Profile.Instance.Level}_revive");
	}

	public void LevelTimeout()
	{
		LogEvent($"level_{Profile.Instance.Level}_timeout");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void AdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement)
	{
		LogEvent($"{adAction.ToString()}_{adType.ToString()}");
#if USE_GA
		try
		{
			GameAnalytics.NewAdEvent((GameAnalyticsSDK.GAAdAction)adAction, (GameAnalyticsSDK.GAAdType)adType, adSdkName, adPlacement);
		} catch
		{

		}
#endif
	}

	#endregion

	#region ConversionTracking

	public void RequestConversion()
	{
		StartCoroutine(IERequestConversion());
	}

	IEnumerator IERequestConversion()
	{
#if USE_AF
		while(AppsFlyerObjectScript.ConversionDataDictionary == null)
		{
			yield return new WaitForSeconds(1);
			var ConversionDataDictionary = AppsFlyerObjectScript.ConversionDataDictionary;
			if (ConversionDataDictionary != null)
			{
				if(ConversionDataDictionary.ContainsKey("af_status"))
				{
					LogEvent(ConversionDataDictionary["af_status"].ToString().ToLower());
					Debug.Log("dungnv: af_status: " + ConversionDataDictionary["af_status"]);
				}
			}
		}
#else
		yield return null;
#endif
	}

#endregion
}

public enum GAAdAction
{
	Undefined = 0,
	Clicked = 1,
	Show = 2,
	FailedShow = 3,
	RewardReceived = 4,
	Request = 5,
	Loaded = 6
}

public enum GAAdType
{
	Undefined = 0,
	Video = 1,
	RewardedVideo = 2,
	Playable = 3,
	Interstitial = 4,
	OfferWall = 5,
	Banner = 6
}