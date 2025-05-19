using Cysharp.Threading.Tasks.Triggers;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDialogueManager : MonoBehaviour, IDataSharable
{

    SkillSystemManager.SkillData skillDialogueStatus;
    [SerializeField] private Image DialogueSkillImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillSpecText;
    [SerializeField] private TextMeshProUGUI equipButtonText;

    [SerializeField] private int skillCoolTime;
    [SerializeField] private string skillExplainText;
    [SerializeField] private int skillLevel;
    [SerializeField] private int skillAttribute;
    [SerializeField] private int skillDamagePower;
    bool isInfoSet = false;

    private void OnEnable()
    {
        if(isInfoSet ==false)
        {
            return;
        }

        if (SkillSystemManager.Instance.IsSkillEquipped(skillNameText.text))
        {
            equipButtonText.text = "장착해제";
        }

        else
        {
            equipButtonText.text = "장착";
        }

    }
    public void GetSkillStatus(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
    {

        skillDialogueStatus = new SkillSystemManager.SkillData();
        skillDialogueStatus = skillStatus;

        Sprite sprite = skillSprite;
        DialogueSkillImage.sprite = sprite;
        skillNameText.text = $"{skillStatus.skillName}";
        skillSpecText.text = "스킬레벨" + skillStatus.skillLevel.ToString() +
            "\n" + skillStatus.skillExplainText + "\n" + $"피해량 :{skillStatus.skillDamagePower}\n" + $"대기시간 :{skillStatus.skillCoolTime}";

        isInfoSet = true;
    }


    public void CloseDialouge()
    {
        gameObject.SetActive(false);

    }

    public void OutSkillStatus()
    {


    }

    public void EquipOrRelease()
    {
        if (equipButtonText.text.Equals("장착"))
        {
            Logger.Info("장착프로세스 확인중");
            SkillSystemManager.Instance.EquipSkill(skillNameText.text);
            gameObject.SetActive(false);
        }
        else
        {
            SkillSystemManager.Instance.ReleaseSkill(skillNameText.text);
            gameObject.SetActive(false);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
