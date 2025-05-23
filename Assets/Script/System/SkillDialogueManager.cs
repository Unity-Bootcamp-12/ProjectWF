using Cysharp.Threading.Tasks.Triggers;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDialogueManager : MonoBehaviour
{
    SkillData skillDialogueStatus;
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

    public void SetDialogueSkillStatusInfo(SkillData skillStatus, Sprite skillSprite)
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

   

}