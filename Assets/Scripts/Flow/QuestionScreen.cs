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
				UiMotion.PopStar(answerStars[i].rectTransform, 0f);
				if (answerButtons[i] != null)
					answerButtons[i].transform.DOPunchRotation(new Vector3(0f, 0f, 14f), 0.4f, 10, 0.6f).SetUpdate(true);
			}
			else if (i == selectedIndex)
			{
				answerStars[i].color = new Color(1f, 0.4f, 0.4f);
				if (answerButtons[i] != null)
					answerButtons[i].transform.DOShakeRotation(0.35f, new Vector3(0f, 0f, 16f), 12, 90f, true).SetUpdate(true);
			}
			else
				answerStars[i].color = new Color(1f, 1f, 1f, 0.2f);
		}
	}

	protected override void PlayShowMotion()
	{
		UiMotion.Kill(transform);
		transform.localScale = Vector3.one;
		UiMotion.DropIn(UiMotion.Find(transform, "TopHud") ?? UiMotion.Find(transform, "TopHud (1)"), 200f, 0f, 8f);
		UiMotion.TitlePop(UiMotion.Find(transform, "LevelTitle"), 0.12f);
		UiMotion.TossIn(UiMotion.Find(transform, "ScoreBox"), -220f, 0.06f);
		UiMotion.TitlePop(UiMotion.Find(transform, "Progress"), 0.2f);
		RectTransform card = UiMotion.Find(transform, "ChallengeCard");
		if (card != null)
			UiMotion.JellyPopup(card, 0.08f, 0.5f, null);
		UiMotion.TitlePop(UiMotion.Find(transform, "ChallengeLabel"), 0.22f);
		RectTransform body = UiMotion.Find(transform, "QuestionBody");
		if (body != null)
			UiMotion.TitlePop(body, 0.3f);
		if (answerButtons != null)
		{
			for (int i = 0; i < answerButtons.Length; i++)
			{
				if (answerButtons[i] == null)
					continue;
				float fromX = (i % 2 == 0) ? -420f : 420f;
				UiMotion.TossIn(answerButtons[i].transform as RectTransform, fromX, 0.22f + i * 0.09f);
			}
		}
		DOVirtual.DelayedCall(0.85f, StartIdleMotion).SetUpdate(true).SetLink(gameObject);
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
