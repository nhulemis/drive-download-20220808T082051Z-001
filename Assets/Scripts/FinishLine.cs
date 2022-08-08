using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishLine : MonoBehaviour
{
	[SerializeField] Transform jumpPoint;
	[SerializeField] Transform kickPoint;
	[SerializeField] Animator boss;
	[SerializeField] GameObject[] fx;
	[SerializeField] Transform minDistance;
	[SerializeField] Transform maxDistance;
	const float fallSpeed = 16;
	Collider col;
	public static bool Finished = false;

	private void Start()
	{
		col = GetComponent<Collider>();
		Finished = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.tag.Equals(GameConst.Data.playertag))
		{
			return;
		}

		Finished = true;
		finishCoroutine = StartCoroutine(IEFinish(other));
	}

	Tween rushTween = null;
	const float rushTime = 3.5f;
	Coroutine finishCoroutine = null;
	IEnumerator IEFinish(Collider other)
	{
		var playerController = other.GetComponent<PlayerController>();
		if (playerController == null) yield break;
		var player = other.GetComponentInChildren<Player>();
		if (player == null) yield break;
		Gem.Earned += 10 + Mathf.RoundToInt(player.Power * 100);
		playerController.enabled = false;
		rushTween = playerController.transform.DOMove(jumpPoint.position, rushTime)
			.SetEase(Ease.Linear);
		yield return new WaitForSeconds(rushTime);
		foreach (var f in fx)
		{
			f.gameObject.SetActive(true);
		}
		player.Animator.SetTrigger("kick");
		yield return playerController.transform.DOJump(kickPoint.position, 2, 1, 1.8f).SetEase(Ease.Linear).WaitForCompletion();
		boss.SetTrigger("knock");
		CameraController.Instance.SetTarget(boss.gameObject);
		CameraController.Instance.transform.DORotate(new Vector3(35, 0, 0), 0.5f);
		var fallPoint = Vector3.Lerp(minDistance.position + minDistance.forward * Random.Range(0, 15f), maxDistance.position, player.Power);
		var fallTime = Vector3.Distance(boss.transform.position, fallPoint) / fallSpeed;
		SoundManager.Instance.PlaySFX("block");
		yield return boss.transform.DOJump(
			fallPoint,
			3,
			1,
			fallTime).SetEase(Ease.Linear).WaitForCompletion();
		boss.SetTrigger("stop");
		SceneMaster.Instance.OpenScene(SceneID.Result);
	}

	static bool stop = false;
	private void Update()
	{
		if(stop)
		{
			stop = false;
			if(rushTween != null) rushTween.Kill();
			if (finishCoroutine != null) StopCoroutine(finishCoroutine);
		}
	}

	public static void Stop()
	{
		stop = true;
		Debug.LogError("Stop");
	}
}
