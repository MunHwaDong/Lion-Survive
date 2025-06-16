using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JH;
using UnityEngine;

public class SkillDataStat
{
    public int Obj_Id;
    public int Fx_Id;
    public int IceSkillAreaPoolId;
    public string SkillType;
    public string name;
    public int Level; //이거
    public int MaxLevel;
    public float C_DMG;
    public float C_Cool; //이거
    public float C_Range;
    public float C_LifeTime;
    public float C_kbForce;
    public float P_LifeTime;
    public float P_Size;
    public float P_Speed;
    public float P_Count;
    public float P_PierceC;
    public float P_SpreadA;
    public float S_Radius;
    public float S_DMG;
    public float S_knForce;
    public float H_Amount;
    public float I_Slow_Amount;
    public float I_Tick;
    public float I_MaxDistance;
    public float I_FreezeDelay;
    public float I_FreezeDMG;
    public float I_UnfreezeDMG;
    public float I_FreezeSlow;
    public string UI_Name;
    public string UI_Description;
    public Sprite UI_Icon;

    public FullSkillStat ToFullSkillStat()
    {
        return new FullSkillStat
        {
            commonStat = new CommonSkillStat
            {
                maxLevel = this.MaxLevel,
                damage = this.C_DMG,
                cooldown = this.C_Cool,
                range = this.C_Range,
                lifeTime = this.C_LifeTime,
                knockbackForce = this.C_kbForce,
            },
            projectileStat = new ProjectileStat
            {
                lifetime = this.P_LifeTime,
                size = this.P_Size,
                projectileSpeed = this.P_Speed,
                projectileCount = (int)this.P_Count,
                pierceCount = this.P_PierceC,
                spreadAngle = this.P_SpreadA,
                projectilePoolId = this.Obj_Id,
                impactEffectPoolId = this.Fx_Id
            },
            splashStat = new SplashStat
            {
                splashRadius = this.S_Radius,
                splashDamage = this.S_DMG,
                splashKnockbackForce = this.S_knForce,
                splashEffectPoolId = this.Fx_Id
            },
            healStat = new HealStat
            {
                healAmount = this.H_Amount,
            },
            iceSkillStat = new IceSkillStat
            {
                IceSkillPoolId = this.Obj_Id,
                IceSkillAreaPoolId = this.IceSkillAreaPoolId,
                slowAmount = this.I_Slow_Amount,
                tickInterval = this.I_Tick,
                maxPlacementRange = this.I_MaxDistance,
                freezeEffectDelay = this.I_FreezeDelay,
                freezeDamage = this.I_FreezeDMG,
                unfreezeDamage = this.I_UnfreezeDMG,
                slowAfterFreeze = this.I_FreezeSlow
            },
        };
    }
}

[System.Serializable]
public class SkillDataStatRow
{
    public int Obj_Id;
    public int Fx_Id;
    public int IceSkillAreaPoolId;
    public string SkillType;
    public string name;
    public int Level;
    public int MaxLevel;
    public string C_DMG;
    public string C_Cool;
    public string C_Range;
    public string C_LifeTime;
    public string C_kbForce;
    public string P_LifeTime;
    public string P_Size;
    public string P_Speed;
    public string P_Count;
    public string P_PierceC;
    public string P_SpreadA;
    public string S_Radius;
    public string S_DMG;
    public string S_knForce;
    public string H_Amount;
    public string I_Slow_Amount;
    public string I_Tick;
    public string I_MaxDistance;
    public string I_FreezeDelay;
    public string I_FreezeDMG;
    public string I_UnfreezeDMG;
    public string I_FreezeSlow;
    public string UI_Name;
    public string UI_Description;
    public string UI_Path;
}

public class SkillDataStatTable : BaseTable
{
    public override void Parsing(string jsonPath)
    {
        base.Parsing(jsonPath);

        skillDataRows = JsonHelper.FromJson<SkillDataStatRow>(json);
        skillDataList = ConvertList();
    }
    
    private List<SkillDataStat> ConvertList()
    {
        List<SkillDataStat> datas = new();

        foreach(var row in skillDataRows)
        {
            SkillDataStat skillData = new SkillDataStat();
            
            skillData.Obj_Id = row.Obj_Id;
            skillData.Fx_Id = row.Fx_Id;
            
            skillData.IceSkillAreaPoolId = row.IceSkillAreaPoolId;
            skillData.SkillType = row.SkillType;
            skillData.name = row.name;
            skillData.Level = row.Level;
            skillData.MaxLevel = row.MaxLevel;
            
            skillData.C_DMG = Convert.ToSingle(row.C_DMG);
            skillData.C_Cool = Convert.ToSingle(row.C_Cool);
            skillData.C_Range = Convert.ToSingle(row.C_Range);
            skillData.C_LifeTime = Convert.ToSingle(row.C_LifeTime);
            skillData.C_kbForce = Convert.ToSingle(row.C_kbForce);
            skillData.P_LifeTime = Convert.ToSingle(row.P_LifeTime);
            skillData.P_Size = Convert.ToSingle(row.P_Size);
            skillData.P_Speed = Convert.ToSingle(row.P_Speed);
            skillData.P_Count = Convert.ToSingle(row.P_Count);
            skillData.P_PierceC = Convert.ToSingle(row.P_PierceC);
            skillData.P_SpreadA = Convert.ToSingle(row.P_SpreadA);
            skillData.S_Radius = Convert.ToSingle(row.S_Radius);
            skillData.S_DMG = Convert.ToSingle(row.S_DMG);
            skillData.S_knForce = Convert.ToSingle(row.S_knForce);
            skillData.H_Amount = Convert.ToSingle(row.H_Amount);
            skillData.I_Slow_Amount = Convert.ToSingle(row.I_Slow_Amount);
            skillData.I_Tick = Convert.ToSingle(row.I_Tick);
            skillData.I_MaxDistance = Convert.ToSingle(row.I_MaxDistance);
            skillData.I_FreezeDelay = Convert.ToSingle(row.I_FreezeDelay);
            skillData.I_FreezeDMG = Convert.ToSingle(row.I_FreezeDMG);
            skillData.I_UnfreezeDMG = Convert.ToSingle(row.I_UnfreezeDMG);
            skillData.I_FreezeSlow = Convert.ToSingle(row.I_FreezeSlow);
            skillData.I_UnfreezeDMG = Convert.ToSingle(row.I_UnfreezeDMG);
            skillData.I_FreezeSlow = Convert.ToSingle(row.I_FreezeSlow);
            
            skillData.UI_Name = row.UI_Name;
            skillData.UI_Description = row.UI_Description;
            skillData.UI_Icon = Resources.Load<Sprite>(row.UI_Path);
            
            datas.Add(skillData);
        }

        return datas;
    }
    
    public List<SkillDataStat> GetSkillDataStat(int id, int level = -1)
    {
        if (level == -1)
        {
            return skillDataList.Where((skillDataStat) => skillDataStat.Obj_Id == id).ToList();
        }
        else
        {
            return skillDataList.Where((skillDataStat) => skillDataStat.Obj_Id == id && skillDataStat.Level == level).ToList();
        }
    }
    
    SkillDataStatRow[] skillDataRows;
    private List<SkillDataStat> skillDataList = new();
}
