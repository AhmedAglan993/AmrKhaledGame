using UnityEngine;
using UnityEngine.UI;

public static class UiBuilder
{
	public static readonly Color Brown = new Color(0.29f, 0.18f, 0.08f);
	public static readonly Color Cream = new Color(0.96f, 0.90f, 0.78f);

	public static Font DefaultFont()
	{
		Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		if (font == null)
			font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		return font;
	}

	public static RectTransform Stretch(GameObject go)
	{
		RectTransform rt = go.GetComponent<RectTransform>();
		if (rt == null)
			rt = go.AddComponent<RectTransform>();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
		rt.localScale = Vector3.one;
		rt.localRotation = Quaternion.identity;
		return rt;
	}

	public static GameObject Panel(string name, Transform parent, Sprite sprite, Color color)
	{
		GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		go.transform.SetParent(parent, false);
		Stretch(go);
		Image img = go.GetComponent<Image>();
		img.sprite = sprite;
		img.color = color;
		img.raycastTarget = true;
		if (sprite != null)
			img.preserveAspect = false;
		return go;
	}

	public static Image Image(string name, Transform parent, Sprite sprite, Vector2 size, Vector2 anchored)
	{
		GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
		go.transform.SetParent(parent, false);
		RectTransform rt = go.GetComponent<RectTransform>();
		rt.sizeDelta = size;
		rt.anchoredPosition = anchored;
		UnityEngine.UI.Image img = go.GetComponent<UnityEngine.UI.Image>();
		img.sprite = sprite;
		img.color = Color.white;
		img.preserveAspect = true;
		img.raycastTarget = false;
		return img;
	}

	public static Text Label(string name, Transform parent, string content, int fontSize, Color color, Vector2 size, Vector2 anchored, TextAnchor align = TextAnchor.MiddleCenter)
	{
		GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
		go.transform.SetParent(parent, false);
		RectTransform rt = go.GetComponent<RectTransform>();
		rt.sizeDelta = size;
		rt.anchoredPosition = anchored;
		Text text = go.GetComponent<Text>();
		text.font = DefaultFont();
		text.text = content;
		text.fontSize = fontSize;
		text.color = color;
		text.alignment = align;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		text.raycastTarget = false;
		return text;
	}

	public static Button MakeButton(string name, Transform parent, Sprite sprite, Vector2 size, Vector2 anchored, string label, int fontSize, Color labelColor)
	{
		GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
		go.transform.SetParent(parent, false);
		RectTransform rt = go.GetComponent<RectTransform>();
		rt.sizeDelta = size;
		rt.anchoredPosition = anchored;
		Image img = go.GetComponent<Image>();
		img.sprite = sprite;
		img.color = Color.white;
		img.preserveAspect = false;
		img.raycastTarget = true;
		Button btn = go.GetComponent<Button>();
		btn.targetGraphic = img;
		if (!string.IsNullOrEmpty(label))
			Label("Label", go.transform, label, fontSize, labelColor, size, Vector2.zero);
		return btn;
	}

	public static void AnchorTopStretch(RectTransform rt, float height, float yOffset)
	{
		rt.anchorMin = new Vector2(0f, 1f);
		rt.anchorMax = new Vector2(1f, 1f);
		rt.pivot = new Vector2(0.5f, 1f);
		rt.sizeDelta = new Vector2(0f, height);
		rt.anchoredPosition = new Vector2(0f, yOffset);
	}

	public static void AnchorBottomLeft(RectTransform rt, Vector2 size, Vector2 pos)
	{
		rt.anchorMin = new Vector2(0f, 0f);
		rt.anchorMax = new Vector2(0f, 0f);
		rt.pivot = new Vector2(0f, 0f);
		rt.sizeDelta = size;
		rt.anchoredPosition = pos;
	}
}
