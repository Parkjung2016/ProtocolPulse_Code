using FMODUnity;
using UnityEngine;
using EventReference = FMODUnity.EventReference;

public class ThrowSwordAuraSkill : PlayerSkill
{
    [SerializeField] private SwordAura _swordAura;
    [SerializeField] private EventReference _skillSound;

    public override void UseSkill()
    {
        RuntimeManager.PlayOneShot(_skillSound);
        SwordAura aura = Instantiate(_swordAura, transform.position, Quaternion.identity);
        float flip = -_player.AnimatorCompo.GetFlip();
        aura.SetDir(Vector3Int.right * (int)flip);
    }
}