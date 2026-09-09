using UnityEngine;

public class GameScreen : MonoBehaviour
{
	public virtual void Show()
	{
		gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
	}

	public bool IsVisible
	{
		get { return gameObject.activeSelf; }
	}
}
