using Cysharp.Threading.Tasks;
using FMODUnity;
using MoreMountains.Feedbacks;
using UnityEngine;

public class SwingSkill : PlayerSkill
{
    [SerializeField] private int _power;
    [SerializeField] private MMF_Player _hitFeedback;
    [SerializeField] private EventReference _skillSound;
    [SerializeField] private float _detectRadius;

    public override void UseSkill()
    {
        var enemies = FindEnemiesInRange(_player.transform, _detectRadius);
        if (enemies.Count > 0)
        {
            _hitFeedback.PlayFeedbacks();
            RuntimeManager.PlayOneShot(_skillSound);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].HealthCompo?.ApplyDamage(_power);
        }
    }
}