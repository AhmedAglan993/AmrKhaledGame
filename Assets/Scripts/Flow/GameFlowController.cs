using System.Collections.Generic;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
	public static GameFlowController instance;

	const string ProgressKey = "MapProgress_v1";

	[Header("Map Nodes (assign VideoClip on Video nodes)")]
	public List<MapNodeData> nodes = new List<MapNodeData>();

	[Header("Questions")]
	public List<QuestionData> questions = new List<QuestionData>();

	[Header("Screens")]
	public MapScreen mapScreen;
	public QuestionScreen questionScreen;
	public VideoScreen videoScreen;
	public WinScreen winScreen;
	public LoseScreen loseScreen;
	public GameHudScreen gameHudScreen;

	[HideInInspector] public int currentNodeIndex;
	[HideInInspector] public int unlockedCount = 1;
	[HideInInspector] public int[] nodeStars;

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
		HideLegacyMenus();
		ShowMap();
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

		HideAllScreens();
		if (mapScreen != null)
		{
			mapScreen.Show();
			mapScreen.SetScore(ScoreManager.GetInstance().GetScore());
			mapScreen.Refresh(nodes, unlockedCount, nodeStars);
		}
	}

	public void ReturnHome()
	{
		if (LevelManager.instance != null)
			LevelManager.instance.ClearLevel();
		if (videoScreen != null && videoScreen.videoPlayer != null)
			videoScreen.videoPlayer.Stop();
		ShowMap();
	}

	public void OpenNode(int index)
	{
		if (index < 0 || index >= nodes.Count || index >= unlockedCount)
			return;

		currentNodeIndex = index;
		MapNodeData node = nodes[index];
		switch (node.type)
		{
			case MapNodeType.BubbleLevel:
				StartBubbleLevel(node.levelIndex);
				break;
			case MapNodeType.Question:
				OpenQuestion(node.questionIndex);
				break;
			case MapNodeType.Video:
				OpenVideo(node);
				break;
		}
	}

	void StartBubbleLevel(int levelIndex)
	{
		HideAllScreens();
		if (gameHudScreen != null)
		{
			gameHudScreen.Show();
			gameHudScreen.SetLevelTitle("LEVEL " + (levelIndex + 1));
		}
		if (GameManager.instance != null)
			GameManager.instance.gameState = "play";
		LevelManager.instance.StartLevel(levelIndex);
	}

	void OpenQuestion(int questionIndex)
	{
		if (questions == null || questions.Count == 0)
			EnsureSampleQuestions(questions);
		questionIndex = Mathf.Clamp(questionIndex, 0, questions.Count - 1);
		QuestionData q = questions[questionIndex];
		HideAllScreens();
		if (questionScreen != null)
		{
			questionScreen.ShowQuestion(
				nodes[currentNodeIndex].displayName.ToUpper(),
				"QUESTION  " + (questionIndex + 1) + " / " + questions.Count,
				q.question,
				q.answers,
				ScoreManager.GetInstance().GetScore());
		}
	}

	public void OnQuestionAnswered(int answerIndex)
	{
		QuestionData q = questions[Mathf.Clamp(nodes[currentNodeIndex].questionIndex, 0, questions.Count - 1)];
		bool correct = answerIndex == q.correctIndex;
		if (correct)
			ScoreManager.GetInstance().AddScore(50);
		if (questionScreen != null)
			questionScreen.ShowAnswerResult(answerIndex, q.correctIndex);
		CompleteCurrentNode(correct ? 3 : 1);
		Invoke(nameof(ShowMap), 1.1f);
	}

	void OpenVideo(MapNodeData node)
	{
		HideAllScreens();
		if (videoScreen != null)
			videoScreen.Play(node.videoClip);
	}

	public void OnVideoFinished()
	{
		CompleteCurrentNode(3);
		ShowMap();
	}

	public void OnVideoClosed()
	{
		int existing = (nodeStars != null && currentNodeIndex < nodeStars.Length) ? nodeStars[currentNodeIndex] : 0;
		CompleteCurrentNode(existing > 0 ? existing : 3);
		ShowMap();
	}

	public void ShowLevelWin()
	{
		int throws = ScoreManager.GetInstance().GetThrows();
		int stars = throws <= 12 ? 3 : throws <= 20 ? 2 : 1;
		CompleteCurrentNode(stars);
		HideAllScreens();
		if (winScreen != null)
			winScreen.ShowResult(CurrentNodeName(), ScoreManager.GetInstance().GetScore(), stars);
		if (GameManager.instance != null && GameManager.instance.WinMenu != null)
			GameManager.instance.WinMenu.SetActive(false);
	}

	public void ShowLevelLose()
	{
		HideAllScreens();
		if (loseScreen != null)
			loseScreen.ShowResult(CurrentNodeName(), ScoreManager.GetInstance().GetScore(), 0);
		if (GameManager.instance != null && GameManager.instance.LoseMenu != null)
			GameManager.instance.LoseMenu.SetActive(false);
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

	string CurrentNodeName()
	{
		if (nodes != null && currentNodeIndex >= 0 && currentNodeIndex < nodes.Count)
			return nodes[currentNodeIndex].displayName.ToUpper();
		return "LEVEL";
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

	void HideAllScreens()
	{
		if (mapScreen != null) mapScreen.Hide();
		if (questionScreen != null) questionScreen.Hide();
		if (videoScreen != null) videoScreen.Hide();
		if (winScreen != null) winScreen.Hide();
		if (loseScreen != null) loseScreen.Hide();
		if (gameHudScreen != null) gameHudScreen.Hide();
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
}
