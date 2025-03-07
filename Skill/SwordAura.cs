using Cysharp.Threading.Tasks;
using DG.Tweening;
using PJH.Manager;
using UnityEngine;

public class SwordAura : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private int lifeTime = 5;

    [SerializeField] private int _power;

    private Vector3Int _dir;

    private void Awake()
    {
        BeatTracker.Instance.OnBeatMovementFunc.Add(Move);
        BeatTracker.Instance.OnBeatAttackFunc.Add(Attack);
    }

    private void OnDestroy()
    {
        if (BeatTracker.Instance != null)
        {
            BeatTracker.Instance.OnBeatMovementFunc.Remove(Move);
            BeatTracker.Instance.OnBeatAttackFunc.Remove(Attack);
        }
    }

    public void SetDir(Vector3Int dir)
    {
        _dir = dir;
        visual.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - 180);
    }

    private async UniTask Attack()
    {
        Entity target = EntityManager.Instance.Find(Vector3Int.RoundToInt(transform.position));
        if (target == null) return;
        Player player = Managers.PlayerManager.Player;
        if (target == player) return;
        player.AttackEnemy(target, _power);
        Destroy(gameObject);
    }

    private async UniTask Move()
    {
        if (_dir == Vector3.zero) return;
        transform.DORotate(new Vector3(0, 0, transform.rotation.z + (_dir.x * 45)), .1f);
        Vector3 newPos = Vector3Int.RoundToInt(transform.position) + _dir;
        await transform.DOMove(newPos, .1f).ToUniTask();
        lifeTime--;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}