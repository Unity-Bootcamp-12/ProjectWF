using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    [SerializeField] private EnumSkillAttribute skillButtonAttributeNumber;
    [SerializeField] private int skillGradeNumber;

    public Image skillButtonImage;
    private SkillData skillButtonStatus;

    private void Start()
    {
        skillButtonImage = GetComponent<Image>();
        skillButtonImage.sprite =
            SkillSystemManager.Instance.GetSkillSprite((int)skillButtonAttributeNumber, skillGradeNumber);
        ShowButtonSkillUnlockedState();
    }

    private void Update()
    {
        ShowButtonSkillUnlockedState();
    }

    public void OnSkillButtonClick()
    {
        Logger.Info($"{skillButtonAttributeNumber}, {skillGradeNumber}");
        SkillSystemManager.Instance.ShowDialogue(skillButtonAttributeNumber, skillGradeNumber);
    }

    public void ShowButtonSkillUnlockedState()
    {
        bool isSkillUnlocked =
            SkillSystemManager.Instance.isSkillUsingUnloked((int)skillButtonAttributeNumber, skillGradeNumber);
        if (isSkillUnlocked)
        {
            skillButtonImage.color = Color.white;
        }
        else
        {
            skillButtonImage.color = Color.gray;
        }
    }
}