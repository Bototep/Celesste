using UnityEngine;

public class Key : MonoBehaviour
{
	public float floatSpeed = 1f; 
	public float floatHeight = 0.5f; 
	private Vector3 startPos;

	void Start(){
		startPos = transform.position;
	}

	void Update(){
		float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
		transform.position = new Vector3(startPos.x, newY, startPos.z);
	}
}
