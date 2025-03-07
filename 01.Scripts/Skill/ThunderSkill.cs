using FMODUnity;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ThunderSkill : PlayerSkill
{
    [SerializeField] private int damage = 5;
    [SerializeField] private ParticleSystem thunderParticle;
    [SerializeField] private float _detectRadius = 4f;
    [SerializeField] private MMF_Player _feedback;
    [SerializeField] private EventReference _skillSound;

    public override void UseSkill()
    {
        var enemies = FindEnemiesInRange(_player.transform, _detectRadius);
        if (enemies.Count > 0)
        {
            _feedback.PlayFeedbacks();
            RuntimeManager.PlayOneShot(_skillSound);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].HealthCompo?.ApplyDamage(damage);
            if (thunderParticle)
                Instantiate(thunderParticle, enemies[i].position, Quaternion.identity);
        }
    }
}