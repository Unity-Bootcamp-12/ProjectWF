using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    [SerializeField] private int skillButtonAttributeNumber;
    [SerializeField] private int skillGradeNumber;
    [SerializeField] private string skillName;
    [SerializeField] private int skillCoolTime;
    [SerializeField] string skillExplainText;
    [SerializeField] private int skillLevel;
    [SerializeField] private int skillAttribute;
    [SerializeField] private int skillDamagePower;
    
    public Image skillButtonImage;
    private SkillSystemManager.SkillData skillButtonStatus;
    private void Start()
    {
        Logger.Info($" 스킬 변수명 :{gameObject.name},{skillButtonAttributeNumber},{skillGradeNumber}");
         skillButtonImage =GetComponent<Image>();
         SetSkillButtonStatus();
         skillButtonImage.sprite = SkillSystemManager.Instance.GetSkillSprite(skillButtonAttributeNumber, skillGradeNumber); 
    }
    public void SetSkillButtonStatus()
    {
        skillButtonStatus = SkillSystemManager.Instance.GetSkillData(skillButtonAttributeNumber,skillGradeNumber);
       
        
        skillCoolTime = skillButtonStatus.skillCoolTime;
        skillExplainText = skillButtonStatus.skillExplainText;
        skillLevel = skillButtonStatus.skillLevel;
        skillAttribute = skillButtonStatus.skillAttribute;
        skillDamagePower = skillButtonStatus.skillDamagePower;
    }


    public void TransferInfoSkillButtonToDialogue()
    {
        Logger.Info("버튼 눌림 확인");
       
    }

    public void TransferIndex(int attributeIndex, int gradeIndex)
    {
        
        
    }

}






