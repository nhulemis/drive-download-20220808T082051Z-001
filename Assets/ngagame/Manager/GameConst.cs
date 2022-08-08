using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConst", menuName = "GameConst")]
public class GameConst : ScriptableObject
{
	public int playerLayer;
	public int brokenLayer;
	public int ragdollLayer;
	public string playertag;
	public LayerMask elementLayer;

	static GameConst data;
	public static GameConst Data
	{
		get
		{
			if(data == null)
			{
				data = Resources.Load<GameConst>("GameConst");
				if(data == null)
				{
					data = new GameConst();
				}
			}
			return data;
		}
	}
}
