using UnityEngine;

public class GameScreen : MonoBehaviour
{
	public RectTransform motionRoot;

	public virtual void Show()
	{
		gameObject.SetActive(true);
		if (transform.localScale.sqrMagnitude < 0.0001f)
			transform.localScale = Vector3.one;
		PlayShowMotion();
	}

	public virtual void Hide()
	{
		UiMotion.Kill(transform);
		gameObject.SetActive(false);
	}

	public bool IsVisible
	{
		get { return gameObject.activeSelf; }
	}

	protected virtual void PlayShowMotion()
	{
		RectTransform root = motionRoot != null ? motionRoot : transform as RectTransform;
		if (root == null)
		{
			StartIdleMotion();
			return;
		}
		UiMotion.JellyPopup(root, 0f, 0.5f, StartIdleMotion);
	}

	protected virtual void StartIdleMotion()
	{
		ScreenHud.PlayIconIdle(transform);
	}
}
