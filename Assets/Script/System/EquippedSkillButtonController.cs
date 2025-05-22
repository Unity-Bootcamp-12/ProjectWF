using System;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSkillButtonController : MonoBehaviour
{
    [SerializeField] private int skillIndex;
    private SkillData  skillData;
    private int skillAttribute;
    private int skillGrade;

    private Image skillImage;

    private void Awake()
    {
        skillImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (SkillSystemManager.Instance.equipSkillData[skillIndex] != null)
        {
            skillData = SkillSystemManager.Instance.equipSkillData[skillIndex];
            skillAttribute = skillData.skillAttribute;
            skillGrade = skillData.skillGrade;
            skillImage.sprite = SkillSystemManager.Instance.GetSkillSprite(skillAttribute, skillGrade);
        }
        else
        {
            skillData = null;
            skillImage.sprite = null;
        }
    }
}
