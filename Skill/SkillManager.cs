using System;
using System.Collections.Generic;
using System.Linq;


public class SkillManager : MonoSingleton<SkillManager>
{
    public Dictionary<Type, PlayerSkill> skills;

    private void Awake()
    {
        skills = new Dictionary<Type, PlayerSkill>();

        GetComponents<PlayerSkill>().ToList().ForEach(skill =>
        {
            Type type = skill.GetType();
            skills.Add(type, skill);
        });
    }

    public T GetSkill<T>() where T : PlayerSkill
    {
        Type t = typeof(T);
        if (skills.TryGetValue(t, out PlayerSkill target))
        {
            return target as T;
        }

        return null;
    }

    public PlayerSkill GetSkill(string skillTypeName)
    {
        Type type = Type.GetType(skillTypeName);
        if (type == null) return null;

        return skills.GetValueOrDefault(type);
    }

    public void UseSkillFeedback(PlayerSkill skillType)
    {
    }
}