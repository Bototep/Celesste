using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private int checkpointIndex; 
	private void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag("Player")) 
		{
			GameManager.instance.SetRespawnPoint(checkpointIndex);
		}
	}
}