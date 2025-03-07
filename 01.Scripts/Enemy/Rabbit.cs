using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using PJH.Manager;
using UnityEngine;

public class Rabbit : Enemy
{
    [SerializeField] private int _maxAttackDistance = 4;
    [SerializeField] private EventReference _attackSound;

    private int _currentAttackWaitTurn = 0;

    public override bool CanAttack()
    {
        Vector3Int ppos = Managers.PlayerManager.Player.NextPosition;
        return (position.x == ppos.x || position.y == ppos.y) && Vector3Int.Distance(position, ppos) < 4.1f;
    }

    protected override async UniTask Attack()
    {
        RuntimeManager.PlayOneShot(_attackSound);

        bool hit = false;
        Player player = Managers.PlayerManager.Player;
        for (int i = 1; i <= _maxAttackDistance; i++)
        {
            Vector3Int pos = position + attackDir * i;
            if (player.NextPosition == pos)
                hit = true;
        }

        if (hit)
        {
            Vector3Int moveto = player.NextPosition - attackDir;
            AnimatorCompo.ChangeAnimation("Attack", false, 1);
            await transform.DOMove(moveto, 0.1f);
            player.HealthCompo.ApplyDamage(5);
        }
        else
            await transform.DOMove(position + attackDir * _maxAttackDistance, 0.1f);

        AnimatorCompo.ChangeAnimation("Idle");
    }

    protected override async UniTask Move()
    {
        float dir = (NextPosition - position).x;
        AnimatorCompo.SetFlip(dir);
        AnimatorCompo.ChangeAnimation("Move");
        await MoveCompo.MoveTo(NextPosition);
        AnimatorCompo.ChangeAnimation("Idle");
    }

    protected override void NextMove()
    {
        var dir = Managers.PlayerManager.Player.NextPosition - position;

        if (dir.sqrMagnitude < 0.1f)
            NextPosition = position;
        else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.z))
            if (Mathf.Abs(dir.x) > 0.1f)
                NextPosition = position + new Vector3Int(dir.x > 0 ? 1 : -1, 0, 0);
            else
                NextPosition = position + new Vector3Int(0, 0, dir.z > 0 ? 1 : -1);
        else if (Mathf.Abs(dir.z) > 0.1f)
            NextPosition = position + new Vector3Int(0, 0, dir.z > 0 ? 1 : -1);
        else
            NextPosition = position + new Vector3Int(dir.x > 0 ? 1 : -1, 0, 0);

        foreach (var entity in EntityManager.Instance.entities)
        {
            if (entity == this)
                continue;
            if (NextPosition == entity.NextPosition)
            {
                NextPosition = position;
                break;
            }
        }
    }

    protected override UniTask PrepareAttack()
    {
        AnimatorCompo.ChangeAnimation("Attack Ready", false);
        var dir = Managers.PlayerManager.Player.NextPosition - position;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            attackDir = new Vector3Int(dir.x > 0 ? 1 : -1, 0, 0);
        else
            attackDir = new Vector3Int(0, 0, dir.z > 0 ? 1 : -1);
        AnimatorCompo.SetFlip(attackDir.x);
        return UniTask.CompletedTask;
    }
}