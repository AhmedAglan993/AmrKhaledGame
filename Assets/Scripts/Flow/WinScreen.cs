using UnityEngine;
using UnityEngine.UI;

public class WinScreen : GameScreen
{
	public Text levelText;
	public Text scoreText;
	public Text starsText;

	void Awake()
	{
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
