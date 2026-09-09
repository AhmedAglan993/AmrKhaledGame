using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : GameScreen
{
	public TMP_Text placeholderText;
	public VideoPlayer videoPlayer;
	public RawImage videoImage;
	public RenderTexture videoRt;

	void Awake()
	{
		ScreenHud.Wire(transform);
		Button close = FindButton("CloseVideo");
		if (close != null)
		{
			close.onClick.RemoveAllListeners();
			close.onClick.AddListener(Close);
		}

		if (videoPlayer != null)
		{
			videoPlayer.playOnAwake = false;
			videoPlayer.isLooping = false;
			if (videoRt != null)
			{
				videoPlayer.targetTexture = videoRt;
				if (videoImage != null)
					videoImage.texture = videoRt;
			}
		}
	}

	public void Play(VideoClip clip)
	{
		Show();
		if (placeholderText != null)
		{
			placeholderText.gameObject.SetActive(clip == null);
			placeholderText.text = clip == null
				? "Assign a VideoClip on this node in the Inspector.\nTap CLOSE when you are done."
				: "";
		}

		if (videoPlayer == null)
			return;

		videoPlayer.Stop();
		videoPlayer.clip = clip;
		videoPlayer.loopPointReached -= OnFinished;
		if (clip != null)
		{
			videoPlayer.loopPointReached += OnFinished;
			videoPlayer.Play();
		}
	}

	public void Close()
	{
		if (videoPlayer != null)
			videoPlayer.Stop();
		if (GameFlowController.instance != null)
			GameFlowController.instance.OnVideoClosed();
	}

	protected override void PlayShowMotion()
	{
		UiMotion.Kill(transform);
		transform.localScale = Vector3.one;
		RectTransform image = UiMotion.Find(transform, "VideoImage");
		if (image != null)
			UiMotion.Popup(image, 0.38f, null);
		else
			UiMotion.FadeIn(gameObject, 0.3f, null);
		RectTransform close = UiMotion.Find(transform, "CloseVideo");
		if (close != null)
		{
			UiMotion.Prepare(close);
			close.localScale = Vector3.one * 0.6f;
			close.DOScale(1f, 0.35f).SetDelay(0.15f).SetEase(Ease.OutBack).SetUpdate(true);
		}
		DOVirtual.DelayedCall(0.4f, StartIdleMotion).SetUpdate(true).SetLink(gameObject);
	}

	void OnFinished(VideoPlayer source)
	{
		if (GameFlowController.instance != null)
			GameFlowController.instance.OnVideoFinished();
	}

	Button FindButton(string name)
	{
		Button[] buttons = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i].name == name)
				return buttons[i];
		}
		return null;
	}
}
