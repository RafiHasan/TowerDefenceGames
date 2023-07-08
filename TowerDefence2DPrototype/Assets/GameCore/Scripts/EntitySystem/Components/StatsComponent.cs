using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum StatID
{
    NONE,
    HEALTH,
    SPEED,
    DAMAGE,
    DAMAGEMIN,
    DAMAGEMAX,
    RANGE,
    COOLDOWN,
    COOLDOWNMIN,
    COOLDOWNMAX,
    SLOWAMMOUNT
}

[Serializable]
public struct Stat
{
    public StatID ID;
    public float value;
    public float buff;
}

public struct StatsComponent : IComponentData
{
    public CustomFixedList16<Stat> Stats;

    public bool HasStat(StatID statId)
    {
        for(int i=0;i<Stats.Count;i++)
        {
            if (statId == Stats[i].ID)
                return true;
        }

        return false;
    }

    public float GetStatValue(StatID statId)
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            if (statId == Stats[i].ID)
                return Stats[i].value*(1+ Stats[i].buff);
        }

        return float.MinValue;
    }

    public void SetStatValue(StatID statId,float value)
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            if (statId == Stats[i].ID)
                Stats[i].value=value;
        }       
    }

    public void SetBuffValue(StatID statId, float value)
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            if (statId == Stats[i].ID)
                Stats[i].buff = value;
        }
    }
}
