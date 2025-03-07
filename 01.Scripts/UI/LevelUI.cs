using System;
using PJH.Manager;
using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    private TextMeshProUGUI _levelTMP;

    private void Awake()
    {
        _levelTMP = GetComponent<TextMeshProUGUI>();
        Managers.LevelManager.OnChangedLevel += HandleLevelChanged;
    }

    private void OnDestroy()
    {
        Managers.LevelManager.OnChangedLevel -= HandleLevelChanged;
    }

    private void HandleLevelChanged(int value)
    {
        _levelTMP.SetText(value.ToString());
    }
}