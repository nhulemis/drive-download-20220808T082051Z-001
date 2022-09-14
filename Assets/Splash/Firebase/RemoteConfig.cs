using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UnityEngine;

public class RemoteConfig : MonoBehaviour
{
    public bool IsInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is intitialized.
                FirebaseApp app = FirebaseApp.DefaultInstance;

                IsInitialized = true;
                LoadConfig();

                // WARNING: Do not call Crashlytics APIs from asynchronous tasks;
                // they are not currently supported.

                // Set a flag here for indicating that your project is ready to use Firebase.
                Debug.LogFormat("[Job] <color=#2e9947> FirebaseInitializer Success Job -----> {0} s</color>",
                    Time.realtimeSinceStartup);
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void LoadConfig()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>()
        {
            //{ "remoteConfigInfo", ""}
        };
        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
       var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(6));

       fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    async void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }
        
        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                // re-fresh new config
                OverrideConfig();

                Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
                break;
        }

        
    }
    
    

    private void OverrideConfig()
    {
        var firebaseConfig = FirebaseRemoteConfig.DefaultInstance;

        foreach (var config in firebaseConfig.AllValues)
        {
            Log.Debug(config.Value.StringValue);
            if (config.Key == "ShowAds")
            {
              var showAdConfig =  JsonConvert.DeserializeObject<ShowAdsConfig>(config.Value.StringValue);
              if (showAdConfig != null)
              {
                  ads_go.Instance.timesLoseGameToShowInterAds = showAdConfig.TimesToShow;
                  ads_go.Instance.isShowAds = showAdConfig.ShowAds;
              }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public class ShowAdsConfig
    {
        public bool ShowAds { get; set; }
        public int TimesToShow { get; set; }
    }
}