#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public static class SplitFlowCanvases
{
	[MenuItem("Tools/Flow/Split Screens Into Canvases")]
	public static void Split()
	{
		GameObject flowCanvas = GameObject.Find("FlowCanvas");
		if (flowCanvas == null)
		{
			Debug.LogError("FlowCanvas not found.");
			return;
		}

		GameFlowController flow = Object.FindFirstObjectByType<GameFlowController>();
		Transform root = flowCanvas.transform;

		GameObject map = Promote(root, "MapPanel", "MapCanvas", 50);
		GameObject question = Promote(root, "QuestionPanel", "QuestionCanvas", 51);
		GameObject video = Promote(root, "VideoPanel", "VideoCanvas", 52);
		GameObject win = Promote(root, "WinPanel", "WinCanvas", 53);
		GameObject lose = Promote(root, "LosePanel", "LoseCanvas", 54);
		GameObject hud = Promote(root, "GameHud", "GameHudCanvas", 55);

		MapScreen mapScreen = map.GetComponent<MapScreen>();
		if (mapScreen == null)
			mapScreen = map.AddComponent<MapScreen>();
		mapScreen.nodesRoot = FindChild(map.transform, "MapNodes");
		mapScreen.scoreText = FindText(map.transform, "ScoreText");
		mapScreen.nodeOpenSprite = LoadSprite("Assets/Sprites/Map/AaCHte8xlwJgaa_iR6mt9w-AaCHtgIU2bEELHB-rfj9Kw.png");
		mapScreen.nodeCurrentSprite = LoadSprite("Assets/Sprites/Map/AaCHte8xlwJgaa_iR6mt9w-AaCHtgILa9SpcTtiWwC4JA.png");
		mapScreen.lockIconSprite = LoadSprite("Assets/Sprites/Map/AaCHte8xlwJgaa_iR6mt9w-AaCHtgJpcPqj0Sz3CnJ_NA.png");
		mapScreen.starOnSprite = LoadSprite("Assets/Sprites/Map/AaCH22NHnuh0kIhEW29tZw-AaCH229nnN8OPdIcdyVrnQ.png");

		QuestionScreen questionScreen = question.GetComponent<QuestionScreen>();
		if (questionScreen == null)
			questionScreen = question.AddComponent<QuestionScreen>();
		questionScreen.titleText = FindText(question.transform, "LevelTitle");
		questionScreen.progressText = FindText(question.transform, "Progress");
		questionScreen.bodyText = FindText(question.transform, "QuestionBody");
		questionScreen.scoreText = FindText(question.transform, "ScoreText");
		questionScreen.answerButtons = new Button[]
		{
			FindButton(question.transform, "Answer0"),
			FindButton(question.transform, "Answer1"),
			FindButton(question.transform, "Answer2"),
			FindButton(question.transform, "Answer3")
		};
		questionScreen.answerStars = new Image[]
		{
			FindImageUnder(questionScreen.answerButtons[0], "Star"),
			FindImageUnder(questionScreen.answerButtons[1], "Star"),
			FindImageUnder(questionScreen.answerButtons[2], "Star"),
			FindImageUnder(questionScreen.answerButtons[3], "Star")
		};
		questionScreen.starOnSprite = LoadSprite("Assets/Sprites/Map/AaCH22NHnuh0kIhEW29tZw-AaCH229nnN8OPdIcdyVrnQ.png");
		questionScreen.starOffSprite = LoadSprite("Assets/Sprites/WinScreen/AaCH_CIoRl129XyFHCNZsw-AaCH_DgVsb4QKzxTD8VMXg.png");

		VideoScreen videoScreen = video.GetComponent<VideoScreen>();
		if (videoScreen == null)
			videoScreen = video.AddComponent<VideoScreen>();
		videoScreen.placeholderText = FindText(video.transform, "Placeholder");
		videoScreen.videoPlayer = video.GetComponent<VideoPlayer>();
		if (videoScreen.videoPlayer == null)
			videoScreen.videoPlayer = video.GetComponentInChildren<VideoPlayer>(true);
		videoScreen.videoImage = video.GetComponentInChildren<RawImage>(true);
		videoScreen.videoRt = AssetDatabase.LoadAssetAtPath<RenderTexture>("Assets/Art/UI/VideoTarget.rt");

		WinScreen winScreen = win.GetComponent<WinScreen>();
		if (winScreen == null)
			winScreen = win.AddComponent<WinScreen>();
		winScreen.levelText = FindText(win.transform, "Level");
		winScreen.scoreText = FindText(win.transform, "ScoreValue");
		winScreen.starsText = FindText(win.transform, "StarsLabel");

		LoseScreen loseScreen = lose.GetComponent<LoseScreen>();
		if (loseScreen == null)
			loseScreen = lose.AddComponent<LoseScreen>();
		loseScreen.levelText = FindText(lose.transform, "Level");
		loseScreen.scoreText = FindText(lose.transform, "ScoreValue");
		loseScreen.starsText = FindText(lose.transform, "StarsLabel");

		GameHudScreen hudScreen = hud.GetComponent<GameHudScreen>();
		if (hudScreen == null)
			hudScreen = hud.AddComponent<GameHudScreen>();
		hudScreen.levelText = FindText(hud.transform, "LevelTitle");
		hudScreen.scoreText = FindText(hud.transform, "ScoreText");

		if (flow != null)
		{
			flow.mapScreen = mapScreen;
			flow.questionScreen = questionScreen;
			flow.videoScreen = videoScreen;
			flow.winScreen = winScreen;
			flow.loseScreen = loseScreen;
			flow.gameHudScreen = hudScreen;
			EditorUtility.SetDirty(flow);
		}

		map.SetActive(true);
		question.SetActive(false);
		video.SetActive(false);
		win.SetActive(false);
		lose.SetActive(false);
		hud.SetActive(false);

		Object.DestroyImmediate(flowCanvas);
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		EditorSceneManager.SaveOpenScenes();
		Debug.Log("Split FlowCanvas into 6 screen canvases.");
	}

	static GameObject Promote(Transform parent, string childName, string canvasName, int sort)
	{
		Transform child = parent.Find(childName);
		if (child == null)
			throw new System.Exception("Missing " + childName);
		child.SetParent(null, true);
		child.name = canvasName;
		GameObject go = child.gameObject;

		Canvas canvas = go.GetComponent<Canvas>();
		if (canvas == null)
			canvas = go.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = sort;
		canvas.overrideSorting = true;

		CanvasScaler scaler = go.GetComponent<CanvasScaler>();
		if (scaler == null)
			scaler = go.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1080, 1920);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 0.5f;

		if (go.GetComponent<GraphicRaycaster>() == null)
			go.AddComponent<GraphicRaycaster>();

		RectTransform rt = go.GetComponent<RectTransform>();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
		rt.localScale = Vector3.one;
		return go;
	}

	static Transform FindChild(Transform root, string name)
	{
		Transform[] all = root.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < all.Length; i++)
		{
			if (all[i].name == name)
				return all[i];
		}
		return null;
	}

	static Text FindText(Transform root, string name)
	{
		Text[] all = root.GetComponentsInChildren<Text>(true);
		for (int i = 0; i < all.Length; i++)
		{
			if (all[i].name == name)
				return all[i];
		}
		return null;
	}

	static Button FindButton(Transform root, string name)
	{
		Button[] all = root.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < all.Length; i++)
		{
			if (all[i].name == name)
				return all[i];
		}
		return null;
	}

	static Image FindImageUnder(Button button, string name)
	{
		if (button == null)
			return null;
		Transform t = button.transform.Find(name);
		return t != null ? t.GetComponent<Image>() : null;
	}

	static Sprite LoadSprite(string path)
	{
		return AssetDatabase.LoadAssetAtPath<Sprite>(path);
	}
}
#endif
