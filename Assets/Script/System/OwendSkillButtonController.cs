using UnityEngine;
using UnityEngine.UI;

public class OwendSkillButtonController:MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private string skillName;
    [SerializeField] private int skillCoolTime;
    [SerializeField] string skillExplainText;
    [SerializeField] private int skillLevel;
    [SerializeField] private int skillAttribute;
    [SerializeField] private int skillDamagePower;
    [SerializeField] GameObject skillDialogue;
    public Image skillButtonImage;
    SkillSystemManager.SkillData skillButtonStatus;

    public void RemoveSKillStatus()
    {
        skillButtonStatus = null;
        skillButtonImage.sprite = null;

    }

    public bool IsThatSKillName(string skillName)
    {
        if (skillButtonStatus == null)
        {
            Logger.Error("SkillButtonStatus is null");
        }
        if (skillButtonStatus.skillName == skillName)
        {
            return true;
        }
        else
        {
            return false;   
        }
           
    }

    public bool IsSkillButtonNull()
    {
        if (skillButtonStatus == null)
        {
            return true;
        }

        else
        {
            return false;   
        }
        
    }
    
    

    public void SetOwnedSkillButtonSkillStatusInfo(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
    {
        skillButtonStatus = new SkillSystemManager.SkillData();
        
        skillButtonImage =GetComponent<Image>();
        
        skillButtonStatus = skillStatus;
        Sprite sprite = skillSprite;
        
        skillButtonImage.sprite = sprite;
        skillCoolTime = skillStatus.skillCoolTime;
        skillExplainText = skillStatus.skillExplainText;
        skillLevel = skillStatus.skillLevel;
        skillAttribute = skillStatus.skillAttribute;
        skillDamagePower = skillStatus.skillDamagePower;
    }

    public void TransferInfoOwenedSkillButtonToDialogue()
    {
        //SkillSystemManager.Instance.InitDialogueInfo(skillButtonStatus,skillButtonImage.sprite);
    }
}
