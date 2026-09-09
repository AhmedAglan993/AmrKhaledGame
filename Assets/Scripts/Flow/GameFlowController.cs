using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameFlowController : MonoBehaviour
{
	public static GameFlowController instance;

	const string ProgressKey = "MapProgress_v1";

	[Header("Map Nodes (types assigned randomly, videos stay empty until you drop clips here)")]
	public List<MapNodeData> nodes = new List<MapNodeData>();

	[Header("Questions")]
	public List<QuestionData> questions = new List<QuestionData>();

	[Header("Shared Sprites")]
	public Sprite homeSprite;
	public Sprite soundSprite;
	public Sprite pauseSprite;
	public Sprite scorePanelSprite;
	public Sprite screenBackgroundSprite;

	[Header("Map Sprites")]
	public Sprite mapBackgroundSprite;
	public Sprite nodeOpenSprite;
	public Sprite nodeCurrentSprite;
	public Sprite lockIconSprite;
	public Sprite starOnSprite;
	public Sprite starOffSprite;
	public Sprite titleBannerSprite;

	[Header("Q&A Sprites")]
	public Sprite challengePanelSprite;
	public Sprite answerButtonSprite;

	[Header("Result Sprites")]
	public Sprite resultPanelSprite;
	public Sprite primaryButtonSprite;
	public Sprite secondaryButtonSprite;

	[HideInInspector] public int currentNodeIndex;
	[HideInInspector] public int unlockedCount = 1;
	[HideInInspector] public int[] nodeStars;

	[Header("Scene UI (persistent)")]
	public GameObject flowCanvas;
	public GameObject mapPanel;
	public GameObject questionPanel;
	public GameObject videoPanel;
	public GameObject winPanel;
	public GameObject losePanel;
	public GameObject gameHud;
	public Transform mapNodesRoot;

	[Header("Map UI")]
	public Text mapScoreText;

	[Header("Question UI")]
	public Text questionScoreText;
	public Text questionTitleText;
	public Text questionProgressText;
	public Text questionBodyText;
	public Button[] answerButtons;
	public Image[] answerStars;

	[Header("Video UI")]
	public Text videoPlaceholderText;
	public VideoPlayer videoPlayer;
	public RawImage videoImage;
	public RenderTexture videoRt;

	[Header("Win UI")]
	public Text winScoreText;
	public Text winStarsText;
	public Text winLevelText;

	[Header("Lose UI")]
	public Text loseScoreText;
	public Text loseStarsText;
	public Text loseLevelText;

	[Header("Game HUD")]
	public Text gameHudLevelText;
	public Text gameHudScoreText;

	int pendingStars;
	bool questionLocked;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}

	void Start()
	{
		EnsureData();
		LoadProgress();
		if (flowCanvas == null)
			Debug.LogError("GameFlowController: FlowCanvas is missing from the scene. UI must live in the scene, not be created at runtime.");
		else
			WirePersistentButtons();
		HideLegacyMenus();
		ShowMap();
	}

	void Update()
	{
		if (gameHud != null && gameHud.activeSelf && gameHudScoreText != null && ScoreManager.instance != null)
			gameHudScoreText.text = "SCORE  " + ScoreManager.GetInstance().GetScore();
	}

	public static void EnsureSampleQuestions(List<QuestionData> list)
	{
		if (list == null)
			return;
		list.Clear();
		list.Add(MakeQuestion(
			"Which action helps protect our environment?",
			new[] { "Save water", "Leave the lights on", "Throw rubbish on the ground", "Waste water" },
			0));
		list.Add(MakeQuestion(
			"What should you do with empty plastic bottles?",
			new[] { "Recycle them", "Burn them", "Throw them in a river", "Bury them in the garden" },
			0));
		list.Add(MakeQuestion(
			"Which of these is a renewable energy source?",
			new[] { "Coal", "Oil", "Solar power", "Diesel" },
			2));
		list.Add(MakeQuestion(
			"Why should we plant more trees?",
			new[] { "They make more noise", "They clean the air", "They waste water", "They block all sunlight" },
			1));
	}

	public static void GenerateRandomNodes(List<MapNodeData> list)
	{
		if (list == null)
			return;
		list.Clear();

		var types = new List<MapNodeType>();
		for (int i = 0; i < 10; i++)
			types.Add(MapNodeType.BubbleLevel);
		for (int i = 0; i < 4; i++)
			types.Add(MapNodeType.Question);
		for (int i = 0; i < 2; i++)
			types.Add(MapNodeType.Video);

		var rng = new System.Random(20260910);
		for (int i = types.Count - 1; i > 0; i--)
		{
			int j = rng.Next(i + 1);
			MapNodeType tmp = types[i];
			types[i] = types[j];
			types[j] = tmp;
		}

		int level = 0;
		int quiz = 0;
		int video = 0;
		for (int i = 0; i < types.Count; i++)
		{
			var node = new MapNodeData { type = types[i] };
			switch (types[i])
			{
				case MapNodeType.BubbleLevel:
					node.levelIndex = level;
					node.displayName = "Level " + (level + 1);
					level++;
					break;
				case MapNodeType.Question:
					node.questionIndex = quiz;
					node.displayName = "Quiz " + (quiz + 1);
					quiz++;
					break;
				case MapNodeType.Video:
					node.displayName = "Video " + (video + 1);
					video++;
					break;
			}
			list.Add(node);
		}
	}

	void EnsureData()
	{
		if (questions == null || questions.Count == 0)
			EnsureSampleQuestions(questions);
		if (nodes == null || nodes.Count == 0)
			GenerateRandomNodes(nodes);
		if (nodeStars == null || nodeStars.Length != nodes.Count)
			nodeStars = new int[nodes.Count];
		unlockedCount = Mathf.Clamp(unlockedCount, 1, nodes.Count);
	}

	static QuestionData MakeQuestion(string q, string[] answers, int correct)
	{
		return new QuestionData
		{
			question = q,
			answers = answers,
			correctIndex = correct
		};
	}

	void HideLegacyMenus()
	{
		if (GameManager.instance == null)
			return;
		if (GameManager.instance.startUI != null)
			GameManager.instance.startUI.SetActive(false);
		if (GameManager.instance.levelsUI != null)
			GameManager.instance.levelsUI.SetActive(false);
		if (GameManager.instance.WinMenu != null)
			GameManager.instance.WinMenu.SetActive(false);
		if (GameManager.instance.LoseMenu != null)
			GameManager.instance.LoseMenu.SetActive(false);
		GameManager.instance.gameState = "menu";
		if (GameManager.instance.shootScript != null)
			GameManager.instance.shootScript.canShoot = false;
	}

	public void ShowMap()
	{
		Time.timeScale = 1f;
		if (GameManager.instance != null)
		{
			GameManager.instance.gameState = "menu";
			if (GameManager.instance.WinMenu != null)
				GameManager.instance.WinMenu.SetActive(false);
			if (GameManager.instance.LoseMenu != null)
				GameManager.instance.LoseMenu.SetActive(false);
			if (GameManager.instance.shootScript != null)
				GameManager.instance.shootScript.canShoot = false;
		}
		SetPanels(map: true);
		RefreshMapNodes();
		int score = ScoreManager.GetInstance().GetScore();
		if (mapScoreText != null)
			mapScoreText.text = "SCORE  " + score;
	}

	public void ReturnHome()
	{
		if (LevelManager.instance != null)
			LevelManager.instance.ClearLevel();
		if (videoPlayer != null)
			videoPlayer.Stop();
		ShowMap();
	}

	public void OpenNode(int index)
	{
		if (index < 0 || index >= nodes.Count)
			return;
		if (index >= unlockedCount)
			return;

		currentNodeIndex = index;
		MapNodeData node = nodes[index];
		switch (node.type)
		{
			case MapNodeType.BubbleLevel:
				StartBubbleLevel(node.levelIndex);
				break;
			case MapNodeType.Question:
				ShowQuestion(node.questionIndex);
				break;
			case MapNodeType.Video:
				ShowVideo(node);
				break;
		}
	}

	void StartBubbleLevel(int levelIndex)
	{
		SetPanels(game: true);
		if (gameHudLevelText != null)
			gameHudLevelText.text = "LEVEL " + (levelIndex + 1);
		if (GameManager.instance != null)
			GameManager.instance.gameState = "play";
		LevelManager.instance.StartLevel(levelIndex);
	}

	void ShowQuestion(int questionIndex)
	{
		questionLocked = false;
		if (questions == null || questions.Count == 0)
			EnsureSampleQuestions(questions);
		questionIndex = Mathf.Clamp(questionIndex, 0, questions.Count - 1);
		QuestionData q = questions[questionIndex];

		SetPanels(question: true);
		if (questionTitleText != null)
			questionTitleText.text = nodes[currentNodeIndex].displayName.ToUpper();
		if (questionProgressText != null)
			questionProgressText.text = "QUESTION  " + (questionIndex + 1) + " / " + questions.Count;
		if (questionBodyText != null)
			questionBodyText.text = q.question;
		if (questionScoreText != null)
			questionScoreText.text = "SCORE  " + ScoreManager.GetInstance().GetScore();

		for (int i = 0; i < 4; i++)
		{
			string letter = ((char)('A' + i)).ToString();
			string answer = (q.answers != null && i < q.answers.Length) ? q.answers[i] : "";
			Text label = answerButtons[i].GetComponentInChildren<Text>();
			if (label != null)
				label.text = letter + "  -  " + answer;
			answerButtons[i].interactable = true;
			if (answerStars[i] != null)
			{
				answerStars[i].sprite = starOffSprite != null ? starOffSprite : starOnSprite;
				answerStars[i].color = new Color(1f, 1f, 1f, 0.35f);
			}
		}
	}

	void OnAnswerClicked(int answerIndex)
	{
		if (questionLocked)
			return;
		questionLocked = true;

		QuestionData q = questions[Mathf.Clamp(nodes[currentNodeIndex].questionIndex, 0, questions.Count - 1)];
		bool correct = answerIndex == q.correctIndex;
		pendingStars = correct ? 3 : 1;
		if (correct)
			ScoreManager.GetInstance().AddScore(50);

		for (int i = 0; i < 4; i++)
		{
			answerButtons[i].interactable = false;
			if (answerStars[i] == null)
				continue;
			bool highlight = i == q.correctIndex || i == answerIndex;
			answerStars[i].sprite = starOnSprite;
			answerStars[i].color = i == q.correctIndex
				? Color.white
				: (i == answerIndex ? new Color(1f, 0.4f, 0.4f) : new Color(1f, 1f, 1f, 0.2f));
			if (!highlight)
				answerStars[i].color = new Color(1f, 1f, 1f, 0.2f);
		}

		CompleteCurrentNode(pendingStars);
		Invoke(nameof(ShowMap), 1.1f);
	}

	void ShowVideo(MapNodeData node)
	{
		SetPanels(video: true);
		if (videoPlaceholderText != null)
		{
			videoPlaceholderText.gameObject.SetActive(node.videoClip == null);
			videoPlaceholderText.text = node.videoClip == null
				? "Assign a VideoClip on this node in the Inspector.\nTap CLOSE when you are done."
				: "";
		}

		if (videoPlayer == null)
			return;

		videoPlayer.Stop();
		videoPlayer.clip = node.videoClip;
		if (node.videoClip != null)
		{
			videoPlayer.Play();
			videoPlayer.loopPointReached -= OnVideoFinished;
			videoPlayer.loopPointReached += OnVideoFinished;
		}
	}

	void OnVideoFinished(VideoPlayer source)
	{
		CompleteCurrentNode(3);
		ShowMap();
	}

	public void CloseVideo()
	{
		if (videoPlayer != null)
			videoPlayer.Stop();
		CompleteCurrentNode(nodeStars[currentNodeIndex] > 0 ? nodeStars[currentNodeIndex] : 3);
		ShowMap();
	}

	public void ShowLevelWin()
	{
		int throws = ScoreManager.GetInstance().GetThrows();
		int stars = throws <= 12 ? 3 : throws <= 20 ? 2 : 1;
		pendingStars = stars;
		CompleteCurrentNode(stars);
		FillResult(winLevelText, winScoreText, winStarsText, stars);
		SetPanels(win: true);
		if (GameManager.instance != null && GameManager.instance.WinMenu != null)
			GameManager.instance.WinMenu.SetActive(false);
	}

	public void ShowLevelLose()
	{
		pendingStars = 0;
		FillResult(loseLevelText, loseScoreText, loseStarsText, 0);
		SetPanels(lose: true);
		if (GameManager.instance != null && GameManager.instance.LoseMenu != null)
			GameManager.instance.LoseMenu.SetActive(false);
	}

	void FillResult(Text level, Text score, Text stars, int starCount)
	{
		string name = nodes != null && currentNodeIndex < nodes.Count
			? nodes[currentNodeIndex].displayName.ToUpper()
			: "LEVEL";
		if (level != null)
			level.text = name;
		if (score != null)
			score.text = ScoreManager.GetInstance().GetScore().ToString();
		if (stars != null)
			stars.text = starCount + " / 3 STARS";
	}

	public void RetryCurrentNode()
	{
		OpenNode(currentNodeIndex);
	}

	public void GoToNextNode()
	{
		int next = currentNodeIndex + 1;
		if (next < nodes.Count && next < unlockedCount)
			OpenNode(next);
		else
			ShowMap();
	}

	void CompleteCurrentNode(int stars)
	{
		if (nodeStars == null || nodeStars.Length != nodes.Count)
			nodeStars = new int[nodes.Count];
		nodeStars[currentNodeIndex] = Mathf.Max(nodeStars[currentNodeIndex], stars);
		if (currentNodeIndex + 1 >= unlockedCount && currentNodeIndex + 1 <= nodes.Count)
			unlockedCount = Mathf.Min(nodes.Count, currentNodeIndex + 2);
		SaveProgress();
	}

	void SaveProgress()
	{
		var data = new MapProgress { unlockedCount = unlockedCount, stars = nodeStars };
		PlayerPrefs.SetString(ProgressKey, JsonUtility.ToJson(data));
		PlayerPrefs.Save();
	}

	void LoadProgress()
	{
		if (!PlayerPrefs.HasKey(ProgressKey))
			return;
		var data = JsonUtility.FromJson<MapProgress>(PlayerPrefs.GetString(ProgressKey));
		if (data == null)
			return;
		unlockedCount = Mathf.Max(1, data.unlockedCount);
		if (data.stars != null && data.stars.Length == nodes.Count)
			nodeStars = data.stars;
	}

	void SetPanels(bool map = false, bool question = false, bool video = false, bool win = false, bool lose = false, bool game = false)
	{
		if (mapPanel != null) mapPanel.SetActive(map);
		if (questionPanel != null) questionPanel.SetActive(question);
		if (videoPanel != null) videoPanel.SetActive(video);
		if (winPanel != null) winPanel.SetActive(win);
		if (losePanel != null) losePanel.SetActive(lose);
		if (gameHud != null) gameHud.SetActive(game);
		if (flowCanvas != null) flowCanvas.SetActive(true);
	}

	void RefreshMapNodes()
	{
		if (mapNodesRoot == null)
			return;
		for (int i = 0; i < mapNodesRoot.childCount; i++)
		{
			Transform child = mapNodesRoot.GetChild(i);
			bool unlocked = i < unlockedCount;
			bool current = i == Mathf.Min(unlockedCount - 1, nodes.Count - 1);
			Image img = child.GetComponent<Image>();
			if (img != null)
			{
				if (!unlocked)
					img.sprite = nodeOpenSprite != null ? nodeOpenSprite : img.sprite;
				else
					img.sprite = current && nodeCurrentSprite != null ? nodeCurrentSprite : nodeOpenSprite;
				img.color = unlocked ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
			}

			Transform lockT = child.Find("Lock");
			if (lockT != null)
				lockT.gameObject.SetActive(!unlocked);

			Text number = child.Find("Number") != null ? child.Find("Number").GetComponent<Text>() : null;
			if (number != null)
				number.gameObject.SetActive(unlocked);

			Transform stars = child.Find("Stars");
			if (stars != null)
			{
				stars.gameObject.SetActive(unlocked && nodeStars != null && i < nodeStars.Length && nodeStars[i] > 0);
				for (int s = 0; s < stars.childCount; s++)
				{
					Image starImg = stars.GetChild(s).GetComponent<Image>();
					if (starImg == null)
						continue;
					bool on = nodeStars != null && i < nodeStars.Length && s < nodeStars[i];
					starImg.color = on ? Color.white : new Color(1f, 1f, 1f, 0.25f);
				}
			}

			Button btn = child.GetComponent<Button>();
			if (btn != null)
				btn.interactable = unlocked;
		}
	}

	void WirePersistentButtons()
	{
		if (flowCanvas == null)
			return;

		WireNamedButtons(flowCanvas.transform, "Home", ReturnHome);
		WireNamedButtons(flowCanvas.transform, "Sound", delegate
		{
			if (GameManager.instance != null)
				GameManager.instance.ToggleMute();
		});
		WireNamedButtons(flowCanvas.transform, "Pause", delegate
		{
			if (GameManager.instance != null)
				GameManager.instance.ToggleGameState();
		});
		WireNamedButtons(flowCanvas.transform, "CloseVideo", CloseVideo);
		WireNamedButtons(flowCanvas.transform, "MapBtn", ReturnHome);
		WireNamedButtons(flowCanvas.transform, "NextBtn", GoToNextNode);
		WireNamedButtons(flowCanvas.transform, "RetryBtn", RetryCurrentNode);

		if (mapNodesRoot != null)
		{
			for (int i = 0; i < mapNodesRoot.childCount; i++)
			{
				Button btn = mapNodesRoot.GetChild(i).GetComponent<Button>();
				if (btn == null)
					continue;
				int captured = i;
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(delegate { OpenNode(captured); });
			}
		}

		if (answerButtons != null)
		{
			for (int i = 0; i < answerButtons.Length; i++)
			{
				if (answerButtons[i] == null)
					continue;
				int captured = i;
				answerButtons[i].onClick.RemoveAllListeners();
				answerButtons[i].onClick.AddListener(delegate { OnAnswerClicked(captured); });
			}
		}

		if (videoPlayer != null && videoRt != null && videoImage != null)
		{
			videoPlayer.targetTexture = videoRt;
			videoImage.texture = videoRt;
		}
	}

	static void WireNamedButtons(Transform root, string name, UnityEngine.Events.UnityAction action)
	{
		Button[] buttons = root.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i].name != name)
				continue;
			buttons[i].onClick.RemoveAllListeners();
			buttons[i].onClick.AddListener(action);
		}
	}

	public void BakeSceneUi()
	{
		if (flowCanvas != null)
			return;

		flowCanvas = new GameObject("FlowCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		Canvas canvas = flowCanvas.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 50;
		CanvasScaler scaler = flowCanvas.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1080, 1920);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 0.5f;
		UiBuilder.Stretch(flowCanvas);

		mapPanel = BuildMapPanel(flowCanvas.transform);
		questionPanel = BuildQuestionPanel(flowCanvas.transform);
		videoPanel = BuildVideoPanel(flowCanvas.transform);
		winPanel = BuildResultPanel(flowCanvas.transform, "WinPanel", "LEVEL COMPLETE!", true);
		losePanel = BuildResultPanel(flowCanvas.transform, "LosePanel", "TRY AGAIN!", false);
		gameHud = BuildGameHud(flowCanvas.transform);

		mapPanel.SetActive(true);
		questionPanel.SetActive(false);
		videoPanel.SetActive(false);
		winPanel.SetActive(false);
		losePanel.SetActive(false);
		gameHud.SetActive(false);
	}

	GameObject BuildSharedBackground(Transform parent)
	{
		return UiBuilder.Panel("Background", parent, screenBackgroundSprite, Color.white);
	}

	void AddTopHud(Transform parent, bool includePause, UnityEngine.Events.UnityAction onHome, string bannerTitle)
	{
		GameObject bar = new GameObject("TopHud", typeof(RectTransform));
		bar.transform.SetParent(parent, false);
		UiBuilder.AnchorTopStretch(bar.GetComponent<RectTransform>(), 180f, -24f);

		Button home = UiBuilder.MakeButton("Home", bar.transform, homeSprite, new Vector2(140, 140), new Vector2(-430, -90), "", 1, Color.white);
		home.onClick.AddListener(onHome);

		if (titleBannerSprite != null)
		{
			Image banner = UiBuilder.Image("TitleBanner", bar.transform, titleBannerSprite, new Vector2(420, 150), new Vector2(0, -90));
			if (!string.IsNullOrEmpty(bannerTitle))
				UiBuilder.Label("BannerTitle", banner.transform, bannerTitle, 40, UiBuilder.Brown, new Vector2(360, 70), new Vector2(0, -10)).fontStyle = FontStyle.Bold;
		}

		Button sound = UiBuilder.MakeButton("Sound", bar.transform, soundSprite, new Vector2(140, 140), new Vector2(330, -90), "", 1, Color.white);
		sound.onClick.AddListener(() =>
		{
			if (GameManager.instance != null)
				GameManager.instance.ToggleMute();
		});

		if (includePause)
		{
			Button pause = UiBuilder.MakeButton("Pause", bar.transform, pauseSprite, new Vector2(140, 140), new Vector2(470, -90), "", 1, Color.white);
			pause.onClick.AddListener(() =>
			{
				if (GameManager.instance != null)
					GameManager.instance.ToggleGameState();
			});
		}
	}

	Text AddScoreHud(Transform parent)
	{
		Button dummy = UiBuilder.MakeButton("ScoreBox", parent, scorePanelSprite, new Vector2(280, 110), new Vector2(-360, -860), "", 1, Color.white);
		dummy.interactable = false;
		RectTransform rt = dummy.GetComponent<RectTransform>();
		UiBuilder.AnchorBottomLeft(rt, new Vector2(300, 120), new Vector2(40, 40));
		return UiBuilder.Label("ScoreText", dummy.transform, "SCORE  0", 36, UiBuilder.Brown, new Vector2(280, 100), Vector2.zero);
	}

	GameObject BuildMapPanel(Transform parent)
	{
		GameObject panel = new GameObject("MapPanel", typeof(RectTransform));
		panel.transform.SetParent(parent, false);
		UiBuilder.Stretch(panel);

		BuildSharedBackground(panel.transform);

		GameObject scrollGo = new GameObject("MapScroll", typeof(RectTransform), typeof(ScrollRect));
		scrollGo.transform.SetParent(panel.transform, false);
		RectTransform scrollRt = UiBuilder.Stretch(scrollGo);
		scrollRt.offsetMin = new Vector2(0, 160);
		scrollRt.offsetMax = new Vector2(0, -200);

		GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
		viewport.transform.SetParent(scrollGo.transform, false);
		UiBuilder.Stretch(viewport);
		viewport.GetComponent<Image>().color = Color.white;
		viewport.GetComponent<Mask>().showMaskGraphic = false;

		GameObject content = new GameObject("Content", typeof(RectTransform));
		content.transform.SetParent(viewport.transform, false);
		RectTransform contentRt = content.GetComponent<RectTransform>();
		contentRt.anchorMin = new Vector2(0.5f, 0f);
		contentRt.anchorMax = new Vector2(0.5f, 0f);
		contentRt.pivot = new Vector2(0.5f, 0f);
		contentRt.sizeDelta = new Vector2(1080, 2800);
		contentRt.anchoredPosition = Vector2.zero;

		Image mapBg = UiBuilder.Image("MapArt", content.transform, mapBackgroundSprite != null ? mapBackgroundSprite : screenBackgroundSprite, new Vector2(1080, 2800), Vector2.zero);
		RectTransform mapRt = mapBg.rectTransform;
		mapRt.anchorMin = new Vector2(0.5f, 0.5f);
		mapRt.anchorMax = new Vector2(0.5f, 0.5f);
		mapRt.sizeDelta = new Vector2(1080, 2800);
		mapBg.preserveAspect = false;
		mapBg.raycastTarget = false;

		GameObject nodesGo = new GameObject("MapNodes", typeof(RectTransform));
		nodesGo.transform.SetParent(content.transform, false);
		RectTransform nodesRt = nodesGo.GetComponent<RectTransform>();
		nodesRt.anchorMin = new Vector2(0.5f, 0f);
		nodesRt.anchorMax = new Vector2(0.5f, 0f);
		nodesRt.pivot = new Vector2(0.5f, 0f);
		nodesRt.sizeDelta = new Vector2(1080, 2800);
		nodesRt.anchoredPosition = Vector2.zero;
		mapNodesRoot = nodesGo.transform;

		ScrollRect scroll = scrollGo.GetComponent<ScrollRect>();
		scroll.content = contentRt;
		scroll.viewport = viewport.GetComponent<RectTransform>();
		scroll.horizontal = false;
		scroll.vertical = true;
		scroll.movementType = ScrollRect.MovementType.Elastic;
		scroll.inertia = true;

		Vector2[] path =
		{
			new Vector2(-30, 220),
			new Vector2(220, 380),
			new Vector2(-200, 540),
			new Vector2(40, 700),
			new Vector2(250, 860),
			new Vector2(-230, 1020),
			new Vector2(-10, 1180),
			new Vector2(230, 1340),
			new Vector2(-210, 1500),
			new Vector2(20, 1660),
			new Vector2(240, 1820),
			new Vector2(-180, 1980),
			new Vector2(30, 2140),
			new Vector2(210, 2300),
			new Vector2(-90, 2460),
			new Vector2(70, 2600)
		};

		for (int i = 0; i < nodes.Count; i++)
		{
			Vector2 pos = i < path.Length ? path[i] : new Vector2(0, 220 + i * 160);
			CreateMapNodeButton(nodesGo.transform, i, pos);
		}

		AddTopHud(panel.transform, true, ReturnHome, "LEVELS");
		mapScoreText = AddScoreHud(panel.transform);
		return panel;
	}

	void CreateMapNodeButton(Transform parent, int index, Vector2 pos)
	{
		MapNodeData node = nodes[index];
		Sprite sprite = nodeOpenSprite;
		Button btn = UiBuilder.MakeButton("Node_" + index, parent, sprite, new Vector2(150, 150), pos, "", 1, Color.white);
		RectTransform nodeRt = btn.GetComponent<RectTransform>();
		nodeRt.anchorMin = new Vector2(0.5f, 0f);
		nodeRt.anchorMax = new Vector2(0.5f, 0f);
		nodeRt.pivot = new Vector2(0.5f, 0.5f);
		nodeRt.anchoredPosition = pos;
		int captured = index;
		btn.onClick.AddListener(() => OpenNode(captured));

		string shortLabel = node.type == MapNodeType.Question ? "Q" : node.type == MapNodeType.Video ? "V" : (node.levelIndex + 1).ToString();
		UiBuilder.Label("Number", btn.transform, shortLabel, 40, UiBuilder.Brown, new Vector2(120, 80), new Vector2(0, 6)).fontStyle = FontStyle.Bold;

		if (lockIconSprite != null)
		{
			Image lockImg = UiBuilder.Image("Lock", btn.transform, lockIconSprite, new Vector2(70, 70), Vector2.zero);
			lockImg.raycastTarget = false;
		}

		GameObject stars = new GameObject("Stars", typeof(RectTransform));
		stars.transform.SetParent(btn.transform, false);
		RectTransform starsRt = stars.GetComponent<RectTransform>();
		starsRt.sizeDelta = new Vector2(140, 40);
		starsRt.anchoredPosition = new Vector2(0, -70);
		for (int s = 0; s < 3; s++)
			UiBuilder.Image("Star" + s, stars.transform, starOnSprite, new Vector2(36, 36), new Vector2((s - 1) * 38, 0));
	}

	GameObject BuildQuestionPanel(Transform parent)
	{
		GameObject panel = new GameObject("QuestionPanel", typeof(RectTransform));
		panel.transform.SetParent(parent, false);
		UiBuilder.Stretch(panel);
		BuildSharedBackground(panel.transform);
		AddTopHud(panel.transform, true, ReturnHome, "QUIZ");

		questionTitleText = UiBuilder.Label("LevelTitle", panel.transform, "LEVEL 3", 48, UiBuilder.Brown, new Vector2(500, 80), new Vector2(0, 680));
		questionTitleText.fontStyle = FontStyle.Bold;
		questionProgressText = UiBuilder.Label("Progress", panel.transform, "QUESTION  1 / 4", 32, UiBuilder.Brown, new Vector2(500, 50), new Vector2(0, 610));

		Image card = UiBuilder.Image("ChallengeCard", panel.transform, challengePanelSprite != null ? challengePanelSprite : scorePanelSprite, new Vector2(920, 420), new Vector2(0, 250));
		card.preserveAspect = false;
		UiBuilder.Label("ChallengeLabel", card.transform, "CHALLENGE", 28, UiBuilder.Brown, new Vector2(400, 40), new Vector2(0, 170)).fontStyle = FontStyle.Bold;
		questionBodyText = UiBuilder.Label("QuestionBody", card.transform, "Question", 36, UiBuilder.Brown, new Vector2(820, 220), new Vector2(0, -20));

		answerButtons = new Button[4];
		answerStars = new Image[4];
		for (int i = 0; i < 4; i++)
		{
			int captured = i;
			Button btn = UiBuilder.MakeButton("Answer" + i, panel.transform, answerButtonSprite != null ? answerButtonSprite : scorePanelSprite, new Vector2(880, 120), new Vector2(0, -80 - i * 140), "A", 32, UiBuilder.Brown);
			btn.onClick.AddListener(() => OnAnswerClicked(captured));
			answerButtons[i] = btn;
			Image star = UiBuilder.Image("Star", btn.transform, starOffSprite != null ? starOffSprite : starOnSprite, new Vector2(48, 48), new Vector2(380, 0));
			star.raycastTarget = false;
			answerStars[i] = star;
		}

		questionScoreText = AddScoreHud(panel.transform);
		return panel;
	}

	GameObject BuildVideoPanel(Transform parent)
	{
		GameObject panel = new GameObject("VideoPanel", typeof(RectTransform));
		panel.transform.SetParent(parent, false);
		UiBuilder.Stretch(panel);

		Image bg = UiBuilder.Panel("VideoBg", panel.transform, null, Color.black).GetComponent<Image>();
		bg.raycastTarget = true;

		videoRt = new RenderTexture(1080, 1920, 0);
		GameObject rawGo = new GameObject("VideoImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
		rawGo.transform.SetParent(panel.transform, false);
		UiBuilder.Stretch(rawGo);
		videoImage = rawGo.GetComponent<RawImage>();
		videoImage.texture = videoRt;
		videoImage.color = Color.white;

		videoPlayer = panel.AddComponent<VideoPlayer>();
		videoPlayer.playOnAwake = false;
		videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		videoPlayer.targetTexture = videoRt;
		videoPlayer.isLooping = false;

		videoPlaceholderText = UiBuilder.Label("Placeholder", panel.transform, "", 36, Color.white, new Vector2(900, 300), Vector2.zero);

		Button close = UiBuilder.MakeButton("CloseVideo", panel.transform, primaryButtonSprite != null ? primaryButtonSprite : scorePanelSprite, new Vector2(360, 120), new Vector2(0, -780), "CLOSE", 40, Color.white);
		close.onClick.AddListener(CloseVideo);
		return panel;
	}

	GameObject BuildResultPanel(Transform parent, string name, string title, bool isWin)
	{
		GameObject panel = new GameObject(name, typeof(RectTransform));
		panel.transform.SetParent(parent, false);
		UiBuilder.Stretch(panel);

		Image dim = UiBuilder.Panel("Dim", panel.transform, null, new Color(0f, 0f, 0f, 0.45f)).GetComponent<Image>();
		dim.raycastTarget = true;

		Image card = UiBuilder.Image("Card", panel.transform, resultPanelSprite != null ? resultPanelSprite : scorePanelSprite, new Vector2(860, 1100), Vector2.zero);
		card.preserveAspect = false;

		Text titleText = UiBuilder.Label("Title", card.transform, title, 52, UiBuilder.Brown, new Vector2(760, 90), new Vector2(0, 430));
		titleText.fontStyle = FontStyle.Bold;
		Text levelText = UiBuilder.Label("Level", card.transform, "LEVEL 1", 32, UiBuilder.Brown, new Vector2(400, 50), new Vector2(0, 360));

		for (int s = 0; s < 3; s++)
			UiBuilder.Image("Star" + s, card.transform, starOnSprite, new Vector2(110, 110), new Vector2((s - 1) * 130, 220));

		Text starsText = UiBuilder.Label("StarsLabel", card.transform, "0 / 3 STARS", 30, UiBuilder.Brown, new Vector2(400, 40), new Vector2(0, 120));
		Image scoreBox = UiBuilder.Image("ScoreBox", card.transform, scorePanelSprite, new Vector2(420, 160), new Vector2(0, -20));
		UiBuilder.Label("ScoreCaption", scoreBox.transform, "SCORE", 24, UiBuilder.Brown, new Vector2(300, 40), new Vector2(0, 40));
		Text scoreText = UiBuilder.Label("ScoreValue", scoreBox.transform, "0", 48, UiBuilder.Brown, new Vector2(300, 70), new Vector2(0, -20));
		scoreText.fontStyle = FontStyle.Bold;

		Button mapBtn = UiBuilder.MakeButton("MapBtn", card.transform, secondaryButtonSprite != null ? secondaryButtonSprite : scorePanelSprite, new Vector2(280, 110), new Vector2(-180, -420), "MAP", 36, Color.white);
		mapBtn.onClick.AddListener(ReturnHome);

		if (isWin)
		{
			Button next = UiBuilder.MakeButton("NextBtn", card.transform, primaryButtonSprite != null ? primaryButtonSprite : scorePanelSprite, new Vector2(360, 120), new Vector2(170, -420), "NEXT LEVEL", 32, Color.white);
			next.onClick.AddListener(GoToNextNode);
			winLevelText = levelText;
			winScoreText = scoreText;
			winStarsText = starsText;
		}
		else
		{
			Button retry = UiBuilder.MakeButton("RetryBtn", card.transform, primaryButtonSprite != null ? primaryButtonSprite : scorePanelSprite, new Vector2(360, 120), new Vector2(170, -420), "TRY AGAIN", 32, Color.white);
			retry.onClick.AddListener(RetryCurrentNode);
			loseLevelText = levelText;
			loseScoreText = scoreText;
			loseStarsText = starsText;
		}

		return panel;
	}

	GameObject BuildGameHud(Transform parent)
	{
		GameObject panel = new GameObject("GameHud", typeof(RectTransform));
		panel.transform.SetParent(parent, false);
		UiBuilder.Stretch(panel);
		Image blocker = panel.AddComponent<Image>();
		blocker.color = new Color(0, 0, 0, 0);
		blocker.raycastTarget = false;

		AddTopHud(panel.transform, true, ReturnHome, "LEVEL");
		gameHudLevelText = UiBuilder.Label("LevelTitle", panel.transform, "LEVEL 1", 48, UiBuilder.Brown, new Vector2(480, 80), new Vector2(0, 780));
		gameHudLevelText.fontStyle = FontStyle.Bold;
		gameHudScoreText = AddScoreHud(panel.transform);
		return panel;
	}
}
