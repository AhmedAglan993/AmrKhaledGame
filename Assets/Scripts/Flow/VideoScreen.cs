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
			UiMotion.SqueezeIn(image, 0.05f);
		else
			UiMotion.FadeIn(gameObject, 0.25f, null);
		UiMotion.JellyPopup(UiMotion.Find(transform, "VideoBg"), 0f, 0.42f, null);
		RectTransform close = UiMotion.Find(transform, "CloseVideo");
		if (close != null)
		{
			UiMotion.Prepare(close);
			close.localScale = Vector3.zero;
			close.localEulerAngles = new Vector3(0f, 0f, -80f);
			Sequence closeSeq = DOTween.Sequence().SetUpdate(true).SetLink(close.gameObject).SetDelay(0.2f);
			closeSeq.Append(close.DOScale(1.18f, 0.32f).SetEase(Ease.OutBack, 2.2f));
			closeSeq.Join(close.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutElastic));
			closeSeq.Append(close.DOScale(1f, 0.12f).SetEase(Ease.OutQuad));
		}
		UiMotion.TitlePop(UiMotion.Find(transform, "Label"), 0.28f);
		DOVirtual.DelayedCall(0.65f, StartIdleMotion).SetUpdate(true).SetLink(gameObject);
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
