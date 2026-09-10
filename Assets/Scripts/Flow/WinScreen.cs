using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : GameScreen
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
		Wire("NextBtn", delegate
		{
			UiMotion.Punch(UiMotion.Find(transform, "NextBtn"));
			if (GameFlowController.instance != null)
				GameFlowController.instance.GoToNextNode();
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
			UiMotion.CelebratePopup(card, StartIdleMotion);
		else
			base.PlayShowMotion();

		UiMotion.TitlePop(UiMotion.Find(transform, "Title"), 0.22f);
		UiMotion.TitlePop(UiMotion.Find(transform, "Level"), 0.34f);
		UiMotion.TitlePop(UiMotion.Find(transform, "StarsLabel"), 0.72f);
		UiMotion.DropIn(UiMotion.Find(transform, "ScoreBox"), 160f, 0.58f, 8f);
		for (int i = 0; i < 3; i++)
			UiMotion.PopStar(UiMotion.Find(transform, "Star" + i), 0.48f + i * 0.16f);
		UiMotion.TossIn(UiMotion.Find(transform, "MapBtn"), -280f, 0.68f);
		UiMotion.TossIn(UiMotion.Find(transform, "NextBtn"), 280f, 0.76f);
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
