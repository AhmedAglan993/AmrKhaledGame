using UnityEngine;
using UnityEngine.UI;

public class GameHudScreen : GameScreen
{
	public Text levelText;
	public Text scoreText;

	void Awake()
	{
		ScreenHud.Wire(transform);
	}

	void Update()
	{
		if (!IsVisible || scoreText == null)
			return;
		scoreText.text = "SCORE  " + ScoreManager.GetInstance().GetScore();
	}

	public void SetLevelTitle(string title)
	{
		if (levelText != null)
			levelText.text = title;
	}
}
