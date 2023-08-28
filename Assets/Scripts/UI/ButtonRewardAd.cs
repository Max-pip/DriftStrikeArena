using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ButtonRewardAd : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, 150f);
        StartCoroutine(AnimateAppearanceButtonCoroutine());
    }

    private IEnumerator AnimateAppearanceButtonCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _rectTransform.DOAnchorPosY(-60f, 1.5f).SetEase(Ease.InOutQuart);
        yield return new WaitForSeconds(1f);
        DOTween.Sequence()
            .Append(_rectTransform.DOScale(1.3f, 1f))
            .AppendInterval(0.25f)
            .Append(_rectTransform.DOScale(1f, 1f))
            .SetLoops(-1);
    }
}
