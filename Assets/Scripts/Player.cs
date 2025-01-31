using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float jumpForce = 10f;
	public float dashRange = 5f; // The distance the player will travel during the dash
	public float dashDuration = 0.5f; // The time it takes to complete the dash
	public float dashSpeed = 10f; // Fixed dash speed
	public float fallSpeedIncrease = 5f; // Amount to increase fall speed when pressing S
	public LayerMask groundLayer;

	private Rigidbody2D rb;
	private bool isGrounded;
	private bool isDashing;
	private bool canDash = true; // Determines if the player can dash
	private bool hasDashedInAir = false; // Ensures the player can only dash once in the air

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		// If the player is dashing, skip movement input
		if (isDashing)
		{
			// While dashing, we do nothing
			return;
		}

		float moveInput = Input.GetAxis("Horizontal");
		rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

		// Check if player is grounded and can jump
		Debug.Log("Is Grounded: " + isGrounded);
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			Debug.Log("Jumping!");
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			isGrounded = false;
		}

		// Check for dash input (Shift key release)
		if (canDash && Input.GetKeyUp(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
		{
			// Dash only if on the ground or not yet dashed in the air
			if (isGrounded || !hasDashedInAir)
			{
				StartCoroutine(Dash(moveInput));
			}
		}

		// If the player is in the air and presses 'S', increase the fall speed
		if (Input.GetKey(KeyCode.S))
		{
			IncreaseFallSpeed();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (((1 << collision.gameObject.layer) & groundLayer) != 0)
		{
			isGrounded = true;
			hasDashedInAir = false; // Reset air dash when touching the ground
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (((1 << collision.gameObject.layer) & groundLayer) != 0)
		{
			isGrounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (((1 << collision.gameObject.layer) & groundLayer) != 0)
		{
			isGrounded = false;
		}
	}

	// Dash Coroutine
	private IEnumerator Dash(float moveInput)
	{
		canDash = false; // Disable dash input while dashing
		isDashing = true;
		float startTime = Time.time;

		// Calculate the dash direction (based on movement or keys pressed)
		float dashDirection = moveInput != 0 ? moveInput : (Input.GetKey(KeyCode.D) ? 1f : -1f);

		// Set the velocity based on the fixed dash speed and direction
		rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);

		// Wait for the dash duration (covering fixed range)
		float totalDashTime = dashRange / dashSpeed; // Time required to cover the dashRange at dashSpeed

		// Wait until the dash duration is complete
		while (Time.time < startTime + totalDashTime)
		{
			yield return null;
		}

		// Reset the dash state and allow the player to dash again
		isDashing = false;
		canDash = true;

		// If the player was in the air, mark that they have dashed in the air
		if (!isGrounded)
		{
			hasDashedInAir = true;
		}
	}

	// Increase the fall speed when pressing S
	private void IncreaseFallSpeed()
	{
		// Increase the vertical velocity to fall faster
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - fallSpeedIncrease * Time.deltaTime);
	}
}
