using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class EquippedSkillButtonController : MonoBehaviour
{
    [SerializeField] private int skillIndex;
    private SkillData  skillData;
    private int skillAttribute;
    private int skillGrade;
    private float skillCoolTime;
    private string skillPrefabPath;
    private bool isOnCooldown = false;
    
    [Header("UI 요소")]
    private Image skillImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image backGroundImage;
    
    [SerializeField] private SkillIndicator skillIndicator;
    private GameObject skillEffectPrefab;
    private bool isSkillLocked = false;

    private string skillname;
    
    private void Awake()
    {
        skillImage = GetComponent<Image>();
        
        if (SkillSystemManager.Instance.equipSkillData[skillIndex] != null)
        {
            skillData = SkillSystemManager.Instance.equipSkillData[skillIndex];
            skillAttribute = skillData.skillAttribute;
            skillGrade = skillData.skillGrade;
            skillCoolTime = skillData.skillCoolTime;
            skillname = skillData.skillName;
            skillImage.sprite = SkillSystemManager.Instance.GetSkillSprite(skillAttribute, skillGrade);
            backGroundImage.sprite = skillImage.sprite;
            skillPrefabPath = skillData.skillPrefabPath;

        }
        else
        {
            skillData = null;
            skillImage.sprite = null;
        }
    }

    public void OnSkillButtonClick()
    {
        if (isOnCooldown || isSkillLocked) 
        {
            return;   
        }
        skillData = SkillSystemManager.Instance.equipSkillData[skillIndex];
        skillIndicator.SetSkillIndex(skillIndex);
        
        skillIndicator.StartTargeting(OnTargetConfirmed);        
        GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    }
    
    private void OnTargetConfirmed(Vector3 targetPos)
    {
        SoundController.Instance.PlaySFX(SFXType.CastSound);
        Vector3 spawnPosition = skillIndicator.GetCurrentTargetPosition();
        skillEffectPrefab = Resources.Load<GameObject>(skillPrefabPath);
        GameObject skillPrefab = Instantiate(skillEffectPrefab, spawnPosition, skillEffectPrefab.transform.rotation);
        SkillController controller = skillPrefab.GetComponent<SkillController>();
        if (controller != null)
        {
            controller.SetSkillDamagePower(skillData.skillDamagePower);
        }
        GetComponent<Image>().color = Color.black;
            
        isOnCooldown = true;
        StartCooldown().Forget();
    }
    
    // 프레임마다 대기하며 쿨타임 UI 갱신
    private async UniTaskVoid StartCooldown()
    {
        float timer = skillCoolTime;

        while (timer > 0f)
        {
            if (isSkillLocked)
            {
                return;                
            }
            timer -= Time.deltaTime;

            float ratio = Mathf.Clamp01(timer / skillCoolTime);
            skillImage.fillAmount = ratio;
            cooldownText.text = Mathf.CeilToInt(timer).ToString();

            await UniTask.Yield();            
        }

        // 쿨다운 완료
        isOnCooldown = false;
        skillImage.fillAmount = 0f;
        cooldownText.text = "";

        backGroundImage.color = Color.white;
        GetComponent<Image>().color = Color.white;
    }   

    private void ResetCooldown()
    {
        isSkillLocked = true;
        isOnCooldown = false;
        skillImage.fillAmount = 0f;
        cooldownText.text = "";
        DelayIsEndGame().Forget();
        backGroundImage.color = Color.white;
        GetComponent<Image>().color = Color.white;
    }

    private void InGameSkill()
    {
        DelayInGameSkill().Forget();
    }

    private async UniTask DelayInGameSkill()
    {
        isSkillLocked = true;
        await UniTask.Delay(5000);
        isSkillLocked = false;
    }

    private async UniTask DelayIsEndGame()
    {
        await UniTask.Delay(1000);
        isSkillLocked = false;
    }
}
