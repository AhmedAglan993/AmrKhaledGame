using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager
{
	private int score = 0;
	private int throws = 0;
	public static ScoreManager instance;

	public static ScoreManager GetInstance()
	{
		if (instance == null)
			instance = new ScoreManager();

		return instance;
	}

	public void UpdateScoreUI()
	{
		GameObject scoreGo = GameObject.FindWithTag("Score");
		if (scoreGo == null)
			return;
		Text _score = scoreGo.GetComponent<Text>();
		if (_score != null)
			_score.text = "Score: " + score;
	}

	public void AddScore(int score)
	{
		this.score += score;
		UpdateScoreUI();
	}

	public int GetScore()
	{
		return score;
	}

	public void AddThrows()
	{
		throws++;
	}

	public int GetThrows()
	{
		return throws;
	}

	public void Reset()
	{
		score = 0;
		throws = 0;
		UpdateScoreUI();
	}
}
