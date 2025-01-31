using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance; // Singleton instance
	public int score = 0; // Player's score
	public bool key;
	public GameObject finishLine;

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
		if(key)
		{
			finishLine.SetActive(true);
		}
	}

	public void AddScore(int amount)
	{
		score += amount;
		Debug.Log("Score: " + score); // Display score in console
	}
}
