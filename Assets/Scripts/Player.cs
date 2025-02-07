using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameManager gameManager;
	public Animator playeranim;

	private CapsuleCollider2D playerCollider;
	private Vector2 normalColliderSize;
	private Vector2 crouchColliderSize;
	private Vector2 normalColliderOffset;
	private Vector2 crouchColliderOffset;
	private bool isCrouching = false;
	private float normalColliderHeight;
	private float crouchColliderHeight = 0.5f; 
	private float moveSpeed = 8f;
	private float jumpForce = 15f;
	private float dashRange = 3f;
	private float dashDuration = 0.2f;
	private float dashSpeed = 40f;
	private float fallSpeedIncrease = 20f;
	private Rigidbody2D rb;
	public bool isGrounded;
	private bool isDashing;
	private bool canDash = true;
	private bool hasDashedInAir = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.interpolation = RigidbodyInterpolation2D.Interpolate;

		playerCollider = GetComponent<CapsuleCollider2D>();
		normalColliderSize = playerCollider.size;
		normalColliderOffset = playerCollider.offset;
		float heightReduction = normalColliderSize.y * 0.5f;
		crouchColliderSize = new Vector2(normalColliderSize.x, normalColliderSize.y - heightReduction);
		crouchColliderOffset = new Vector2(normalColliderOffset.x, normalColliderOffset.y - (heightReduction / 2f)); // Move collider downward
	}

	void Update()
	{
		if (isDashing) return;

		float moveInput = Input.GetAxis("Horizontal");

		// Handle crouch input
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			Crouch();
		}
		else
		{
			StandUp();
		}

		if (!isCrouching && CanMove(Vector2.right * moveInput))
		{
			rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

			// Flip player based on movement direction
			if (moveInput > 0)
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
			else if (moveInput < 0)
			{
				transform.localScale = new Vector3(-1, 1, 1);
			}
		}

		if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			playeranim.Play("Jump", -1, 0f);
			isGrounded = false;
		}

		if (canDash && Input.GetKeyUp(KeyCode.LeftShift) && !isCrouching &&
			(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
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

		if (rb.velocity.y < 0 && isGrounded == false) // Falling
		{
			playeranim.SetBool("Fall", true); // Set the Fall animation to true
		}
		else if (rb.velocity.y >= 0 && isGrounded == false) // Not falling (either jumping or idle)
		{
			playeranim.SetBool("Fall", false); // Stop the Fall animation if the player is not falling
		}
	}

	private bool CanMove(Vector2 direction)
	{
		float checkDistance = 0.2f; // Adjust based on player size
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance, LayerMask.GetMask("Ground"));
		return hit.collider == null; // True if no obstacle ahead
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
			// Loop through all contact points
			foreach (ContactPoint2D contact in collision.contacts)
			{
				// Check if the normal is pointing upward (landed from above)
				if (contact.normal.y > 0.5f) // 0.5 allows slight slopes
				{
					playeranim.Play("HitGround", -1, 0f);
					isGrounded = true;
					hasDashedInAir = false;
					return; // Exit loop early to prevent unnecessary checks
				}
			}
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			GameManager.instance.RespawnPlayer(this);
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			foreach (ContactPoint2D contact in collision.contacts)
			{
				if (contact.normal.y > 0.5f) // Ensure player is touching the top of the ground
				{
					isGrounded = true;
					return; // Exit loop early
				}
			}
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
		playeranim.Play("Dash", -1, 0f);
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

	private void Crouch()
	{
		if (!isCrouching)
		{
			isCrouching = true;
			moveSpeed /= 2; // Reduce speed
			playerCollider.size = crouchColliderSize; // Shrink collider
			playerCollider.offset = crouchColliderOffset; // Move collider down
			playeranim.SetBool("Crouch", true);
		}
	}

	private void StandUp()
	{
		if (isCrouching)
		{
			isCrouching = false;
			moveSpeed *= 2; // Reset speed
			playerCollider.size = normalColliderSize; // Restore size
			playerCollider.offset = normalColliderOffset; // Reset position
			playeranim.SetBool("Crouch", false);
			playeranim.Play("Stand", -1, 0f);
		}
	}
}
