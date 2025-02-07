using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameManager gameManager;
	public Animator playeranim;
	public ParticleSystem DashVFX;
	private GameObject currentOneWayPlatform;

	[SerializeField] private CapsuleCollider2D playerOneWayCollider;
	private CapsuleCollider2D playerCollider;
	private Vector2 normalColliderSize;
	private Vector2 crouchColliderSize;
	private Vector2 normalColliderOffset;
	private Vector2 crouchColliderOffset;
	private bool isCrouching = false;
	private float moveSpeed = 8f;
	private float jumpForce = 16f;
	private float dashRange = 3f;
	private float dashSpeed = 40f;
	private float fallSpeedIncrease = 40f;
	private Rigidbody2D rb;
	public bool isGrounded;
	private bool isDashing;
	private bool canDash = true;
	private bool hasDashedInAir = false;

	void Start(){
		rb = GetComponent<Rigidbody2D>();
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.interpolation = RigidbodyInterpolation2D.Interpolate;

		playerCollider = GetComponent<CapsuleCollider2D>();
		normalColliderSize = playerCollider.size;
		normalColliderOffset = playerCollider.offset;
		float heightReduction = normalColliderSize.y * 0.5f;
		crouchColliderSize = new Vector2(normalColliderSize.x, normalColliderSize.y - heightReduction);
		crouchColliderOffset = new Vector2(normalColliderOffset.x, normalColliderOffset.y - (heightReduction / 2f));
	}

	void Update(){
		if (isDashing) return;

		float moveInput = Input.GetAxis("Horizontal");

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
			Crouch();
		}else{
			StandUp();
		}

		if (!isCrouching && CanMove(Vector2.right * moveInput)){
			rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

			if (moveInput > 0){
				transform.localScale = new Vector3(1, 1, 1);
			}else if (moveInput < 0){
				transform.localScale = new Vector3(-1, 1, 1);
			}
		}

		if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching){
			CreateDust();
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			playeranim.Play("Jump", -1, 0f);
			isGrounded = false;
		}

		if (canDash && Input.GetKeyUp(KeyCode.LeftShift) && !isCrouching &&
			(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))){
			if (isGrounded || !hasDashedInAir){
				StartCoroutine(Dash(moveInput));
			}
		}

		if (!isGrounded && Input.GetKey(KeyCode.S)){
			IncreaseFallSpeed();
		}

		if (rb.velocity.y < 0 && isGrounded == false) {
			playeranim.SetBool("Fall", true); 
		}else if (rb.velocity.y >= 0 && isGrounded == false){
			playeranim.SetBool("Fall", false); 
		}
	}

	private bool CanMove(Vector2 direction){
		float checkDistance = 0.2f; 
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance, LayerMask.GetMask("Ground"));
		return hit.collider == null; 
	}

	private void OnTriggerEnter2D(Collider2D collision){
		if (collision.gameObject.layer == LayerMask.NameToLayer("Coin")){
			GameManager.instance.AddScore(1);
			Destroy(collision.gameObject);
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Key")){
			GameManager.instance.key = true;
			Destroy(collision.gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
			foreach (ContactPoint2D contact in collision.contacts){
				if (contact.normal.y > 0.5f) {
					playeranim.Play("HitGround", -1, 0f);
					isGrounded = true;
					hasDashedInAir = false;
					return; 
				}
			}
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")){
			GameManager.instance.RespawnPlayer(this);
			gameManager.Die();
		}

		if (collision.gameObject.CompareTag("OneWayPlatform")){
			currentOneWayPlatform = collision.gameObject;
		}

		if (collision.gameObject.CompareTag("Finish")){
			gameManager.Finish();
			Destroy(gameObject); 
		}
	}

	private void OnCollisionStay2D(Collision2D collision){
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
			foreach (ContactPoint2D contact in collision.contacts){
				if (contact.normal.y > 0.5f){
					isGrounded = true;
					return; 
				}
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision){
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
			isGrounded = false;
		}

		if (collision.gameObject.CompareTag("OneWayPlatform")){
			currentOneWayPlatform = null;
		}
	}

	private IEnumerator Dash(float moveInput){
		CreateDust();
		canDash = false;
		isDashing = true;
		playeranim.Play("Dash", -1, 0f);

		float startTime = Time.time;
		float originalGravityScale = rb.gravityScale;

		rb.gravityScale = 0;
		rb.velocity = new Vector2(rb.velocity.x, 0);
		float dashDirection = moveInput != 0 ? moveInput : (Input.GetKey(KeyCode.D) ? 1f : -1f);
		rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
		float totalDashTime = dashRange / dashSpeed;
		while (Time.time < startTime + totalDashTime){
			yield return null;
		}

		rb.gravityScale = originalGravityScale;

		isDashing = false;
		canDash = true;

		if (!isGrounded){
			hasDashedInAir = true;
		}
	}


	private void IncreaseFallSpeed(){
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - fallSpeedIncrease * Time.deltaTime);
	}

	private void Crouch(){
		if (!isCrouching){
			isCrouching = true;
			moveSpeed /= 2; 
			playerCollider.size = crouchColliderSize; 
			playerCollider.offset = crouchColliderOffset;
			playeranim.SetBool("Crouch", true);
		}
	}

	private void StandUp(){
		if (isCrouching){
			isCrouching = false;
			moveSpeed *= 2; 
			playerCollider.size = normalColliderSize; 
			playerCollider.offset = normalColliderOffset; 
			playeranim.SetBool("Crouch", false);
			playeranim.Play("Stand", -1, 0f);
		}
	}

	void CreateDust(){
		DashVFX.Play();
	}
}
