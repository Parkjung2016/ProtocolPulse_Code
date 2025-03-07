using System;
using DG.Tweening;
using TransitionsPlus;
using UnityEngine;
using Object = UnityEngine.Object;

public class FadeManager : MonoSingleton<FadeManager>
{
    private TransitionAnimator _transitionAnimator;

    private void Awake()
    {
        FadeManager[] fadeManagers = Object.FindObjectsByType<FadeManager>(FindObjectsSortMode.None);
        if (fadeManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            _transitionAnimator = GetComponent<TransitionAnimator>();
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void FadeIn(float duration, Action CallBack = null)
    {
        Instance._transitionAnimator.profile.duration = duration;
        Instance._transitionAnimator.Play();
        DOVirtual.DelayedCall(duration, () => CallBack?.Invoke());
    }

    public static void FadeOut(float duration, Action CallBack = null)
    {
        DOVirtual.Float(1, 0, 1, x => Instance._transitionAnimator.SetProgress(x)).OnComplete(() => CallBack?.Invoke());
    }
}