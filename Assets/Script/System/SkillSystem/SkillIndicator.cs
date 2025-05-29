using UnityEngine;
using System;
using UnityEngine.UI;

public enum IndicatorType
{
    None,
    Rectangle,
    Circle
    
}

public class SkillIndicator : MonoBehaviour
{
    [SerializeField]private RectTransform targetingPanel;
    [SerializeField]private Image rectangleSkillIndicatorPrefab;
    [SerializeField]private Image heightRectangleSkillIndicatorPrefab;
    [SerializeField]private Image widthRectangleSkillIndicatorPrefab;
    [SerializeField]private Image circleSkillIndicatorPrefab;
    
    private IndicatorType indicatorType;
    private Image currentIndicator;
    
    private Camera mainCamera;
    private float inGameGroundHeight = 1.5f;
    
    private SkillData skillData;
    private int skillIndex;
    private int skillRangeVertical; 
    private int skillRangeHorizontal; 
    private int skillRangeRadius;
    private int skillAttribute;
    
    private EnumSkillTargetType skillTargetType;
    [SerializeField] private Transform playerTransform;
    private Vector2 indicatorVelocity;

    private int widthResolution;
    private int heightResolution;

    private Touch touch;
    private bool isSkillClicked = false;
    
    private GameObject skillEffectPrefab;
    private string skillName;
    private void SwitchBySkillType()
    {
        Logger.Info($"{skillRangeVertical}, {skillRangeHorizontal}, {skillRangeRadius}");
        if (skillRangeVertical > 0 && skillRangeHorizontal > 0 && skillRangeRadius > 0)
        {
            indicatorType = IndicatorType.Circle;
        }
        else if (skillRangeVertical > 0 && skillRangeHorizontal > 0 && skillRangeRadius == 0)
        {
            indicatorType = IndicatorType.Rectangle;
        }
        else if (skillRangeVertical == 0 && skillRangeHorizontal == 0 && skillRangeRadius == 0)
        {
            indicatorType = IndicatorType.None;
        }
    }

    private void Awake()
    {
        // HACK: 코드 수정을 최소화하기 위한 코드
        targetingPanel = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameController.Instance.OnSkillReset += EndTargeting;
    }

    private void OnDisable()
    {
        GameController.Instance.OnSkillReset -= EndTargeting;
    }

    public void Open(int skillIndex)
    {
        this.skillIndex = skillIndex;
        SetSkillData(SkillSystemManager.Instance.GetUsedSkillData());
        gameObject.SetActive(true);
    }

    private Vector2 _recentScreenPosition;
    private void Update()
    {
#if UNITY_EDITOR
        // 마우스 입력 (에디터 전용)
        if (Input.GetMouseButtonDown(0))
        {
            CreateIndicator();
        }
        else if (Input.GetMouseButton(0))
        {
            _recentScreenPosition = Input.mousePosition;
            UpdateIndicatorPosition(_recentScreenPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (IsValidToUseSkill(_recentScreenPosition))
            {
                Vector3 targetWorldPosition = GetTargetWorldPositionFrom(_recentScreenPosition);
                ConfirmTarget(targetWorldPosition);
                SkillSystemManager.Instance.StartUseSkill(skillIndex);
            }
            EndTargeting();
        }
#elif UNITY_IOS || UNITY_ANDROID
    // 터치 입력 (iOS/Android)
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        _recentScreenPosition = touch.position;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                CreateIndicator();
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                UpdateIndicatorPosition(_recentScreenPosition);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (IsValidToUseSkill(_recentScreenPosition))
                {
                    Vector3 targetWorldPosition = GetTargetWorldPositionFrom(_recentScreenPosition);
                    ConfirmTarget(targetWorldPosition);
                    SkillSystemManager.Instance.StartUseSkill(skillIndex);
                }
                EndTargeting();
                break;
        }
    }
#endif

    }

    
    public void SetSkillData(SkillData skillData)
    {
        Time.timeScale = 0.5f;

        this.skillData = skillData;
        skillName = skillData.skillName;
        skillRangeRadius = skillData.skillRangeRadius;
        skillRangeHorizontal = skillData.skillRangeHorizontal;
        skillRangeVertical = skillData.skillRangeVertical;
        skillAttribute = skillData.skillAttribute;
        skillTargetType = (EnumSkillTargetType)skillData.skillTargetType;
        Logger.Info($"Indicator start {skillName}");
        
        SwitchBySkillType();
    }

    private void ConfirmTarget(Vector3 targetPos)
    {
        SoundController.Instance.PlaySFX(SFXType.CastSound);
        // TODO: 스킬에 따라 스폰 위치 다시 결정해야 함.
        // Vector3 spawnPosition = GetCurrentTargetPosition();
        Vector3 spawnPosition = SwitchSkillTargetPosition(targetPos);
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
    }

    private RectTransform targetingPanelRectTransform;
    
    private void CreateIndicator()
    {
        if (indicatorType == IndicatorType.Circle)
        {
            //TODO: 인디케이터 인스턴싱 후 색상, 크기 조정 등
            var prefab = circleSkillIndicatorPrefab;
            if (prefab != null)
            {
                currentIndicator = Instantiate(prefab, targetingPanel.transform);
            }
        }
        else if (indicatorType == IndicatorType.Rectangle)
        {
            var prefab = rectangleSkillIndicatorPrefab;
            
            if (skillRangeHorizontal > skillRangeVertical)
            {
               prefab = widthRectangleSkillIndicatorPrefab;
            }
            else if (skillRangeHorizontal < skillRangeVertical)
            {
                prefab = heightRectangleSkillIndicatorPrefab;
            }
            
            if (prefab != null)
            {
                currentIndicator = Instantiate(prefab, targetingPanel.transform);
            }
        }
        ChangeImageColor(currentIndicator);
    }

    private void ChangeImageColor(Image skillIndicatorImage)
    {
        if (skillIndicatorImage == null)
        {
            return;
        }
        
        switch (skillAttribute)
        {
            case 0:
                skillIndicatorImage.color = Color.red;
                break;
            case 1:
                skillIndicatorImage.color = Color.yellow;
                break;
            case 2:
                skillIndicatorImage.color = Color.blue;
                break;
            default:
                break;
        }
        
        Color color = skillIndicatorImage.color;
        color.a = 0.2f;
        skillIndicatorImage.color = color;
    }
    
    // private Image SkillIndicatorPrefab(Image skillIndicatorPrefab)
    // {
    //     // return Instantiate(skillIndicatorPrefab);
    //
    //     RectTransform rectangleSkillIndicatorPrefabRectTransform = skillIndicatorPrefab.GetComponent<RectTransform>();
    //     
    //     RectTransform targetingPanelRectTransform = targetingPanel.GetComponent<RectTransform>();
    //     heightResolution = Screen.height;
    //     widthResolution = Screen.width;
    //
    //     Vector2 offsetMin = targetingPanelRectTransform.offsetMin;
    //     offsetMin.y = heightResolution * 0.2f;
    //     targetingPanelRectTransform.offsetMin = offsetMin;
    //     
    //     float canvasWidth = targetingPanelRectTransform.rect.width;
    //     float canvasHeight = targetingPanelRectTransform.rect.height;
    //     switch (indicatorType)
    //     {
    //         case IndicatorType.Rectangle:
    //             if (skillRangeHorizontal > skillRangeVertical)
    //             {
    //                 rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight * 0.1f);
    //             }
    //             else if (skillRangeHorizontal < skillRangeVertical)
    //             {
    //                 rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(canvasWidth * 0.2f, canvasHeight);
    //             }
    //             else if (skillRangeHorizontal == skillRangeVertical)
    //             {
    //                 rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight);
    //             }
    //
    //             break;
    //         case IndicatorType.Circle:
    //             float diameter = skillRangeRadius;
    //             float coefficient = widthResolution * (float)100 / 1080;
    //             rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(diameter*coefficient, diameter*coefficient);
    //             break;
    //         case IndicatorType.None:
    //             skillIndicatorPrefab.gameObject.SetActive(false);
    //             break;
    //         default:
    //             skillIndicatorPrefab.gameObject.SetActive(false);
    //             break;
    //     }
    //     
    //     Color indicatorColor = Color.white;
    //     switch (skillAttribute)
    //     {
    //         case 0:
    //             indicatorColor = new Color(1f, 0f, 0f, 0.2f);
    //             break;
    //         case 1:
    //             indicatorColor = new Color(0f, 1f, 1f, 0.2f);
    //             break;
    //         case 2:
    //             indicatorColor = new Color(0f, 0f, 1f, 0.2f);
    //             break;
    //         default:
    //             indicatorColor = Color.white;
    //             break;
    //     }
    //
    //     skillIndicatorPrefab.color = indicatorColor;
    //
    //     return skillIndicatorPrefab;
    //     
    // }


    private void UpdateIndicatorPosition(Vector2 screenPosition)
    {
        if (indicatorType == IndicatorType.None)
        {
            return;
        }

        Vector2 localPoint;
        RectTransform imageRect = targetingPanel.GetComponent<RectTransform>();
        RectTransform indicatorRect = currentIndicator.GetComponent<RectTransform>();

        // 전달받은 스크린 좌표를 패널 기준 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRect,
            screenPosition,
            null,
            out localPoint
        );

        // 인디케이터 크기 맞춤으로 제작
        Vector2 clampedPosition = ClampToPanel(imageRect, indicatorRect, localPoint);

        // 위치 적용
        indicatorRect.localPosition = clampedPosition;
    }
    
    private Vector2 ClampToPanel(RectTransform image, RectTransform indicator, Vector2 localPos)
    {
        Vector2 panelSize = image.rect.size;
        Vector2 indicatorSize = indicator.rect.size;

        float halfPanelWidth = panelSize.x / 2f;
        float halfPanelHeight = panelSize.y / 2f;

        float halfIndicatorWidth = indicatorSize.x / 2f;
        float halfIndicatorHeight = indicatorSize.y / 2f;

        float clampedX = Mathf.Clamp(localPos.x, -halfPanelWidth + halfIndicatorWidth, halfPanelWidth - halfIndicatorWidth);
        float clampedY = Mathf.Clamp(localPos.y, -halfPanelHeight + halfIndicatorHeight, halfPanelHeight - halfIndicatorHeight);

        return new Vector2(clampedX, clampedY);
    }

    private bool IsValidToUseSkill(Vector2 screenPosition)
    {
        var panelRect = targetingPanel.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(panelRect, screenPosition);
    }

    private void EndTargeting()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
        
        Time.timeScale = 1f;
        
        gameObject.SetActive(false);
    }

    private Vector3 SwitchSkillTargetPosition(Vector3 position)
    {
        Vector3 spawnPosition = Vector3.zero;

        switch (skillTargetType)
        {
            case EnumSkillTargetType.Buff:
                spawnPosition = playerTransform.position;
                break;
            case EnumSkillTargetType.InFrontOfPlayer:
                float distance = 2f; 
                Vector3 forwardPos = playerTransform.position + playerTransform.forward * distance;
                Vector3 targetMousePos = GetTargetWorldPositionFrom(_recentScreenPosition);
                spawnPosition = new Vector3(forwardPos.x, forwardPos.y, targetMousePos.z);
                break;
            case EnumSkillTargetType.ByMousePoint:
                spawnPosition = GetTargetWorldPositionFrom(_recentScreenPosition);
                break;
        }

        return spawnPosition;

    }

    // 에디터에서 사용
    private Vector3 GetTargetWorldPositionFrom(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return new Vector3(hit.point.x, inGameGroundHeight, hit.point.z);
        }
        return Vector3.zero;
    }
}

