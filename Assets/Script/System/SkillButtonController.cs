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
        SkillSystemManager.Instance.onSkillUnlockStateChanged += ShowButtonSkillUnlockedState;
    }
    

    public void OnSkillButtonClick()
    {
        Logger.Info($"{skillButtonAttributeNumber}, {skillGradeNumber}");
        SkillSystemManager.Instance.ShowDialogue(skillButtonAttributeNumber, skillGradeNumber);
    }

    public void ShowButtonSkillUnlockedState(int skillAttributeNumber , int skillGradeNumber )
    {
        if (skillAttributeNumber != (int)skillButtonAttributeNumber || skillGradeNumber != this.skillGradeNumber)
        {
            return;
        }
        bool isSkillUnlocked = SkillSystemManager.Instance.isSkillUsingUnloked((int)skillButtonAttributeNumber, skillGradeNumber);
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