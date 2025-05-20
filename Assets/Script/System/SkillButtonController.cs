using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    [SerializeField] private string skillName;
    [SerializeField] private int skillCoolTime;
    [SerializeField] string skillExplainText;
    [SerializeField] private int skillLevel;
    [SerializeField] private int skillAttribute;
    [SerializeField] private int skillDamagePower;
    public Image skillButtonImage;
    SkillSystemManager.SkillData skillButtonStatus;

    public void SetSkillButtonStatusInfo(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
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


    public void TransferInfoSkillButtonToDialogue()
    {
        Logger.Info("버튼 눌림 확인");
        SkillSystemManager.Instance.InitDialogueInfo(skillButtonStatus,skillButtonImage.sprite);
    }

}






