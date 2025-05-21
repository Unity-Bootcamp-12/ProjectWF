using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class SkillDialogueUI : BaseUI
{
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillExplainText;
    public TextMeshProUGUI skillCoolTimeText;
    public TextMeshProUGUI skillLevelText;
    public TextMeshProUGUI  skillAttributeText;
    public TextMeshProUGUI skillDamagePowerText;
    public Image skillIcon;

    public TextMeshProUGUI skillEquipButtonText;
    
    private SkillData skillData;
    private bool isEquipped;
    private string  skillName;
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        skillData = uiData as SkillData;

        skillName = skillData.skillName;
        skillNameText.text = skillName;
        skillExplainText.text = skillData.skillExplainText;
        skillCoolTimeText.text = skillData.skillCoolTime.ToString();
        skillLevelText.text = skillData.skillLevel.ToString();
        skillAttributeText.text = ((EnumSkillAttribute)skillData.skillAttribute).ToString();
        skillDamagePowerText.text = skillData.skillDamagePower.ToString();
        skillIcon.sprite = SkillSystemManager.Instance.GetSkillSprite(skillData.skillAttribute, skillData.skillGrade);
        
        isEquipped = SkillSystemManager.Instance.IsSkillEquipped(skillName);
        if (isEquipped)
        {
            skillEquipButtonText.text = "장착 해제";
        }
        else
        {
            skillEquipButtonText.text = "장착";
        }
    }

    public void OnClickEquipButton()
    {
        if (isEquipped)
        {
            SkillSystemManager.Instance.ReleaseSkill(skillName);
        }
        else
        {
            SkillSystemManager.Instance.EquipSkill(skillName);
        }
        CloseUI();
    }

    public void OnClickExitButton()
    {
        CloseUI();
    }
}
