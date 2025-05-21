using Cysharp.Threading.Tasks.Triggers;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDialogueManager : MonoBehaviour
{
    SkillSystemManager.SkillData skillDialogueStatus;
    [SerializeField] private Image DialogueSkillImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillSpecText;
    [SerializeField] private TextMeshProUGUI equipButtonText;
    [SerializeField] private GameObject[] ownedSkillButtonSet;

    [SerializeField] private int skillCoolTime;
    [SerializeField] private string skillExplainText;
    [SerializeField] private int skillLevel;
    [SerializeField] private int skillAttribute;
    [SerializeField] private int skillDamagePower;
    bool isInfoSet = false;

    public void SetDialogueSkillStatusInfo(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
    {
        skillDialogueStatus = skillStatus;
        Sprite sprite = skillSprite;
        DialogueSkillImage.sprite = sprite;
        skillNameText.text = $"{skillStatus.skillName}";
        skillSpecText.text = "스킬레벨:" + skillStatus.skillLevel.ToString() +
                             "\n" + skillStatus.skillExplainText + "\n" + $"스킬데미지  :{skillStatus.skillDamagePower}\n" +
                             $"스킬 쿨타임 :{skillStatus.skillCoolTime}";
        //CheckEquipStatus(skillStatus.skillName);
    }


    public void CloseDialouge()
    {
        gameObject.SetActive(false);
    }

    // public void TransferInfoDialougeToOwnedSkillButton()
    // { 
    //     SkillSystemManager.Instance.EquipSkillToSkillSet(skillDialogueStatus, DialogueSkillImage.sprite);
    // }
    //
    // public void CheckEquipStatus(string skillText)
    // {
    //     if (SkillSystemManager.Instance.IsSkillEquipped(skillText))
    //     {
    //         equipButtonText.text = "장착해제";
    //     }
    //
    //     else
    //     {
    //         equipButtonText.text = "장착";
    //     }
    // }
    //
    // public void EquipOrRelease()
    // {
    //     if (equipButtonText.text.Equals("장착"))
    //     {
    //         Logger.Info("장착 프로세스 확인");
    //         SkillSystemManager.Instance.EquipSkill(skillNameText.text);
    //         TransferInfoDialougeToOwnedSkillButton();
    //         gameObject.SetActive(false);
    //     }
    //     else
    //     {
    //         SkillSystemManager.Instance.ReleaseSkill(skillNameText.text);
    //         SkillSystemManager.Instance.ReleaseSkillFromSkillSet(skillNameText.text);
    //         gameObject.SetActive(false);
    //     }
    // }

    // 스킬, 장착 해제  

}