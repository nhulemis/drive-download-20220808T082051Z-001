using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spin : MonoBehaviour
{
    [SerializeField] Vector3 end;
    [SerializeField] float time = 5;
    [SerializeField] Ease ease = Ease.Linear;
    [SerializeField] Transform move; 

    Tween tween = null;
    Sequence mySequence = null;
    public void Start()
    {
        // Grab a free Sequence to use
        mySequence = DOTween.Sequence();
        mySequence.Append(move.DOLocalRotate(end, time, RotateMode.FastBeyond360).SetEase(ease));
        mySequence.SetLoops(-1, LoopType.Restart).SetEase(ease);
    }
}
