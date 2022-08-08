using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum SceneID
{
	Main,
	Gameplay,
	Home,
	Result,
	RewardSkin,
	ChestRoom,
	Settings,
	Shop
}

public class SceneMaster : Singleton<SceneMaster>
{
	public void OpenScene(SceneID scene)
	{
		SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
	}

	public void CloseScene(SceneID scene)
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == scene.ToString())
			{
				SceneManager.UnloadSceneAsync(scene.ToString());
			}
		}
	}

	public void ReloadScene(SceneID scene)
	{
		bool exist = false;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == scene.ToString())
			{
				exist = true;
			}
		}
		if (exist)
        {
			
		}
		SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
	}

	

	LoadingPanel loadingPanel;
	private void Awake()
	{
		var loadingPanelPrefab = Resources.Load<LoadingPanel>("LoadingPanel");
		loadingPanel = Instantiate(loadingPanelPrefab);
	}

	float showLoadingTime;
	public void ShowLoading(float time = 3f, bool useShield = false)
	{
		if(loadingPanel == null)
		{
			return;
		}
		loadingPanel.gameObject.SetActive(true);
		loadingPanel.loadingObject.SetActive(!useShield);
		loadingPanel.shieldObject.SetActive(useShield);
		loadingPanel.CanvasGroup.DOKill();
		loadingPanel.CanvasGroup.alpha = 1;
		showLoadingTime = time;
	}

	public void HideLoading()
	{
		if (loadingPanel == null)
		{
			return;
		}
		loadingPanel.CanvasGroup.DOKill();
		loadingPanel.CanvasGroup.DOFade(0, 0.2f).OnComplete(delegate
		{
			loadingPanel.gameObject.SetActive(false);
		});
	}

	public void ShowBannerShield()
	{
		if (loadingPanel == null)
		{
			return;
		}
		loadingPanel.ShowBannerShield();
	}

	private void FixedUpdate()
	{
		if(loadingPanel == null || !loadingPanel.gameObject.activeSelf)
		{
			return;
		}

		showLoadingTime -= Time.fixedDeltaTime;
		if(showLoadingTime < 0)
		{
			loadingPanel.loadingObject.gameObject.SetActive(false);
			loadingPanel.shieldObject.gameObject.SetActive(false);
		}
	}

#if UNITY_EDITOR
	private void OnGUI()
	{
		return;
		if (GUI.Button(new Rect(Size(1), Size(1), Size(3), Size(1)), "Open"))
		{
			OpenScene(SceneID.Gameplay);
		}

		if (GUI.Button(new Rect(Size(1), Size(2), Size(3), Size(1)), "Reload"))
		{
			ReloadScene(SceneID.Gameplay);
		}

		if (GUI.Button(new Rect(Size(1), Size(3), Size(3), Size(1)), "Close"))
		{
			CloseScene(SceneID.Gameplay);
		}

	}

	float Size(int size)
	{
		return Screen.width * 0.1f * size;
	}
#endif
}
