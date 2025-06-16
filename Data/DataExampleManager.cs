using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DataExampleManager : Singleton<DataExampleManager>
{
    void Start()
    {
        // Debug.Log(DataController.Instance.PlayerDataContainer.GetData(Status.MAX_HP) + " HP");
        // Debug.Log(DataController.Instance.PlayerDataContainer.GetData(Status.AVOID) + " AVOID");
        // Debug.Log(DataController.Instance.PlayerDataContainer.GetData(Status.ATTACK) + " Attack");
        // Debug.Log(DataController.Instance.PlayerDataContainer.GetData(Status.RANGE) + " Range");
        // Debug.Log(DataController.Instance.PlayerDataContainer.GetData(Status.SPEED) + " Speed");
        //
        // Debug.Log("----------------------------------------------------------------------------------------------");
        //
        // Status.MAX_HP.Set(100);
        // Status.AVOID.Set(-99);
        // Status.ATTACK.Set(-999);
        // Status.RANGE.Set(-9999);
        // Status.SPEED.Set(-9999);
        //
        // Debug.Log("----------------------------------------------------------------------------------------------");
        //
        // Debug.Log(Status.MAX_HP.Get<float>());
        // Debug.Log(Status.AVOID.Get<float>());
        // Debug.Log(Status.ATTACK.Get<float>());
        // Debug.Log(Status.RANGE.Get<float>());
        //
        // Debug.Log("----------------------------------------------------------------------------------------------");
        //
        // Debug.Log(Status.ATTACK.Get<float>());
        // Item.SWORD.Apply();
        // Debug.Log(Status.ATTACK.Get<float>());
        //
        // Debug.Log(nameof(PlayerDataContainer));
        
        Debug.Log(StatusAttri.AVOID.Get<int>());
        
        Debug.Log("----------------------------------------------------------------------------------------------");
        
        Skill.MAGIC_DAGGER.Set(0.5f);
        
        Debug.Log(Skill.MAGIC_DAGGER.Get<(float, int)>() + " : Magic Dagger (Cool Time, Level) Value");
        
        Skill.MAGIC_DAGGER.Set(Skill.LEVEL_UP);
        
        Debug.Log(Skill.MAGIC_DAGGER.Get<(float, int)>() + " : Magic Dagger (Cool Time, Level) Value");
        
        Debug.Log("----------------------------------------------------------------------------------------------");

        Debug.Log(Item.SWORD.Get<(float, int)>() + " : Sword");
        
        Item.SWORD.Set(-88);
        
        Debug.Log(Item.SWORD.Get<(float, int)>() + " : Sword");
        
        Debug.Log(Item.BOOK.Get<(float, int)>() + " : BOOK");
        
        Item.BOOK.Set(-77);
        
        Debug.Log(Item.BOOK.Get<(float, int)>() + " : BOOK");
        
        Debug.Log(Item.RING.Get<(float, int)>() + " : RING");
        
        Item.RING.Set(-88);
        
        Debug.Log(Item.RING.Get<(float, int)>() + " : RING");


        //Debug.Log(Skill.MAGIC_DAGGER.Get<SkillDataStat>() + " : Magic Dagger Skill Data Stat");
    }

    public void MakeMoney()
    {
        Status.MONEY.Set(Status.MONEY.Get<float>() + 10);
    }

    public void GetEXP()
    {
        Status.EXP.Set(Status.EXP.Get<float>() + 1);
    }

    public void RandomLevelUp()
    {
        var rand = Random.Range(1, 14);
        
        var target = (StatusAttri)rand;
        
        target.Set(target.Get<int>() + 1);
    }

    public void SceneChange()
    {
        //SceneId.DemoInGame.Load(true);
        
        SceneId.TitleScene.Load(true);
    }

    public void SetAttack()
    {
        var val = Status.ATTACK.Get<float>();
        Status.ATTACK.Set(val * 2);
    }
    
    public void SetCooldown()
    {
        var val = Status.COOLDOWN.Get<float>();
        Status.COOLDOWN.Set(val * 2);
    }
    
    public void SetProjectileNum()
    {
        var val = Status.PROJECTILE_NUM.Get<float>();
        Status.PROJECTILE_NUM.Set(val * 2);
    }
    
    public void SetSpeed()
    {
        var val = Status.SPEED.Get<float>();
        Status.SPEED.Set(val * 2);
    }
    
    public void SetHp()
    {
        var val = Status.MAX_HP.Get<float>();
        Status.MAX_HP.Set(val * 2);
    }
    
    public void SetRange()
    {
        var val = Status.RANGE.Get<float>();
        Status.RANGE.Set(val * 2);
    }
    
    public void SetThrouh()
    {
        var val = Status.THROUGH.Get<float>();
        Status.THROUGH.Set(val * 2);
    }
    
    public void SetProjectileSpeed()
    {
        var val = Status.PROJECTILE_SPEED.Get<float>();
        Status.PROJECTILE_SPEED.Set(val * 2);
    }
    
    public void SetSkillDuration()
    {
        var val = Status.SKILL_DURATION.Get<float>();
        Status.SKILL_DURATION.Set(val * 2);
    }
    
    public void SetAvoid()
    {
        var val = Status.AVOID.Get<float>();
        Status.AVOID.Set(val * 2);
    }
    
    public void SetResurrection()
    {
        var val = Status.RESURRECTION.Get<float>();
        Status.RESURRECTION.Set(val * 2);
    }
    
    public void SetBlood()
    {
        var val = Status.BLOOD_ABSORBING.Get<float>();
        Status.BLOOD_ABSORBING.Set(val * 2);
    }
}
