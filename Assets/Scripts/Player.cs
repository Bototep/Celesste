using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
	public GameManager gameManager;
	public Transform respawnPoint;


	[SerializeField]
	private float moveSpeed = 5f;
	
	[SerializeField]
	private float dashRange = 4f;

	[SerializeField]
	private float dashDuration = 0.1f;

	[SerializeField]
	private float dashSpeed = 40f;

	[SerializeField]
	private float fallSpeedIncrease = 100f;
	
	[SerializeField]
	private float jumpForce = 10f;


	private Rigidbody2D rb;
	private bool isGrounded;
	private bool isDashing;
	private bool canDash = true;
	private bool hasDashedInAir = false;


	

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (isDashing)
		{
			return;
		}
		
		float moveInput = Input.GetAxis("Horizontal");
		rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
		

		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			isGrounded = false;
		}

		if (canDash && Input.GetKeyUp(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
		{
			if (isGrounded || !hasDashedInAir)
			{
				StartCoroutine(Dash(moveInput));
			}
		}

		if (!isGrounded && Input.GetKey(KeyCode.S))
		{
			IncreaseFallSpeed();
		}
		
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Coin"))
		{
			GameManager.instance.AddScore(1);
			Destroy(collision.gameObject);
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Key"))
		{
			GameManager.instance.key = true;
			Destroy(collision.gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			isGrounded = true;
			hasDashedInAir = false;
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			Respawn();
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			isGrounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			isGrounded = false;
		}
	}

	private IEnumerator Dash(float moveInput)
	{
		canDash = false;
		isDashing = true;
		float startTime = Time.time;

		float dashDirection = moveInput != 0 ? moveInput : (Input.GetKey(KeyCode.D) ? 1f : -1f);
		rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
		float totalDashTime = dashRange / dashSpeed;

		while (Time.time < startTime + totalDashTime)
		{
			yield return null;
		}

		isDashing = false;
		canDash = true;

		if (!isGrounded)
		{
			hasDashedInAir = true;
		}
	}

	
	private void IncreaseFallSpeed()
	{
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - fallSpeedIncrease * Time.deltaTime);
	}
	

	private void Respawn()
	{
		if (respawnPoint != null)
		{
			transform.position = respawnPoint.position;  // Set player position to respawn point
			rb.velocity = Vector2.zero;  // Reset velocity to avoid any lingering movement
		}
	}
}
