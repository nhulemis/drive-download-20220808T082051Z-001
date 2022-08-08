using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
	[SerializeField] GameObject[] levels;

	public PlayerLoader PlayerLoader { get; private set; }
	public static Gameplay Instance;

	private void Awake()
	{
		Instance = this;
		SceneMaster.Instance.OpenScene(SceneID.Main);
		Init();
	}

	void Init()
	{
		var road = GetComponentInChildren<Road>();
		if(road == null)
		{
			var prefab = levels[(Profile.Instance.Level - 1) % levels.Length];
			Instantiate(prefab, transform);
		}
		
		PlayerLoader = GetComponentInChildren<PlayerLoader>();
		Gem.Earned = 0;
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
