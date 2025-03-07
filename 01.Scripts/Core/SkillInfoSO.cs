using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Skill Info", menuName = "SO/Skill Info")]
public class SkillInfoSO : ScriptableObject
{
    public string skillTypeName;
    public List<InputType> toUseSkillInputList;
}

public enum InputType
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    SameMoveDir,
    Attack
}