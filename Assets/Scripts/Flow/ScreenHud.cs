using UnityEngine;
using UnityEngine.UI;

public static class ScreenHud
{
	public static void Wire(Transform root)
	{
		if (root == null)
			return;
		Button[] buttons = root.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			string name = buttons[i].name;
			if (name == "Home")
			{
				buttons[i].onClick.RemoveAllListeners();
				Transform target = buttons[i].transform;
				buttons[i].onClick.AddListener(delegate { UiMotion.Punch(target); OnHome(); });
			}
			else if (name == "Sound")
			{
				buttons[i].onClick.RemoveAllListeners();
				Transform target = buttons[i].transform;
				buttons[i].onClick.AddListener(delegate { UiMotion.Punch(target); OnSound(); });
			}
			else if (name == "Pause")
			{
				buttons[i].onClick.RemoveAllListeners();
				Transform target = buttons[i].transform;
				buttons[i].onClick.AddListener(delegate { UiMotion.Punch(target); OnPause(); });
			}
		}
	}

	public static void PlayIconIdle(Transform root)
	{
		UiMotion.PulseNamed(root, "Home", "Sound", "Pause", "MapBtn", "NextBtn", "RetryBtn", "CloseVideo");
	}

	static void OnHome()
	{
		if (GameFlowController.instance != null)
			GameFlowController.instance.ReturnHome();
	}

	static void OnSound()
	{
		if (GameManager.instance != null)
			GameManager.instance.ToggleMute();
	}

	static void OnPause()
	{
		if (GameManager.instance != null)
			GameManager.instance.ToggleGameState();
	}
}
