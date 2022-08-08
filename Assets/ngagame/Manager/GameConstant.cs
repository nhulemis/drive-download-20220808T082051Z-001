using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstanst
{
    // Layers
    public static int Gameplay = 8;
    public static int Player = 9;
    public static int Element = 10;
    public static int Obstacle = 11;
	public static int UILayer = 5;

    // Tags
    public static string ObstacleTag = "Obstacle";

	// Fog
	public static float FogStartDistanceNear = 80;
	public static float FogEndDistanceNear = 150;
	public static float FogStartDistanceFar = 80;
	public static float FogEndDistanceFar = 150;

	// Config
	public static float FlyHeight = 5;
	public static float StartPosition = 6;
	public static int COIN_MUL_VALUE = 5;
	public static float START_JUMP_VELOCITY = 10.5f;
	public static float JUMP_GRAVITY = 16f;
}
