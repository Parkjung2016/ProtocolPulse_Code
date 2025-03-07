using PJH.Manager;
using System.Collections.Generic;
using UnityEngine;

public delegate void CooldownInfoEvent(float current, float total);

public abstract class PlayerSkill : MonoBehaviour
{
    [SerializeField] private bool _skillEnabled = false;
    public bool skillEnabled
    {
        get => _skillEnabled;
        set
        {
            _skillEnabled = value;
            PopupPanel.Instance.Popup(desc);
        }
    }
    public LayerMask whatIsEnemy;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected int _maxCheckEnemy = 5;

    protected float _cooldownTimer;
    protected Player _player;
    protected Collider[] _colliders;

    public bool IsCooldown => _cooldownTimer > 0f;
    public event CooldownInfoEvent OnCooldownEvent;

    public string desc;

    protected virtual void Start()
    {
        _player = Managers.PlayerManager.Player;
        _colliders = new Collider[_maxCheckEnemy];
        BeatTracker.Instance.OnBeatTiming += HandleBeatTiming;
    }
    private void OnDestroy()
    {
        if (!BeatTracker.Instance) return;
        BeatTracker.Instance.OnBeatTiming -= HandleBeatTiming;

    }
    private void HandleBeatTiming()
    {
        if (_cooldownTimer == 0) return;
        _cooldownTimer -= 1;
        OnCooldownEvent?.Invoke(_cooldownTimer, _cooldown);
    }


    public virtual bool AttemptUseSkill()
    {
        if (_cooldownTimer <= 0 && skillEnabled)
        {
            _cooldownTimer = _cooldown;
            UseSkill();
            return true;
        }

        Debug.Log("Skill cooldown or locked");
        return false;
    }

    public virtual void UseSkill()
    {
    }

    public virtual Transform FindClosestEnemy(Transform checkTransform, float radius)
    {
        Transform closestEnemy = null;
        int cnt = Physics.OverlapSphereNonAlloc(checkTransform.position, radius, _colliders, whatIsEnemy);

        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < cnt; ++i)
        {
            if (_colliders[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                float distanceToEnemy = Vector2.Distance(checkTransform.position, _colliders[i].transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = _colliders[i].transform;
                }
            }
        }

        return closestEnemy;
    }

    public List<Enemy> FindEnemiesInRange(Transform checkTransform, float radius)
    {
        int cnt = Physics.OverlapSphereNonAlloc(checkTransform.position, radius, _colliders, whatIsEnemy);
        List<Enemy> list = new List<Enemy>();

        for (int i = 0; i < cnt; ++i)
        {
            if (_colliders[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                list.Add(enemy);
            }
        }

        return list;
    }
}