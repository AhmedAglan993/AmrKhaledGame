using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : GameScreen
{
	public Transform nodesRoot;
	public Text scoreText;
	public Sprite nodeOpenSprite;
	public Sprite nodeCurrentSprite;
	public Sprite lockIconSprite;
	public Sprite starOnSprite;

	void Awake()
	{
		WireHud();
		if (nodesRoot == null)
			return;
		for (int i = 0; i < nodesRoot.childCount; i++)
		{
			Button btn = nodesRoot.GetChild(i).GetComponent<Button>();
			if (btn == null)
				continue;
			int index = i;
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(delegate { OnNodeClicked(index); });
		}
	}

	void OnNodeClicked(int index)
	{
		if (GameFlowController.instance != null)
			GameFlowController.instance.OpenNode(index);
	}

	public void SetScore(int score)
	{
		if (scoreText != null)
			scoreText.text = "SCORE  " + score;
	}

	public void Refresh(List<MapNodeData> nodes, int unlockedCount, int[] nodeStars)
	{
		if (nodesRoot == null || nodes == null)
			return;

		for (int i = 0; i < nodesRoot.childCount; i++)
		{
			Transform child = nodesRoot.GetChild(i);
			bool unlocked = i < unlockedCount;
			bool current = i == Mathf.Min(unlockedCount - 1, nodes.Count - 1);
			Image img = child.GetComponent<Image>();
			if (img != null)
			{
				img.sprite = current && nodeCurrentSprite != null ? nodeCurrentSprite : nodeOpenSprite;
				img.color = unlocked ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
			}

			Transform lockT = child.Find("Lock");
			if (lockT != null)
				lockT.gameObject.SetActive(!unlocked);

			Transform numberT = child.Find("Number");
			if (numberT != null)
				numberT.gameObject.SetActive(unlocked);

			Transform stars = child.Find("Stars");
			if (stars != null)
			{
				bool showStars = unlocked && nodeStars != null && i < nodeStars.Length && nodeStars[i] > 0;
				stars.gameObject.SetActive(showStars);
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

	void WireHud()
	{
		ScreenHud.Wire(transform);
	}
}
