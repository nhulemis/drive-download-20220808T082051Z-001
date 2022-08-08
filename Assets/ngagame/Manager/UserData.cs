using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class UserData
{
    public bool vip = false;
	public bool firstOpen = true;
	public bool rateShowed = false;
	public int coins = 0;
	public int level = 1;
	public string currentSkin = "Default";
	public List<string> ownedSkins = new List<string>(0);
	public float skinProgress = 0;
	public int unlockSkinIndex = 0;
	public int keyAmount = 0;
}
