using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class ShopManager : Singleton<ShopManager>
{
	ShopData shopData;
	bool Initialized = false;
	ShopSave data;
	public static string SAVE_NAME = "shop_save";
	public static string DEFAULT_SKIN = "Default";
	public static int UNLOCK_RANDOM_PRICE = 1500;
	public static int LEVEL_TO_UNLOCK = 5;

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		shopData = Resources.Load<ShopData>("ShopData");

		try
		{
			if (PlayerPrefs.HasKey(SAVE_NAME))
			{
				string dataAsJson = PlayerPrefs.GetString(SAVE_NAME);
				// Pass the json to JsonUtility, and tell it to create a GameData object from it
				data = JsonUtility.FromJson<ShopSave>(dataAsJson);
			}
			else
			{
				data = new ShopSave();

				if (data.owned != null && data.owned.Count <= 0 && shopData != null && shopData.skinList.Length > 0)
				{
					data.owned.Add(shopData.skinList[0].ToString());
					data.currentSkin = shopData.skinList[0].ToString();
				}
				
				Debug.LogWarning("Init shop data!");
			}

		}
		catch { }

		if (data == null)
		{
			data = new ShopSave();
		}

		Initialized = true;
	}

	public ShopData Data
	{
		get
		{
			if(!Initialized)
			{
				Init();
			}
			return shopData;
		}
	}

	public string CurrentSkin
	{
		get
		{
			if (!Initialized)
			{
				Init();
			}
			return data != null ? data.currentSkin : shopData.skinList[0].ToString();
		}

		set
		{
			if (!Initialized)
			{
				Init();
			}
			if (data != null)
			{
				data.currentSkin = value;
				if (data.newUnlocks.Contains(value))
				{
					data.newUnlocks.Remove(value);
				}

				SaveData();
			}
		}
	}
	
	public void Unlock(string id, bool equip)
	{
		if(data != null && data.owned != null)
		{
			data.owned.Add(id);

			if(equip)
			{
				data.currentSkin = id;
				if (data.newUnlocks.Contains(id))
				{
					data.newUnlocks.Remove(id);
					Debug.LogError(data.newUnlocks.Contains(id));
				}
			} else
			{
				data.newUnlocks.Add(id);
			}

			SaveData();
		}
	}

	public void Claim(string id)
	{
		if (data != null && data.rvLocks != null)
		{
			data.rvLocks.Add(id);
			data.newUnlocks.Add(id);

			SaveData();
		}
	}

	public bool Owned(string id)
	{
		if(!Initialized)
		{
			Init();
		}
		return data != null && data.owned != null ? data.owned.Contains(id) : false;
	}

	public bool HasNew()
	{
		if (!Initialized)
		{
			Init();
		}
		return Profile.Instance.Coins >= UNLOCK_RANDOM_PRICE || data.newUnlocks.Count > 0;
	}

	public bool IsNew(string skinName)
	{
		if (!Initialized)
		{
			Init();
		}
		return data.newUnlocks.Contains(skinName);
	}

	public bool IsFullSkin
	{
		get
		{
			return data.owned.Count >= shopData.skinList.Length;
		}
	}

	void SaveData()
	{
		string dataAsJson = JsonUtility.ToJson(data);
		try
		{
			PlayerPrefs.SetString(SAVE_NAME, dataAsJson);
		}
		catch
		{

		}
	}

	[System.Serializable]
	public class ShopSave
	{
		public List<string> owned = new List<string>();
		public List<string> newUnlocks = new List<string>();
		public List<string> rvLocks = new List<string>();
		public string currentSkin;
	}

	public string SkinInProgress(int index)
	{
		if(shopData == null)
		{
			return string.Empty;
		}
		var guess = shopData.skinList[index % shopData.skinList.Length];
		if (Owned(guess.ToString()) || guess == DEFAULT_SKIN)
		{
			foreach (var skin in shopData.skinList)
			{
				if (!Owned(skin.ToString()))
				{
					return skin;
				}
			}
			return string.Empty;
		}
		else
		{
			return guess;
		}
	}

	public string RandomRewardSkin()
	{
		List<string> avaibles = new List<string>();
		foreach (var skin in shopData.skinList)
		{
			if (!Owned(skin.ToString()))
			{
				avaibles.Add(skin);
			}
		}
		if (avaibles.Count <= 0)
		{
			return string.Empty;
		}

		return avaibles[Random.Range(0, avaibles.Count)];
	}

	public GameObject GetSkinModel(string skinName)
	{
		return Resources.Load<GameObject>("Skins/" + skinName);
	}

	public Sprite GetSkinPreview(string skinName)
	{
		return Resources.Load<Sprite>("Skins/Preview/" + skinName);
	}
}