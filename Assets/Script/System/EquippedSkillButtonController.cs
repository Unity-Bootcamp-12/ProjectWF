using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class EquippedSkillButtonController : MonoBehaviour
{
    [SerializeField] private int skillIndex;
    private SkillData  skillData;
    private int skillAttribute;
    private int skillGrade;
    private float skillCoolTime;

    private bool isOnCooldown = false;
    private string skillName;
    
    [Header("UI 요소")]
    private Image skillImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image backGroundImage;
    
    [SerializeField] private SkillIndicator skillIndicator;
    private GameObject skillEffectPrefab;
    private bool isSkillLocked = false;

    
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
            skillCoolTime = skillData.skillCoolTime;
            skillName = skillData.skillName;
            skillImage.sprite = SkillSystemManager.Instance.GetSkillSprite(skillAttribute, skillGrade);
            backGroundImage.sprite = skillImage.sprite;
        }
        else
        {
            skillData = null;
            skillImage.sprite = null;
        }
        backGroundImage.sprite = skillImage.sprite;      
        GameController.Instance.OnInGameResetSkillCoolTime += ResetCooldown;
    }

    public void OnSkillButtonClick()
    {
        
        if (isOnCooldown || isSkillLocked || skillData == null) 
        {
            SoundController.Instance.PlaySFX(SFXType.UpgradeNegativeSound);
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
        skillEffectPrefab = Resources.Load<GameObject>($"SkillPrefab/{skillName}");
        GameObject skillPrefab = Instantiate(skillEffectPrefab, spawnPosition, skillEffectPrefab.transform.rotation);
        SoundController.Instance.PlaySkillSFX(skillName);
        SkillController controller = skillPrefab.GetComponent<SkillController>();

        if (controller != null)
        {
            controller.SetSkillDamagePower(skillData.skillDamagePower);
            
            SkillData currentSkill = SkillSystemManager.Instance.equipSkillData[skillIndex];
            EnumSkillAttribute currentAttribute = (EnumSkillAttribute)currentSkill.skillAttribute;
            ElementalAttribute attribute = ElementalAttribute.None;
            if (currentAttribute == EnumSkillAttribute.Fire)
            {
                attribute = ElementalAttribute.Fire;
            }
            else if (currentAttribute == EnumSkillAttribute.Lightning)
            {
                attribute = ElementalAttribute.Lightning;
            }
            else if (currentAttribute == EnumSkillAttribute.Water)
            {
                attribute = ElementalAttribute.Water;
            }
            controller.SetAttribute(attribute);
            controller.SetSkillType((EnumSkillType)skillData.skillType);
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
            if (isOnCooldown == false)
            {
                return;
            }

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
    }   

    private void ResetCooldown()
    {
        isSkillLocked = false;
        isOnCooldown = false;
        skillImage.fillAmount = 0f; 
        cooldownText.text = "";
    }

}
