using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionScreen : GameScreen
{
	public TMP_Text titleText;
	public TMP_Text progressText;
	public TMP_Text bodyText;
	public TMP_Text scoreText;
	public Button[] answerButtons;
	public Image[] answerStars;
	public Sprite starOnSprite;
	public Sprite starOffSprite;

	bool locked;

	void Awake()
	{
		ScreenHud.Wire(transform);
		if (answerButtons == null)
			return;
		for (int i = 0; i < answerButtons.Length; i++)
		{
			if (answerButtons[i] == null)
				continue;
			int index = i;
			answerButtons[i].onClick.RemoveAllListeners();
			answerButtons[i].onClick.AddListener(delegate { OnAnswerClicked(index); });
		}
	}

	public void ShowQuestion(string title, string progress, string body, string[] answers, int score)
	{
		locked = false;
		Show();
		if (titleText != null)
			titleText.text = title;
		if (progressText != null)
			progressText.text = progress;
		if (bodyText != null)
			bodyText.text = body;
		if (scoreText != null)
			scoreText.text = "SCORE  " + score;

		if (answerButtons == null)
			return;
		for (int i = 0; i < answerButtons.Length; i++)
		{
			string letter = ((char)('A' + i)).ToString();
			string answer = (answers != null && i < answers.Length) ? answers[i] : "";
			TMP_Text label = answerButtons[i].GetComponentInChildren<TMP_Text>(true);
			if (label != null)
				label.text = letter + "  -  " + answer;
			answerButtons[i].interactable = true;
			if (answerStars != null && i < answerStars.Length && answerStars[i] != null)
			{
				answerStars[i].sprite = starOffSprite != null ? starOffSprite : starOnSprite;
				answerStars[i].color = new Color(1f, 1f, 1f, 0.35f);
			}
		}
	}

	public void ShowAnswerResult(int selectedIndex, int correctIndex)
	{
		if (answerButtons == null)
			return;
		for (int i = 0; i < answerButtons.Length; i++)
		{
			answerButtons[i].interactable = false;
			if (answerStars == null || i >= answerStars.Length || answerStars[i] == null)
				continue;
			answerStars[i].sprite = starOnSprite;
			if (i == correctIndex)
			{
				answerStars[i].color = Color.white;
				UiMotion.Punch(answerStars[i].transform);
			}
			else if (i == selectedIndex)
				answerStars[i].color = new Color(1f, 0.4f, 0.4f);
			else
				answerStars[i].color = new Color(1f, 1f, 1f, 0.2f);
		}
	}

	protected override void PlayShowMotion()
	{
		UiMotion.Kill(transform);
		transform.localScale = Vector3.one;
		UiMotion.SlideIn(UiMotion.Find(transform, "TopHud") ?? UiMotion.Find(transform, "TopHud (1)"), new Vector2(0f, 160f), 0f, 0.35f);
		UiMotion.SlideIn(UiMotion.Find(transform, "ScoreBox"), new Vector2(0f, -140f), 0.04f, 0.35f);
		RectTransform card = UiMotion.Find(transform, "ChallengeCard");
		if (card != null)
			UiMotion.Popup(card, 0.4f, null);
		if (answerButtons != null)
		{
			for (int i = 0; i < answerButtons.Length; i++)
			{
				if (answerButtons[i] == null)
					continue;
				RectTransform rt = answerButtons[i].transform as RectTransform;
				UiMotion.Prepare(rt);
				Vector2 dest = rt.anchoredPosition;
				rt.anchoredPosition = dest + new Vector2(0f, -80f);
				rt.localScale = Vector3.one * 0.85f;
				rt.DOAnchorPos(dest, 0.32f).SetDelay(0.12f + i * 0.07f).SetEase(Ease.OutCubic).SetUpdate(true);
				rt.DOScale(1f, 0.32f).SetDelay(0.12f + i * 0.07f).SetEase(Ease.OutBack).SetUpdate(true);
			}
		}
		DOVirtual.DelayedCall(0.55f, StartIdleMotion).SetUpdate(true).SetLink(gameObject);
	}

	void OnAnswerClicked(int index)
	{
		if (locked)
			return;
		locked = true;
		if (answerButtons != null && index >= 0 && index < answerButtons.Length && answerButtons[index] != null)
			UiMotion.Punch(answerButtons[index].transform);
		if (GameFlowController.instance != null)
			GameFlowController.instance.OnQuestionAnswered(index);
	}
}
