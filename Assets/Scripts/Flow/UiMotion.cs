using DG.Tweening;
using UnityEngine;

public class UiRestPose : MonoBehaviour
{
	public Vector3 localScale = Vector3.one;
	public Vector2 anchoredPosition;
	public bool hasAnchored;
}

public static class UiMotion
{
	public static void Kill(Transform root)
	{
		if (root == null)
			return;
		DOTween.Kill(root);
		Transform[] all = root.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < all.Length; i++)
			DOTween.Kill(all[i]);
		CanvasGroup[] groups = root.GetComponentsInChildren<CanvasGroup>(true);
		for (int i = 0; i < groups.Length; i++)
			DOTween.Kill(groups[i]);
	}

	public static CanvasGroup Group(GameObject go)
	{
		CanvasGroup cg = go.GetComponent<CanvasGroup>();
		if (cg == null)
			cg = go.AddComponent<CanvasGroup>();
		return cg;
	}

	public static void Remember(Transform t)
	{
		if (t == null)
			return;
		UiRestPose rest = t.GetComponent<UiRestPose>();
		if (rest != null)
			return;
		rest = t.gameObject.AddComponent<UiRestPose>();
		rest.localScale = t.localScale.sqrMagnitude < 0.0001f ? Vector3.one : t.localScale;
		RectTransform rt = t as RectTransform;
		if (rt != null)
		{
			rest.hasAnchored = true;
			rest.anchoredPosition = rt.anchoredPosition;
		}
	}

	public static void Restore(Transform t)
	{
		if (t == null)
			return;
		UiRestPose rest = t.GetComponent<UiRestPose>();
		if (rest == null)
			return;
		t.localScale = rest.localScale;
		if (rest.hasAnchored)
		{
			RectTransform rt = t as RectTransform;
			if (rt != null)
				rt.anchoredPosition = rest.anchoredPosition;
		}
	}

	public static void Prepare(Transform t)
	{
		if (t == null)
			return;
		Remember(t);
		DOTween.Kill(t);
		Restore(t);
	}

	public static void Popup(RectTransform root, float duration, TweenCallback onDone)
	{
		if (root == null)
			return;
		Prepare(root);
		CanvasGroup cg = Group(root.gameObject);
		DOTween.Kill(cg);
		cg.alpha = 0f;
		root.localScale = Vector3.one * 0.78f;
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(root.gameObject);
		s.Join(cg.DOFade(1f, duration * 0.55f));
		s.Join(root.DOScale(1f, duration).SetEase(Ease.OutBack));
		if (onDone != null)
			s.OnComplete(onDone);
	}

	public static void FadeIn(GameObject go, float duration, TweenCallback onDone)
	{
		if (go == null)
			return;
		CanvasGroup cg = Group(go);
		DOTween.Kill(cg);
		cg.alpha = 0f;
		Tweener tw = cg.DOFade(1f, duration).SetUpdate(true).SetLink(go);
		if (onDone != null)
			tw.OnComplete(onDone);
	}

	public static void SlideIn(RectTransform rt, Vector2 fromOffset, float delay, float duration)
	{
		if (rt == null)
			return;
		Prepare(rt);
		Vector2 dest = rt.anchoredPosition;
		rt.anchoredPosition = dest + fromOffset;
		rt.DOAnchorPos(dest, duration).SetDelay(delay).SetEase(Ease.OutCubic).SetUpdate(true).SetLink(rt.gameObject);
	}

	public static void StaggerPop(Transform parent, float delayStep, float duration)
	{
		if (parent == null)
			return;
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			Prepare(child);
			Vector3 dest = child.localScale;
			child.localScale = dest * 0.15f;
			child.DOScale(dest, duration).SetDelay(i * delayStep).SetEase(Ease.OutBack).SetUpdate(true).SetLink(child.gameObject);
		}
	}

	public static void IdlePulse(Transform t, float extra, float duration)
	{
		if (t == null)
			return;
		Remember(t);
		Restore(t);
		DOTween.Kill(t);
		Vector3 baseScale = t.localScale;
		t.DOScale(baseScale * extra, duration)
			.SetEase(Ease.InOutSine)
			.SetLoops(-1, LoopType.Yoyo)
			.SetUpdate(true)
			.SetLink(t.gameObject);
	}

	public static void IdleBob(RectTransform rt, float amount, float duration)
	{
		if (rt == null)
			return;
		Remember(rt);
		Restore(rt);
		DOTween.Kill(rt);
		rt.DOAnchorPosY(rt.anchoredPosition.y + amount, duration)
			.SetEase(Ease.InOutSine)
			.SetLoops(-1, LoopType.Yoyo)
			.SetUpdate(true)
			.SetLink(rt.gameObject);
	}

	public static void Punch(Transform t)
	{
		if (t == null)
			return;
		t.DOPunchScale(Vector3.one * 0.12f, 0.28f, 8, 0.55f).SetUpdate(true);
	}

	public static RectTransform Find(Transform root, string name)
	{
		if (root == null)
			return null;
		Transform[] all = root.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < all.Length; i++)
		{
			if (all[i].name == name)
				return all[i] as RectTransform;
		}
		return null;
	}

	public static void PulseNamed(Transform root, params string[] names)
	{
		if (root == null || names == null)
			return;
		for (int n = 0; n < names.Length; n++)
		{
			RectTransform rt = Find(root, names[n]);
			if (rt != null)
				IdlePulse(rt, 1.07f, 1.15f + n * 0.08f);
		}
	}
}
