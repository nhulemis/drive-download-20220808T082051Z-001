using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Runner : MonoBehaviour
{
  public static Runner Instance;
  
  private Queue<UnityAction> callback;

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
    }

    else
    {
      Instance = this;
    }
    DontDestroyOnLoad(gameObject);
    callback = new Queue<UnityAction>();
  }


  public void AddEventCallBack(UnityAction action)
  {
    callback.Enqueue(action);
  }

  public void FixedUpdate()
  {
    if (callback. Count > 0)
    {
      var eventCallback = callback.Dequeue();
      eventCallback?.Invoke();
    }
  }
}
