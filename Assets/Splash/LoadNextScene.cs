using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      StartCoroutine(LoadNExt());
    }

    IEnumerator LoadNExt()
    {
      yield return new WaitForSecondsRealtime(2);
      Debug.Log("Show ne con trai");

#if true
      if (Application.internetReachability != NetworkReachability.NotReachable)
        {
          ads_go.Instance.ShowInterstitial();
        }
        else
#endif
        
        {
          LoadNext();
        }
        
    }

    public void LoadNext()
    {
      Debug.Log("LoadNext");
      SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
