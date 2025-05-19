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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetSkillStatus(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
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


    public void OutSkillStatus()
    {
        Logger.Info(skillButtonStatus.skillName);
        skillDialogue.SetActive(true);
        skillDialogue.GetComponent<SkillDialogueManager>().GetSkillStatus(skillButtonStatus, skillButtonImage.sprite);

    }

}

public interface IDataSharable
{
    public void GetSkillStatus(SkillSystemManager.SkillData skillStatus, Sprite skillSprite);
    public void OutSkillStatus();


}




