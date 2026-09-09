using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : GameScreen
{
	public Text placeholderText;
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
