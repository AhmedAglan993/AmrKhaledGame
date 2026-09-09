using UnityEngine;
using UnityEngine.UI;

public class QuestionScreen : GameScreen
{
	public Text titleText;
	public Text progressText;
	public Text bodyText;
	public Text scoreText;
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
			Text label = answerButtons[i].GetComponentInChildren<Text>();
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
				answerStars[i].color = Color.white;
			else if (i == selectedIndex)
				answerStars[i].color = new Color(1f, 0.4f, 0.4f);
			else
				answerStars[i].color = new Color(1f, 1f, 1f, 0.2f);
		}
	}

	void OnAnswerClicked(int index)
	{
		if (locked)
			return;
		locked = true;
		if (GameFlowController.instance != null)
			GameFlowController.instance.OnQuestionAnswered(index);
	}
}
