using DG.Tweening;
using PJH.Manager;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        var playerHealthCompo = Managers.PlayerManager.Player.HealthCompo;
        playerHealthCompo.OnHealthChanged += HandleHealthChanged;
        playerHealthCompo.OnMaxHealthChanged += HandleMaxHealthChanged;

        HandleMaxHealthChanged(playerHealthCompo.MaxHealth);
        _slider.value = playerHealthCompo.MaxHealth;
    }

    private void HandleMaxHealthChanged(int value)
    {
        _slider.maxValue = value;
    }

    private void OnDestroy()
    {
        if (Managers.PlayerManager == null)
            return;

        Player player = Managers.PlayerManager.Player;
        if (player == null)
            return;
        player.HealthCompo.OnHealthChanged -= HandleHealthChanged;
        player.HealthCompo.OnMaxHealthChanged -= HandleMaxHealthChanged;
    }

    private void HandleHealthChanged(int value)
    {
        _slider.DOValue(value, .3f);
    }
}