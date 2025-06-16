using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmproText;
    [SerializeField] private Image cardImage;
    
    void Start()
    {
        RandomSelect();
    }

    private void RandomSelect()
    {
        var curItems = DataController.Instance.InGameDataContainer.GetItems();
        
        int rand = Random.Range(0, curItems.Count);
        
        Debug.Log(curItems[rand]);

        if (curItems[rand].Item1 is Item item)
        {
            tmproText.text = DataController.Instance.GetVolatileStatData((int)item).description;
            cardImage.sprite = DataController.Instance.GetVolatileStatData((int)item).icon;
            
            item.Apply();
        }
        else if (curItems[rand].Item1 is Skill skill)
        {
            int nextLevel = skill.Get<(float, int)>().Item2 + 1;
            bool isEvolvedSkill = nextLevel > 8;
            
            if (isEvolvedSkill)
            {
                skill += 1;         // 진화한 스킬 ID
                nextLevel = 1;   // 진화 스킬은 1레벨부터 시작
            }
            
            var targetSkill = DataController.Instance.GetSkillDataStat((int)skill, nextLevel).FirstOrDefault();
            cardImage.sprite = targetSkill.UI_Icon;
            tmproText.text = targetSkill.UI_Description;
            
            skill.Set(Skill.LEVEL_UP);
        }
    }
}
