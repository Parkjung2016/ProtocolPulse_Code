using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

public class BeatTrackerUI : MonoSingleton<BeatTrackerUI>
{
    [SerializeField] private RectTransform _elementLParent;
    [SerializeField] private BeatElement _elementLPrefab;
    [SerializeField] private RectTransform _circle;
    [SerializeField] private float _elementSpawnPosX = 300;
    [SerializeField] private MMF_Player _successCheckInteractFeedback;
    [SerializeField] private MMF_Player timingFeedback;
    private Queue<BeatElement> _beatElements = new();
    private float _timer;

    private void Awake()
    {
        BeatTracker.Instance.OnSuccessCheckInteract += HandleSuccessCheckInteract;
        BeatTracker.Instance.OnBeatTiming += HandleCreateBitElem;
    }
    private void OnDestroy()
    {
        if (!BeatTracker.Instance)
            return;
        BeatTracker.Instance.OnSuccessCheckInteract -= HandleSuccessCheckInteract;
        BeatTracker.Instance.OnBeatTiming -= HandleCreateBitElem;
    }
    private void HandleCreateBitElem()
    {
        timingFeedback.PlayFeedbacks();
        CreateBeatElement();
    }

    private void HandleSuccessCheckInteract()
    {
        _successCheckInteractFeedback.PlayFeedbacks();
    }

    public async UniTask CreateBeatElement()
    {
        float duration = BeatTracker.Instance.GetBeatInterval();
        await UniTask.Delay((int)(duration / 2 * 1000));
        var element = Instantiate(_elementLPrefab, _elementLParent);
        element.SetMovement(duration * 2, new Vector2(-_elementSpawnPosX, 0));
        _beatElements.Enqueue(element);
    }

    public void DestroyBeatElement()
    {
        if (_beatElements.Count == 0) return;
        var element = _beatElements.Dequeue();
        Destroy(element.gameObject);
    }
}