using System;
using UnityEngine;
using UnityEngine.Video;

public enum MapNodeType
{
	BubbleLevel,
	Question,
	Video
}

[Serializable]
public class QuestionData
{
	public string question;
	public string[] answers = new string[4];
	public int correctIndex;
}

[Serializable]
public class MapNodeData
{
	public string displayName = "Node";
	public MapNodeType type = MapNodeType.BubbleLevel;
	[Tooltip("Used when type is BubbleLevel. Matches LevelManager level index.")]
	public int levelIndex;
	[Tooltip("Used when type is Question. Index into the Questions list.")]
	public int questionIndex;
	[Tooltip("Assign an .mp4 / VideoClip later. Leave empty until the video is ready.")]
	public VideoClip videoClip;
}

[Serializable]
public class MapProgress
{
	public int unlockedCount = 1;
	public int[] stars;
}
