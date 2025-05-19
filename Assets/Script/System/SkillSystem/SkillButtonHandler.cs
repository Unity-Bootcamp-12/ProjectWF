using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Cysharp.Threading.Tasks;

public class SkillButtonHandler : MonoBehaviour
{
    [Header("쿨타임 설정")] 
    [SerializeField] private float skillCooltime = 8f;
    private bool isOnCooldown = false;
    
    [Header("UI 요소")]
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image backGroundImage;
    
    [Header("스킬 인디케이터 시스템")]
    [SerializeField] private SkillIndicator skillIndicator;

    [Header("스킬별 인디케이터 & 이펙트 프리팹")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject skillEffectPrefab;
    private bool isSkillLocked = false;

    private void OnEnable()
    {
        GameController.Instance.OnSkillReset += ResetCooldown;
        GameController.Instance.OnInGameStart += InGameSkill;
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSkillReset -= ResetCooldown;
        }
    }
    
    private void Start()
    {
        backGroundImage.sprite = cooldownFillImage.sprite;
        InGameSkill();
    }

    public void OnSkillButtonClick()
    {
        if (isOnCooldown || isSkillLocked) 
        {
            return;   
        }
        // 타겟팅 시작
        skillIndicator.StartTargeting(indicatorPrefab, OnTargetConfirmed);        
        GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    }
    
    private void OnTargetConfirmed(Vector3 targetPos)
    {
        Instantiate(skillEffectPrefab, targetPos, Quaternion.identity);
        
        GetComponent<Image>().color = Color.black;
            
        isOnCooldown = true;
        StartCooldown().Forget();
    }
    
    // 프레임마다 대기하며 쿨타임 UI 갱신
    private async UniTaskVoid StartCooldown()
    {
        float timer = skillCooltime;

        while (timer > 0f)
        {
            if (isSkillLocked)
            {
                return;                
            }
            timer -= Time.deltaTime;

            float ratio = Mathf.Clamp01(timer / skillCooltime);
            cooldownFillImage.fillAmount = ratio;
            cooldownText.text = Mathf.CeilToInt(timer).ToString();

            await UniTask.Yield();            
        }

        // 쿨다운 완료
        isOnCooldown = false;
        cooldownFillImage.fillAmount = 0f;
        cooldownText.text = "";

        backGroundImage.color = Color.white;
        GetComponent<Image>().color = Color.white;
    }   

    private void ResetCooldown()
    {
        isSkillLocked = true;
        isOnCooldown = false;
        cooldownFillImage.fillAmount = 0f;
        cooldownText.text = "";
        DelayIsEndGame().Forget();
        // 배경색 복원 (기본 상태)
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
        await UniTask.Delay(3000);
        isSkillLocked = false;
    }

    private async UniTask DelayIsEndGame()
    {
        await UniTask.Delay(1000);
        isSkillLocked = false;
    }

}