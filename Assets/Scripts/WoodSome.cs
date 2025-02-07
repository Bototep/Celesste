using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSome : MonoBehaviour
{
	private BoxCollider2D platformCollider;

	void Start()
	{
		platformCollider = GetComponent<BoxCollider2D>();
	}

	void Update()
	{
		// Platform behavior is handled by the collision detection in OnCollisionStay2D and OnTriggerStay2D
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// If player comes from above, allow them to stand on the platform
		if (collision.gameObject.CompareTag("Player"))
		{
			foreach (ContactPoint2D contact in collision.contacts)
			{
				if (contact.normal.y > 0.5f) // Check if coming from above (normal points upwards)
				{
					// Player is on top, allow standing
					collision.gameObject.GetComponent<Player>().isGrounded = true;
				}
			}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		// If the player is coming from below, allow them to pass through
		if (collision.gameObject.CompareTag("Player"))
		{
			foreach (ContactPoint2D contact in collision.contacts)
			{
				if (contact.normal.y < -0.5f) // If coming from below (normal points downwards)
				{
					// Disable the collision temporarily to allow passing through
					platformCollider.enabled = false;
				}
				else
				{
					// Enable the collider again for standing
					platformCollider.enabled = true;
				}
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			// Once player leaves the platform, reset
			platformCollider.enabled = true;
		}
	}
}
