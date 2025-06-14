using System;
using UnityEngine;
using UnityEngine.UI;

public class OwendSkillButtonController:MonoBehaviour
{
    private Image skillButtonImage;
    [SerializeField] private int skillIndex;

    private SkillData skillData;
    private int skillAttribute;
    private int skillGrade;

    private void Start()
    {
        skillButtonImage = GetComponent<Image>();
        ShowOwenedSkillButtonState();
    }

    private void Update()
    {
        
        ShowOwenedSkillButtonState();
    }

    public void ShowOwenedSkillButtonState()
    {
        skillData = SkillSystemManager.Instance.equipSkillData[skillIndex];
        if (skillData == null)
        {
            skillButtonImage.sprite = null;
        }
        else
        {
            skillAttribute = skillData.skillAttribute;
            skillGrade = skillData.skillGrade;
            skillButtonImage.sprite = SkillSystemManager.Instance.GetSkillSprite(skillAttribute, skillGrade);
        }
    }
    public void OnOwnedSkillButtonClick()
    {
        if (skillData == null)
        {
            SoundController.Instance.PlaySFX(SFXType.UpgradeNegativeSound);
            return;
        }
        SoundController.Instance.PlaySFX(SFXType.UIClickSound);
        SkillSystemManager.Instance.ShowDialogue((EnumSkillAttribute)skillAttribute,skillGrade);
    }
}
