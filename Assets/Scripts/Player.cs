using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
	const float tweenTime = 0.45f;
	const float baseTall = 0.2f;
	const float baseWeight = 0.2f;
	[SerializeField] string calculation;
	[SerializeField] Animator animator;
	public Animator Animator => animator;
	[SerializeField] SplineController splineController;
	[SerializeField] SkinnedMeshRenderer[] skinnedMeshes;
	[SerializeField] Rigidbody head;
	public Rigidbody Head => head;
	CharacterController capsule;

	public float Weight { get; private set; } = baseWeight;
	public float Tall { get; private set; } = baseTall;
	public float Power => (Weight + Tall) / 2f;
	public bool Dead => Weight <= 0 && Tall <= 0.1f;

	private void Awake()
	{
		capsule = transform.parent.GetComponentInParent<CharacterController>();
	}

	public void CalculateTall(string calculation)
	{
		if(string.IsNullOrEmpty(calculation))
		{
			return;
		}
		try
		{
			DataTable dt = new DataTable();
			Tall = System.Convert.ToInt32(dt.Compute($"({Tall * 100}){calculation}", "")) / 100f;
			UpdateTall(Mathf.LerpUnclamped(-0.2f, 1f, Tall));
		} catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}

	void UpdateCollider()
	{
		var local = transform.InverseTransformPoint(head.transform.position);
		capsule.height = local.y;
		capsule.center = new Vector3(capsule.center.x, local.y / 2f, capsule.center.z);
	}

	public void CalculateWeight(string calculation)
	{
		if (string.IsNullOrEmpty(calculation))
		{
			return;
		}
		try
		{
			DataTable dt = new DataTable();
			Weight = System.Convert.ToInt32(dt.Compute($"{Weight * 100f}{calculation}", "")) / 100f;
			UpdateWeight(Mathf.LerpUnclamped(-20f, 100f, Weight));
		}
		catch (System.Exception e)
		{
			Debug.LogError(e);
		}
	}

	Tween tallTween = null;
	void UpdateTall(float newTall)
	{
		float start = splineController.tall; ;
		if (tallTween != null)
		{
			tallTween.Kill();
		}
		tallTween = DOVirtual.Float(start, newTall, tweenTime, (value) =>
		{
			splineController.tall = value;
		}).OnComplete(delegate
		{
			UpdateCollider();
			CheckDie();
		});

		if (Dead)
		{
			FinishLine.Stop();
			animator.SetTrigger("fall");
		}
	}

	Tween weightTween = null;
	void UpdateWeight(float newWeight)
	{
		float start = skinnedMeshes.Length > 0 ? skinnedMeshes[0].GetBlendShapeWeight(0) : 0;
		if(weightTween != null)
		{
			weightTween.Kill();
		}
		weightTween = DOVirtual.Float(start, newWeight, tweenTime, (value) =>
		{
			foreach (var skin in skinnedMeshes)
			{
				skin.SetBlendShapeWeight(0, value);
			}
		}).OnComplete(CheckDie);

		if (Dead)
		{
			FinishLine.Stop();
			animator.SetTrigger("fall");
		}
	}

	bool lose = false;
	public void CheckDie()
	{
		if (!Dead) return;
		if(!lose)
		{
			lose = true;
			SceneMaster.Instance.OpenScene(SceneID.Result);
		}
		
		PlayerController.CanSwipe = false;
		
		head.transform.parent = null;
		head.isKinematic = false;
		head.AddForceAtPosition(head.transform.forward * Random.Range(300, 650), head.transform.position + Random.insideUnitSphere);
		float start = skinnedMeshes.Length > 0 ? skinnedMeshes[0].GetBlendShapeWeight(0) : 0;
		if (weightTween != null)
		{
			weightTween.Kill();
		}
		weightTween = DOVirtual.Float(start, -30f, tweenTime, (value) =>
		{
			foreach (var skin in skinnedMeshes)
			{
				skin.SetBlendShapeWeight(0, value);
			}
		}).SetEase(Ease.Linear).OnComplete(delegate
		{
			foreach (var skin in skinnedMeshes)
			{
				skin.gameObject.SetActive(false);
			}
		});
	}

	public SkinnedMeshRenderer GetBone()
	{
		return skinnedMeshes[0];
	}
}
