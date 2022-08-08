using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	CharacterController character;
	[SerializeField] float roadWidth = 5f;
	[SerializeField] float radius = 1f;
	[SerializeField] float speed = 5;
	public float Speed => speed;
	[SerializeField] float jumpVelocity = 20;
	[SerializeField] float jumpGravity = 9.8f;
	[SerializeField] float jumpSpeed = 10;
	const float jumpAngle = 90;
	[SerializeField] UnityEvent startRunEvent;
	[SerializeField] UnityEvent startIdleEvent;
	[SerializeField] UnityEvent startJumpEvent;

	private bool isTouchDown, flag, jumping = false;
	private float xPosition, yPosition, zPosition, jumpTime, jumpTotalTime, turnVelocity, targetAngle, angle, num = 0;
	Vector3 touchPosition, touchPositionOrigin, playerPositionOrigin, target, direction;
	public static bool CanSwipe { get; set; } = true;
	public bool CanJump { get; set; } = true;

	private void Awake()
	{
		character = GetComponent<CharacterController>();	
	}

	private void Start()
	{
		CanSwipe = true;
	}

	public void DoControl()
	{
		touchPosition = Vector3.one;
		flag = Input.GetMouseButton(0) && CanSwipe;
		if (flag)
		{
			touchPosition = Input.mousePosition;
		}

		if (!jumping && Input.GetMouseButtonDown(0))
		{
			startRunEvent?.Invoke();
		}

		if (!jumping && Input.GetMouseButtonUp(0))
		{
			startIdleEvent?.Invoke();
		}

		if (!flag)
		{
			isTouchDown = false;
		}
		else
		{
			touchPosition.z = 5.5f;
			Vector3 vector = Camera.main.ScreenToWorldPoint(touchPosition);
			vector.x *= 2.2f;
			if (!isTouchDown)
			{
				isTouchDown = true;
				touchPositionOrigin = vector;
				playerPositionOrigin = transform.localPosition;
			}

			num = Mathf.Clamp(playerPositionOrigin.x + (vector.x - touchPositionOrigin.x), -roadWidth + radius, roadWidth - radius);
			Vector3 position2 = transform.localPosition;
			position2.x = xPosition = Mathf.Lerp(position2.x, num, 45f * Time.smoothDeltaTime);
		}

		if(jumping)
		{
			zPosition = transform.localPosition.z + jumpSpeed * Time.deltaTime;
		} else
		{
			if (flag)
			{
				zPosition = transform.localPosition.z + speed * Time.deltaTime;
			} else
			{
				zPosition = transform.localPosition.z;
			}		
		}
		target = new Vector3(
			xPosition,
			yPosition,
			zPosition
		);

		direction = target - transform.position;
		character.Move(direction +  (!jumping ? Physics.gravity : Vector3.up * yPosition));
		targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
		angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, 0.3f);
		transform.rotation = Quaternion.Euler(0, angle, 0);

		if (jumpTime < jumpTotalTime)
		{
			jumpTime += Time.deltaTime;
		}
		if (jumping && jumpTime > jumpTotalTime)
		{
			if (CanJump)
			{
				jumpTime = jumpTotalTime;
			}

			jumping = false;
			if(Input.GetMouseButton(0))
			{
				startRunEvent?.Invoke();
			} else
			{
				startIdleEvent?.Invoke();
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}
		yPosition = jumpVelocity * Mathf.Sin(Mathf.Deg2Rad * jumpAngle) * jumpTime - 0.5f * jumpGravity * jumpTime * jumpTime;
	}

	public void Jump()
	{
		if (jumping)
		{
			return;
		}
		startJumpEvent?.Invoke();
		jumping = true;
		jumpTotalTime = (jumpVelocity * Mathf.Sin(Mathf.Deg2Rad * jumpAngle)) / jumpGravity * 2f;
		jumpTime = 0;
		SoundManager.Instance.PlaySFX("jump-02", 0.6f);
	}

	// Update is called once per frame
	void Update()
    {
		DoControl();
    }
}
