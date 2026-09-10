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
		UiMotion.DropIn(UiMotion.Find(transform, "TopHud"), 210f, 0f, 8f);
		UiMotion.TitlePop(UiMotion.Find(transform, "TitleBanner"), 0.16f);
		UiMotion.TitlePop(UiMotion.Find(transform, "LevelTitle"), 0.24f);
		UiMotion.TossIn(UiMotion.Find(transform, "ScoreBox"), -240f, 0.1f);
		DOVirtual.DelayedCall(0.7f, StartIdleMotion).SetUpdate(true).SetLink(gameObject);
	}
}
