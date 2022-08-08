using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectBone : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    Vector3 originOffset;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            return;
        }
        originOffset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
		{
            return;
		}
        transform.position = target.position + originOffset + offset;
    }
}
