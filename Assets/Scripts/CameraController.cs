using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public GameObject target;        //Public variable to store a reference to the player game object


    private Vector3 offset;            //Private variable to store the offset distance between the player and camera
    private Vector3 targetPosition;            //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - target.transform.position;
    }

	private void OnDestroy()
	{
        Instance = null;
	}

	// LateUpdate is called after Update each frame
	void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        targetPosition = target.transform.position + offset;
        targetPosition.x = transform.position.x;
        transform.position = targetPosition;
    }

    public void SetTarget(GameObject _target)
	{
        target = _target;
        offset = transform.position - target.transform.position;
    }
}