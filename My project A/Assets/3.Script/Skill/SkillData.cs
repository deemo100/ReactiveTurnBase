using System;
using UnityEngine;

[Serializable]
public class SkillData
{
    public int        id;
    public string     name;
    public int        cost;
    public Sprite     icon;        // SkillSlotUI에 뿌리기 위해
    public TargetType targetType;  // EnemyOnly, AllyOnly 등
}

public enum TargetType
{
    EnemyOnly,
    AllyOnly,
    Self,
    All
}