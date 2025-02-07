using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public int score = 0;
	public int dieCount = 0;
	public bool key;
	public float elapsedTime = 0f; // Time counter

	public GameObject finishLine;
	public TMP_Text finish;
	public TMP_Text scoreTxt;
	public TMP_Text dieTxt;
	public TMP_Text timeTxt;

	[SerializeField] private List<Transform> respawnPoints = new List<Transform>();
	private int currentCheckpointIndex = -1;
	private bool isTimerRunning = true; // Controls timer state

	void Awake(){
		if (instance == null){
			instance = this;
		}else{
			Destroy(gameObject);
		}
	}

	void Update(){
		if (key){
			finishLine.SetActive(true);
		}

		scoreTxt.text = "Score: " + score;
		dieTxt.text = "Dead: " + dieCount;

		if (isTimerRunning){
			elapsedTime += Time.deltaTime;
		}

		timeTxt.text = "Time: " + FormatTime(elapsedTime);
	}

	public void AddScore(int amount){
		score += amount;
	}

	public void RespawnPlayer(Player player){
		if (currentCheckpointIndex >= 0 && currentCheckpointIndex < respawnPoints.Count && player != null){
			player.transform.position = respawnPoints[currentCheckpointIndex].position;
			player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
	}

	public void SetRespawnPoint(int checkpointIndex){
		if (checkpointIndex > currentCheckpointIndex && checkpointIndex < respawnPoints.Count){
			currentCheckpointIndex = checkpointIndex;
		}
	}

	public void RestartScene(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Finish(){
		finish.gameObject.SetActive(true);
		isTimerRunning = false; // Stop the timer when finishing
	}

	public void Die(){
		dieCount++;
	}

	private string FormatTime(float time){
		int seconds = Mathf.FloorToInt(time); // Get whole seconds
		int milliseconds = Mathf.FloorToInt((time - seconds) * 100); // Convert fraction to milliseconds (00-99)

		return string.Format("{0:00}:{1:00}", seconds, milliseconds);
	}

}
