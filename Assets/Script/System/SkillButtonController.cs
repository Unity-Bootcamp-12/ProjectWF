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
        ShowSkillButtonUnlockedState((int)skillButtonAttributeNumber, skillGradeNumber);
        SkillSystemManager.Instance.onSkillUnlockStateChanged += UpdateButtonSkillUnlockedState;
    }
    

    public void OnSkillButtonClick()
    {
        SkillSystemManager.Instance.ShowDialogue(skillButtonAttributeNumber, skillGradeNumber);
    }

    public void ShowSkillButtonUnlockedState(int skillAttributeNumber, int skillGradeNumber)
    {
        bool isSkillUnlocked = SkillSystemManager.Instance.isSkillUsingUnloked(skillAttributeNumber,skillGradeNumber);
        if (isSkillUnlocked)
        {
            skillButtonImage.color = Color.white;
        }
        else
        {
            skillButtonImage.color = Color.gray;
        }
    }

    public void UpdateButtonSkillUnlockedState(int skillAttributeNumber , int skillGradeNumber )
    {
        if (skillAttributeNumber != (int)skillButtonAttributeNumber || skillGradeNumber != this.skillGradeNumber)
        {
            return;
        }
        
        ShowSkillButtonUnlockedState(skillAttributeNumber,skillGradeNumber);
        
    }
}