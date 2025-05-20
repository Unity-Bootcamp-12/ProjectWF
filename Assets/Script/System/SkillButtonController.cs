using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour,IDataSharable
{

    [SerializeField] private string skillName;
    [SerializeField] private int skillCoolTime;
    [SerializeField] string skillExplainText;
    [SerializeField] private int skillLevel;
    [SerializeField] private int skillAttribute;
    [SerializeField] private int skillDamagePower;
    [SerializeField] GameObject skillDialogue;
    public Image skillButtonImage;
    SkillSystemManager.SkillData skillButtonStatus;

    public void DownloadSkillStatus(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
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


    public void UploadSkillStatus()
    {
        Logger.Info(skillButtonStatus.skillName);
        skillDialogue.SetActive(true);
        skillDialogue.GetComponent<SkillDialogueManager>().DownloadSkillStatus(skillButtonStatus, skillButtonImage.sprite);

    }

}

public interface IDataSharable
{
    public void DownloadSkillStatus(SkillSystemManager.SkillData skillStatus, Sprite skillSprite);
    public void UploadSkillStatus();


}




