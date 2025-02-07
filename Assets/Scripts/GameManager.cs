using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance; // Singleton instance

	public int score = 0; // Player's score
	public bool key;
	public GameObject finishLine;

	[SerializeField] private List<Transform> respawnPoints = new List<Transform>(); // List of respawn points
	private int currentCheckpointIndex = -1; // Track current checkpoint index

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Update()
	{
		if (key)
		{
			finishLine.SetActive(true);
		}
	}

	public void AddScore(int amount)
	{
		score += amount;
		Debug.Log("Score: " + score); // Display score in console
	}

	public void RespawnPlayer(Player player)
	{
		if (currentCheckpointIndex >= 0 && currentCheckpointIndex < respawnPoints.Count && player != null)
		{
			player.transform.position = respawnPoints[currentCheckpointIndex].position;  // Move player to last checkpoint
			player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;  // Reset velocity
		}
	}

	public void SetRespawnPoint(int checkpointIndex)
	{
		if (checkpointIndex > currentCheckpointIndex && checkpointIndex < respawnPoints.Count)
		{
			currentCheckpointIndex = checkpointIndex;
			Debug.Log("Respawn point updated to checkpoint: " + (currentCheckpointIndex + 1));
		}
	}
}
