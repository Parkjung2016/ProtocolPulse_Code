using DG.Tweening;
using UnityEngine;

public class BeatElement : MonoBehaviour
{
    private RectTransform _rectTrm;
    private CanvasGroup _canvasGroup;

    public void SetMovement(float duration, Vector2 startPos)
    {
        _rectTrm = GetComponent<RectTransform>();
        _rectTrm.anchoredPosition = startPos;
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;

        _rectTrm.DOAnchorPos(-startPos, duration).OnComplete(() =>
        {
            BeatTrackerUI.Instance.DestroyBeatElement();
        });

        Sequence seq = DOTween.Sequence();
        seq.Append(_canvasGroup.DOFade(1, duration / 3));
        seq.Join(transform.DOScale(1f, duration / 3));
        seq.Append(_canvasGroup.DOFade(0, duration / 2));
        seq.Join(transform.DOScale(0f, duration / 2));
        seq.Play();
    }
}