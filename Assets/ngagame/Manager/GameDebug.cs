using UnityEngine;
using System.Collections;

public class GameDebug : MonoBehaviour
{
	[SerializeField] bool debug;
	[SerializeField] bool autoScreenshot;

    private void Start()
    {
#if UNITY_EDITOR
		if(autoScreenshot)
        {
			StartCoroutine(IEAutoScreenshot());
		}
#endif
	}

	IEnumerator IEAutoScreenshot()
    {
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.5f);
		ScreenCapture.CaptureScreenshot($"Screenshots/level_{Profile.Instance.Level.ToString("000")}.png");
		Profile.Instance.Level++;
		yield return new WaitForEndOfFrame();
		SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
	}

    void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	float deltaTime = 0.0f;
	void OnGUI()
	{
		if(!debug)
        {
			return;
        }
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);

		GUI.skin.button.fontSize = 50;
		if (GUI.Button(new Rect(Screen.width - 150, Screen.height - 70, 150, 70), "Next"))
		{
			Profile.Instance.Level++;
			SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		}

		if (GUI.Button(new Rect(0, Screen.height - 70, 150, 70), "Prev"))
		{
			Profile.Instance.Level --;
			SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		}
	}
}