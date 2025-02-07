using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private int checkpointIndex; // The index of this checkpoint

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player")) // Ensure it's the player
		{
			GameManager.instance.SetRespawnPoint(checkpointIndex);
		}
	}
}