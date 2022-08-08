using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsSetting : ScriptableObject
{
	public string googleRewardUnitIdAndroid = "ca-app-pub-6083557332551472/1163005214";
	public string googleInterstitialUnitIdAndroid = "ca-app-pub-6083557332551472/7568900967";

	public string googleRewardUnitIdIphone = "ca-app-pub-3940256099942544/1712485313";
	public string googleInterstitialUnitIdIphone = "ca-app-pub-3940256099942544/4411468910";

	public bool debug = false;
	public List<string> testDeviceIds = new List<string>();
}
