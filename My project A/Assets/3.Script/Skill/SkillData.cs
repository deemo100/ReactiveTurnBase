using System;
using UnityEngine;

[Serializable]
public class SkillData
{
    public int    Id;
    public string Name;
    public int    Cost;
    public string IconName;
    public TargetType TargetType;
}

public enum TargetType
{
    EnemyOnly,
    AllyOnly
}