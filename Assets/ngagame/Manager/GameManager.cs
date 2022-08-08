using UnityEngine;

public class GameManager: MonoBehaviour
{
	public static bool Initialized = false;
	private void Awake()
	{
    if (Initialized)
    {
      return;
    }
		Application.targetFrameRate = 60;
		DontDestroyOnLoad(gameObject);
		Init();
	}

	public void Init()
	{
		Analytics.Instance.Init();
		Ads.Instance.Init();
		//SceneMaster.Instance.ShowBannerShield();
		Initialized = true;
	}

    private void Start()
    {

	}
}
