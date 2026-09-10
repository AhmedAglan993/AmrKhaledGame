using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : GameScreen
{
	public TMP_Text levelText;
	public TMP_Text scoreText;
	public TMP_Text starsText;

	void Awake()
	{
		if (motionRoot == null)
			motionRoot = UiMotion.Find(transform, "Card");
		Wire("MapBtn", delegate
		{
			UiMotion.Punch(UiMotion.Find(transform, "MapBtn"));
			if (GameFlowController.instance != null)
				GameFlowController.instance.ReturnHome();
		});
		Wire("RetryBtn", delegate
		{
			UiMotion.Punch(UiMotion.Find(transform, "RetryBtn"));
			if (GameFlowController.instance != null)
				GameFlowController.instance.RetryCurrentNode();
		});
	}

	public void ShowResult(string levelName, int score, int stars)
	{
		if (levelText != null)
			levelText.text = levelName;
		if (scoreText != null)
			scoreText.text = score.ToString();
		if (starsText != null)
			starsText.text = stars + " / 3 STARS";
		Show();
	}

	protected override void PlayShowMotion()
	{
		UiMotion.Kill(transform);
		transform.localScale = Vector3.one;
		RectTransform dim = UiMotion.Find(transform, "Dim");
		if (dim != null)
			UiMotion.FadeIn(dim.gameObject, 0.18f, null);
		RectTransform card = motionRoot != null ? motionRoot : UiMotion.Find(transform, "Card");
		if (card != null)
			UiMotion.OopsPopup(card, StartIdleMotion);
		else
			base.PlayShowMotion();

		UiMotion.TitlePop(UiMotion.Find(transform, "Title"), 0.18f);
		UiMotion.TitlePop(UiMotion.Find(transform, "Level"), 0.28f);
		UiMotion.TitlePop(UiMotion.Find(transform, "StarsLabel"), 0.58f);
		UiMotion.DropIn(UiMotion.Find(transform, "ScoreBox"), 160f, 0.48f, -8f);
		for (int i = 0; i < 3; i++)
			UiMotion.PopStar(UiMotion.Find(transform, "Star" + i), 0.4f + i * 0.14f);
		UiMotion.TossIn(UiMotion.Find(transform, "MapBtn"), -280f, 0.6f);
		UiMotion.TossIn(UiMotion.Find(transform, "RetryBtn"), 280f, 0.7f);
	}

	void Wire(string buttonName, UnityEngine.Events.UnityAction action)
	{
		Button[] buttons = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i].name != buttonName)
				continue;
			buttons[i].onClick.RemoveAllListeners();
			buttons[i].onClick.AddListener(action);
		}
	}
}
