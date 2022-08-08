using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSkin : MonoBehaviour
{
    [SerializeField] string savePath;
    [SerializeField] Camera cam;
    void Start()
    {
        StartCoroutine(AutoCapture());
    }

    IEnumerator AutoCapture()
	{
        var skins = ShopManager.Instance.Data.skinList;

        foreach(var s in skins)
		{
            var prefab = ShopManager.Instance.GetSkinModel(s);
            var skin = Instantiate(prefab, transform);
            var player = skin.GetComponent<Player>();
            if(player != null)
			{
                player.CalculateWeight("+10");
			}
            yield return new WaitForEndOfFrame();
            Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = cam.targetTexture;
            tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes($"{Application.dataPath}/{savePath}/{s}.png", bytes);
            skin.gameObject.SetActive(false);
            Debug.LogError($"{s}, {skin.gameObject.activeSelf}");
            yield return new WaitForEndOfFrame();
        }
	}
}
