using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float speed = 2f; 
	private bool facingRight = false; 
	private Rigidbody2D rb;

	void Start(){
		rb = GetComponent<Rigidbody2D>();
	}

	void Update(){

		rb.velocity = new Vector2(facingRight ? speed : -speed, rb.velocity.y);
	}

	private void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyWall")){
			Flip();
		}
	}

	void Flip(){
		facingRight = !facingRight; 
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip sprite
	}
}
