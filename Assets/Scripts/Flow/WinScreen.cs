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
			if (GameFlowController.instance != null)
				GameFlowController.instance.ReturnHome();
		});
		Wire("NextBtn", delegate
		{
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
			UiMotion.FadeIn(dim.gameObject, 0.22f, null);
		RectTransform card = motionRoot != null ? motionRoot : UiMotion.Find(transform, "Card");
		if (card != null)
			UiMotion.Popup(card, 0.48f, StartIdleMotion);
		else
			base.PlayShowMotion();

		for (int i = 0; i < 3; i++)
		{
			RectTransform star = UiMotion.Find(transform, "Star" + i);
			if (star == null)
				continue;
			UiMotion.Prepare(star);
			star.localScale = Vector3.zero;
			star.DOScale(1f, 0.35f).SetDelay(0.28f + i * 0.1f).SetEase(Ease.OutBack).SetUpdate(true);
		}
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
