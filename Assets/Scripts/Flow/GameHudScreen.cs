using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameHudScreen : GameScreen
{
	public TMP_Text levelText;
	public TMP_Text scoreText;

	int lastScore = int.MinValue;

	void Awake()
	{
		ScreenHud.Wire(transform);
	}

	void Update()
	{
		if (!IsVisible || scoreText == null)
			return;
		int score = ScoreManager.GetInstance().GetScore();
		if (score == lastScore)
			return;
		lastScore = score;
		scoreText.text = "SCORE  " + score;
		if (score != 0)
			UiMotion.Punch(scoreText.transform);
	}

	public void SetLevelTitle(string title)
	{
		if (levelText != null)
			levelText.text = title;
	}

	protected override void PlayShowMotion()
	{
		UiMotion.Kill(transform);
		transform.localScale = Vector3.one;
		lastScore = int.MinValue;
		UiMotion.SlideIn(UiMotion.Find(transform, "TopHud"), new Vector2(0f, 180f), 0f, 0.38f);
		UiMotion.SlideIn(UiMotion.Find(transform, "ScoreBox"), new Vector2(0f, -160f), 0.06f, 0.38f);
		DOVirtual.DelayedCall(0.4f, StartIdleMotion).SetUpdate(true).SetLink(gameObject);
	}
}
