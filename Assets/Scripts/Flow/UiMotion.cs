using DG.Tweening;
using UnityEngine;

public class UiRestPose : MonoBehaviour
{
	public Vector3 localScale = Vector3.one;
	public Vector3 localEuler = Vector3.zero;
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
		rest.localEuler = t.localEulerAngles;
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
		t.localEulerAngles = rest.localEuler;
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

	public static Vector3 RestScale(Transform t)
	{
		UiRestPose rest = t != null ? t.GetComponent<UiRestPose>() : null;
		return rest != null ? rest.localScale : Vector3.one;
	}

	public static void Popup(RectTransform root, float duration, TweenCallback onDone)
	{
		JellyPopup(root, 0f, duration, onDone);
	}

	public static void JellyPopup(RectTransform root, float delay, float duration, TweenCallback onDone)
	{
		if (root == null)
			return;
		Prepare(root);
		CanvasGroup cg = Group(root.gameObject);
		DOTween.Kill(cg);
		cg.alpha = 0f;
		root.localScale = new Vector3(0.15f, 0.04f, 1f);
		root.localEulerAngles = new Vector3(0f, 0f, 14f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(root.gameObject).SetDelay(delay);
		s.Join(cg.DOFade(1f, 0.16f));
		s.Join(root.DOScale(new Vector3(1.22f, 0.78f, 1f), duration * 0.45f).SetEase(Ease.OutCubic));
		s.Join(root.DOLocalRotate(new Vector3(0f, 0f, -8f), duration * 0.45f).SetEase(Ease.OutCubic));
		s.Append(root.DOScale(new Vector3(0.88f, 1.16f, 1f), duration * 0.22f).SetEase(Ease.OutQuad));
		s.Join(root.DOLocalRotate(new Vector3(0f, 0f, 4f), duration * 0.22f));
		s.Append(root.DOScale(Vector3.one, duration * 0.28f).SetEase(Ease.OutBack));
		s.Join(root.DOLocalRotate(Vector3.zero, duration * 0.28f).SetEase(Ease.OutBack));
		if (onDone != null)
			s.OnComplete(onDone);
	}

	public static void CelebratePopup(RectTransform root, TweenCallback onDone)
	{
		if (root == null)
			return;
		Prepare(root);
		CanvasGroup cg = Group(root.gameObject);
		DOTween.Kill(cg);
		cg.alpha = 0f;
		Vector2 dest = root.anchoredPosition;
		root.anchoredPosition = dest + new Vector2(0f, 460f);
		root.localScale = new Vector3(0.55f, 1.35f, 1f);
		root.localEulerAngles = new Vector3(0f, 0f, -18f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(root.gameObject);
		s.Join(cg.DOFade(1f, 0.12f));
		s.Join(root.DOAnchorPos(dest, 0.55f).SetEase(Ease.OutBounce));
		s.Join(root.DOScale(new Vector3(1.18f, 0.82f, 1f), 0.55f).SetEase(Ease.OutQuad));
		s.Join(root.DOLocalRotate(new Vector3(0f, 0f, 6f), 0.35f).SetEase(Ease.OutCubic));
		s.Append(root.DOScale(new Vector3(0.92f, 1.1f, 1f), 0.12f));
		s.Join(root.DOLocalRotate(new Vector3(0f, 0f, -3f), 0.12f));
		s.Append(root.DOScale(Vector3.one, 0.16f).SetEase(Ease.OutBack));
		s.Join(root.DOLocalRotate(Vector3.zero, 0.16f));
		if (onDone != null)
			s.OnComplete(onDone);
	}

	public static void OopsPopup(RectTransform root, TweenCallback onDone)
	{
		if (root == null)
			return;
		Prepare(root);
		CanvasGroup cg = Group(root.gameObject);
		DOTween.Kill(cg);
		cg.alpha = 0f;
		root.localScale = new Vector3(1.35f, 0.35f, 1f);
		root.localEulerAngles = new Vector3(0f, 0f, 20f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(root.gameObject);
		s.Join(cg.DOFade(1f, 0.12f));
		s.Join(root.DOScale(new Vector3(0.86f, 1.2f, 1f), 0.28f).SetEase(Ease.OutBack));
		s.Join(root.DOLocalRotate(new Vector3(0f, 0f, -14f), 0.22f).SetEase(Ease.OutCubic));
		s.Append(root.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.1f));
		s.Append(root.DOLocalRotate(new Vector3(0f, 0f, -6f), 0.09f));
		s.Append(root.DOLocalRotate(Vector3.zero, 0.12f).SetEase(Ease.OutBack));
		s.Join(root.DOScale(Vector3.one, 0.16f).SetEase(Ease.OutBack));
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
		float tilt = fromOffset.y > 0f ? 10f : -10f;
		if (Mathf.Abs(fromOffset.x) > Mathf.Abs(fromOffset.y))
			tilt = Mathf.Sign(fromOffset.x) * -14f;
		rt.localEulerAngles = new Vector3(0f, 0f, tilt);
		rt.localScale = new Vector3(0.86f, 1.2f, 1f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(rt.gameObject).SetDelay(delay);
		s.Append(rt.DOAnchorPos(dest, duration).SetEase(Ease.OutBack, 1.6f));
		s.Join(rt.DOLocalRotate(Vector3.zero, duration).SetEase(Ease.OutElastic, 1.1f, 0.45f));
		s.Join(rt.DOScale(new Vector3(1.08f, 0.9f, 1f), duration * 0.7f).SetEase(Ease.OutQuad));
		s.Append(rt.DOScale(Vector3.one, 0.14f).SetEase(Ease.OutBack));
	}

	public static void TossIn(RectTransform rt, float fromX, float delay)
	{
		if (rt == null)
			return;
		Prepare(rt);
		Vector2 dest = rt.anchoredPosition;
		rt.anchoredPosition = dest + new Vector2(fromX, 50f);
		rt.localScale = Vector3.one * 0.25f;
		rt.localEulerAngles = new Vector3(0f, 0f, fromX > 0f ? 28f : -28f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(rt.gameObject).SetDelay(delay);
		s.Append(rt.DOAnchorPos(dest, 0.42f).SetEase(Ease.OutBack, 1.8f));
		s.Join(rt.DOScale(new Vector3(1.16f, 0.84f, 1f), 0.42f).SetEase(Ease.OutCubic));
		s.Join(rt.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutElastic, 1.2f, 0.4f));
		s.Append(rt.DOScale(Vector3.one, 0.14f).SetEase(Ease.OutBack));
	}

	public static void DropIn(RectTransform rt, float fromY, float delay, float tilt)
	{
		if (rt == null)
			return;
		Prepare(rt);
		Vector2 dest = rt.anchoredPosition;
		rt.anchoredPosition = dest + new Vector2(0f, fromY);
		rt.localScale = new Vector3(0.7f, 1.35f, 1f);
		rt.localEulerAngles = new Vector3(0f, 0f, tilt);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(rt.gameObject).SetDelay(delay);
		s.Append(rt.DOAnchorPos(dest, 0.48f).SetEase(Ease.OutBounce));
		s.Join(rt.DOScale(new Vector3(1.2f, 0.78f, 1f), 0.48f).SetEase(Ease.OutQuad));
		s.Join(rt.DOLocalRotate(Vector3.zero, 0.55f).SetEase(Ease.OutElastic, 1.15f, 0.4f));
		s.Append(rt.DOScale(Vector3.one, 0.16f).SetEase(Ease.OutBack));
	}

	public static void StaggerPop(Transform parent, float delayStep, float duration)
	{
		StaggerDrop(parent, delayStep);
	}

	public static void StaggerDrop(Transform parent, float delayStep)
	{
		if (parent == null)
			return;
		for (int i = 0; i < parent.childCount; i++)
		{
			RectTransform child = parent.GetChild(i) as RectTransform;
			if (child == null)
				continue;
			float tilt = (i % 2 == 0) ? -18f : 18f;
			DropIn(child, 240f + (i % 4) * 40f, 0.05f + i * delayStep, tilt);
		}
	}

	public static void TitlePop(RectTransform title, float delay)
	{
		if (title == null)
			return;
		Prepare(title);
		title.localScale = Vector3.zero;
		title.localEulerAngles = new Vector3(0f, 0f, -12f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(title.gameObject).SetDelay(delay);
		s.Append(title.DOScale(1.28f, 0.34f).SetEase(Ease.OutBack, 2.2f));
		s.Join(title.DOLocalRotate(new Vector3(0f, 0f, 6f), 0.34f).SetEase(Ease.OutCubic));
		s.Append(title.DOScale(1f, 0.14f).SetEase(Ease.InOutSine));
		s.Join(title.DOLocalRotate(Vector3.zero, 0.18f).SetEase(Ease.OutBack));
	}

	public static void PopStar(RectTransform star, float delay)
	{
		if (star == null)
			return;
		Prepare(star);
		star.localScale = Vector3.zero;
		star.localEulerAngles = new Vector3(0f, 0f, -90f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(star.gameObject).SetDelay(delay);
		s.Append(star.DOScale(1.4f, 0.3f).SetEase(Ease.OutBack, 2.4f));
		s.Join(star.DOLocalRotate(new Vector3(0f, 0f, 16f), 0.3f).SetEase(Ease.OutCubic));
		s.Append(star.DOScale(1f, 0.14f).SetEase(Ease.OutQuad));
		s.Join(star.DOLocalRotate(Vector3.zero, 0.16f));
	}

	public static void SqueezeIn(RectTransform rt, float delay)
	{
		if (rt == null)
			return;
		Prepare(rt);
		CanvasGroup cg = Group(rt.gameObject);
		DOTween.Kill(cg);
		cg.alpha = 0f;
		rt.localScale = new Vector3(1.2f, 0.08f, 1f);
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(rt.gameObject).SetDelay(delay);
		s.Join(cg.DOFade(1f, 0.12f));
		s.Join(rt.DOScale(new Vector3(0.9f, 1.18f, 1f), 0.28f).SetEase(Ease.OutBack));
		s.Append(rt.DOScale(Vector3.one, 0.16f).SetEase(Ease.OutBack));
	}

	public static void FunnyIdle(Transform t, float squash, float bob, float wiggle, float duration)
	{
		if (t == null)
			return;
		Remember(t);
		Restore(t);
		DOTween.Kill(t);
		Vector3 baseScale = t.localScale;
		RectTransform rt = t as RectTransform;
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(t.gameObject).SetLoops(-1, LoopType.Yoyo);
		s.Append(t.DOScale(new Vector3(baseScale.x * (1f + squash), baseScale.y * (1f - squash * 0.7f), baseScale.z), duration).SetEase(Ease.InOutSine));
		t.DOLocalRotate(new Vector3(0f, 0f, wiggle), duration * 1.15f)
			.SetEase(Ease.InOutSine)
			.SetLoops(-1, LoopType.Yoyo)
			.SetUpdate(true)
			.SetLink(t.gameObject);
		if (rt != null && Mathf.Abs(bob) > 0.01f)
		{
			rt.DOAnchorPosY(rt.anchoredPosition.y + bob, duration * 1.1f)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(true)
				.SetLink(rt.gameObject);
		}
	}

	public static void IdlePulse(Transform t, float extra, float duration)
	{
		FunnyIdle(t, extra - 1f, 0f, 6f, duration);
	}

	public static void IdleBob(RectTransform rt, float amount, float duration)
	{
		FunnyIdle(rt, 0.07f, amount, (amount > 0f ? 5f : -5f), duration);
	}

	public static void Punch(Transform t)
	{
		if (t == null)
			return;
		Remember(t);
		Vector3 dest = RestScale(t);
		DOTween.Kill(t);
		t.localScale = dest;
		Sequence s = DOTween.Sequence().SetUpdate(true).SetLink(t.gameObject);
		s.Append(t.DOScale(new Vector3(dest.x * 1.24f, dest.y * 0.7f, dest.z), 0.07f).SetEase(Ease.OutQuad));
		s.Append(t.DOScale(new Vector3(dest.x * 0.84f, dest.y * 1.22f, dest.z), 0.1f).SetEase(Ease.OutQuad));
		s.Append(t.DOScale(dest, 0.16f).SetEase(Ease.OutBack, 2f));
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
			if (rt == null)
				continue;
			float squash = 0.08f + (n % 3) * 0.015f;
			float wiggle = (n % 2 == 0) ? 8f : -8f;
			FunnyIdle(rt, squash, 6f, wiggle, 0.85f + n * 0.07f);
		}
	}
}
