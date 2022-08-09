using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
  private bool showAdDone = false;
  // Start is called before the first frame update
    void Start()
    {
      ads_go.Instance.LoadAd();
      StartCoroutine(LoadNExt());
    }

    IEnumerator LoadNExt()
    {
      yield return new WaitForSecondsRealtime(2);
      Debug.Log("Show ne con trai");

#if true
      if (Application.internetReachability != NetworkReachability.NotReachable)
        {
          ads_go.Instance.ShowAdIfAvailable((value) =>
          {
            showAdDone = value;
          });
        }
        else
#endif
        
        {
          LoadNext();
        }
        
    }

    private AsyncOperation scene = null;
    public void LoadNext()
    {
      Debug.Log("LoadNext");
      scene = SceneManager.LoadSceneAsync(1);
    }

    // Update is called once per frame
    void Update()
    {
      if (showAdDone && scene == null)
      {
        showAdDone = false;
        LoadNext();
      }
    }
}
