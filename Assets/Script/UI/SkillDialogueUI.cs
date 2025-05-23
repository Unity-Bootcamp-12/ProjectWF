using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillDialogueUI : BaseUI
{
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillExplainText;
    public TextMeshProUGUI skillCoolTimeText;
    public TextMeshProUGUI skillLevelText;
    public TextMeshProUGUI skillAttributeText;
    public TextMeshProUGUI skillDamagePowerText;
    public Image skillIcon;

    public TextMeshProUGUI skillEquipButtonText;

    private SkillData skillData;
    private bool isEquipped;
    private bool isSkillUnlocked;
    private string skillName;

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
        isSkillUnlocked =
            SkillSystemManager.Instance.isSkillUsingUnloked(skillData.skillAttribute, skillData.skillGrade);
        isEquipped = SkillSystemManager.Instance.IsSkillEquipped(skillName);
        ShowEquipTextState();
    }

    public void OnClickEquipButton()
    {
        if (isSkillUnlocked)
        {
            if (isEquipped)
            {
                SkillSystemManager.Instance.ReleaseSkill(skillName);
            }
            else
            {
                SkillSystemManager.Instance.EquipSkill(skillName);
            }
        }

        else
        {
            if (skillData.skillGrade == 0)
            {
                SkillSystemManager.Instance.UnlockSkill(skillData.skillAttribute, skillData.skillGrade);
            }

            else
            {
                bool isBeforeSkillUnloked =
                    SkillSystemManager.Instance.isSkillUsingUnloked(skillData.skillAttribute, skillData.skillGrade - 1);
                if (isBeforeSkillUnloked)
                {
                    SkillSystemManager.Instance.UnlockSkill(skillData.skillAttribute, skillData.skillGrade);
                }
            }
        }

        CloseUI();
    }

    public void ShowEquipTextState()
    {
        if (isSkillUnlocked)
        {
            if (isEquipped)
            {
                skillEquipButtonText.text = "장착 해제";
                skillIcon.color = Color.white;
            }
            else
            {
                skillEquipButtonText.text = "장착";
                skillIcon.color = Color.white;
            }
        }

        else
        {
            if (skillData.skillGrade == 0)
            {
                skillEquipButtonText.text = "기술 습득";
                skillIcon.color = Color.gray;
            }

            else
            {
                bool isBeforeSkillUnloked =
                    SkillSystemManager.Instance.isSkillUsingUnloked(skillData.skillAttribute, skillData.skillGrade - 1);
                if (isBeforeSkillUnloked)
                {
                    skillEquipButtonText.text = "기술 습득";
                    skillIcon.color = Color.gray;
                }

                else
                {
                    skillEquipButtonText.text = "사용 불가";
                    skillIcon.color = Color.gray;
                }
            }
        }
    }

    public void OnClickExitButton()
    {
        CloseUI();
    }
}