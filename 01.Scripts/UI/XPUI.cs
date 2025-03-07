using DG.Tweening;
using PJH.Manager;
using UnityEngine;
using UnityEngine.UI;

public class XPUI : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        Managers.LevelManager.OnChangedXP += HandleXPChanged;
        Managers.LevelManager.OnChangedNextLevelUpXP += HandleMaxXPChanged;
    }


    private void OnDestroy()
    {
        Managers.LevelManager.OnChangedXP -= HandleXPChanged;
        Managers.LevelManager.OnChangedNextLevelUpXP -= HandleMaxXPChanged;
    }

    private void HandleXPChanged(int value)
    {
        _slider.value = value;
    }

    private void HandleMaxXPChanged(int value)
    {
        _slider.maxValue = value;
    }
}