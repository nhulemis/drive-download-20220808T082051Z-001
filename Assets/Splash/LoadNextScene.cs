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
      //ads_go.Instance.LoadAd();
      StartCoroutine(LoadNExt());
    }

    IEnumerator LoadNExt()
    {
      yield return new WaitForSecondsRealtime(1.5f);
      yield return LoadGamePlay();
    }

   // private AsyncOperation scene = null;
    public void LoadNext()
    {
      //StartCoroutine(LoadGamePlay());

    }

    IEnumerator LoadGamePlay()
    {
      Debug.Log("LoadNext");
     var scene = SceneManager.LoadSceneAsync(1,LoadSceneMode.Single);

      
      //
      yield return null;
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
