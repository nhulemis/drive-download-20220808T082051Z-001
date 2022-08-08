using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Patrol : MonoBehaviour
{
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] float time = 1;
    [SerializeField] float sleep = 0.25f;
    [SerializeField] float delay = 0.0f;
    [SerializeField] Ease ease = Ease.Linear;
    [SerializeField] Transform move;

    Sequence mySequence = null;
    public void Start()
    {
		var _start = start;
		var _end = end;

        move.transform.localPosition = _start;
        // Grab a free Sequence to use
        mySequence = DOTween.Sequence();
        mySequence.Append(move.DOLocalMove(_end, time).SetEase(ease).SetDelay(sleep));
        mySequence.Append(move.DOLocalMove(_start, time).SetEase(ease).SetDelay(sleep));
        mySequence.SetLoops(-1, LoopType.Restart).SetEase(ease);
        if(delay > 0 )
		{
            mySequence.SetDelay(delay);

        }
    }
}
