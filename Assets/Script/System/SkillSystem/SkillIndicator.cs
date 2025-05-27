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
    [SerializeField]private Canvas targetingPanel;
    [SerializeField]private Image rectangleSkillIndicatorPrefab;
    [SerializeField]private Image circleSkillIndicatorPrefab;
    
    private IndicatorType indicatorType;
    private Image currentIndicator;
    private Action<Vector3> onTargetConfirmed;
    private bool isTargeting = false;
    
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
    
    public void SetSkillIndex(int index)
    {
        skillIndex = index;
        
        if (SkillSystemManager.Instance.equipSkillData[skillIndex] != null)
        {
            skillData = SkillSystemManager.Instance.equipSkillData[skillIndex];
            skillRangeVertical = skillData.skillRangeVertical;
            skillRangeHorizontal = skillData.skillRangeHorizontal;
            skillRangeRadius = skillData.skillRangeRadius;
            skillAttribute = skillData.skillAttribute;
            skillTargetType = (EnumSkillTargetType)skillData.skillTargetType;
            SwitchBySkillType();
        }
        else
        {
            skillData = null;
        }
    }

    private void SwitchBySkillType()
    {
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

    private void OnEnable()
    {
        GameController.Instance.OnSkillReset += EndTargeting;
    }
    
    private void Start()
    {
        targetingPanel.enabled = false;
    }

    private void Update()
    {
        if (!isTargeting)
        {
            return;
        }
        
        #if UNITY_EDITOR

        UpdateIndicatorPositionToMouse();

        // 에디터에서 보여지는 경우
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOnPanel())
            {
                Vector3 targetWorldPosition = GetTargetWorldPositionInEditor();
                onTargetConfirmed?.Invoke(targetWorldPosition);
            }
            // 타겟패널 밖에 입력할 경우
            EndTargeting();
        }
        #else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                UpdateIndicatorPositionToTouch();
            }
            
            if (touch.phase == TouchPhase.Ended)
            {
                if (IsTouchOnPanel(touch.position))
                {
                    Vector3 worldPos = GetTargetWorldPositionInGame(touch.position);
                    onTargetConfirmed?.Invoke(worldPos);
                }
                EndTargeting();
            }
        }
        #endif
    }

    private Image SkillIndicatorPrefab(Image skillIndicatorPrefab, int skillRangeVertical, int skillRangeHorizontal, int skillRangeRadius, int skillAttribute)
    {
        if (skillIndicatorPrefab == null)
        {
            return null;
        }

        RectTransform rectangleSkillIndicatorPrefabRectTransform = skillIndicatorPrefab.GetComponent<RectTransform>();
        RectTransform targetingPanelRectTransform = targetingPanel.GetComponent<RectTransform>();
        float canvasWidth = targetingPanelRectTransform.rect.width;
        float canvasHeight = targetingPanelRectTransform.rect.height;
        switch (indicatorType)
        {
            case IndicatorType.Rectangle:
                if (skillRangeHorizontal > skillRangeVertical)
                {
                    rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(canvasWidth, skillRangeVertical);
                }
                else if (skillRangeHorizontal < skillRangeVertical)
                {
                    rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(skillRangeHorizontal, canvasHeight);
                }
                else if (skillRangeHorizontal == skillRangeVertical)
                {
                    rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight);
                }

                break;
            case IndicatorType.Circle:
                float diameter = skillRangeRadius;
                rectangleSkillIndicatorPrefabRectTransform.sizeDelta = new Vector2(diameter*100, diameter*100);
                break;
            case IndicatorType.None:
                skillIndicatorPrefab.gameObject.SetActive(false);
                break;
            default:
                skillIndicatorPrefab.gameObject.SetActive(false);
                break;
        }
        
        Color indicatorColor = Color.white;
        switch (skillAttribute)
        {
            case 0:
                indicatorColor = new Color(1f, 0f, 0f, 0.2f);
                break;
            case 1:
                indicatorColor = new Color(0f, 1f, 1f, 0.2f);
                break;
            case 2:
                indicatorColor = new Color(0f, 0f, 1f, 0.2f);
                break;
            default:
                indicatorColor = Color.white;
                break;
        }

        skillIndicatorPrefab.color = indicatorColor;

        return skillIndicatorPrefab;
        
    }

    public void StartTargeting(Action<Vector3> onConfirm)
    {
        if (isTargeting)
        {
            return;
        }

        isTargeting = true;
        targetingPanel.enabled = true;
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        
        onTargetConfirmed = onConfirm;

        SwithIndicatorType();
        
    }

    private void SwithIndicatorType()
    {
        if (indicatorType == IndicatorType.Circle)
        {
            var prefab = SkillIndicatorPrefab(circleSkillIndicatorPrefab, skillRangeVertical, skillRangeHorizontal, skillRangeRadius, skillAttribute);
            if (prefab != null)
            {
                currentIndicator = Instantiate(prefab, targetingPanel.transform);
            }
        }
        else if (indicatorType == IndicatorType.Rectangle)
        {
            var prefab = SkillIndicatorPrefab(rectangleSkillIndicatorPrefab, skillRangeVertical, skillRangeHorizontal, skillRangeRadius, skillAttribute);
            if (prefab != null)
            {
                currentIndicator = Instantiate(prefab, targetingPanel.transform);
            }
        }
        
    }

    private void UpdateIndicatorPositionToMouse()
    {
        if (indicatorType == IndicatorType.None)
        {
            return;
        }

        Vector2 localPoint;
        RectTransform imageRect = targetingPanel.GetComponent<RectTransform>();
        RectTransform indicatorRect = currentIndicator.GetComponent<RectTransform>();

        // 마우스 위치를 패널 기준 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRect,
            Input.mousePosition,
            null,
            out localPoint
        );

        // 인디케이터 크기 맞춤으로 제작
        Vector2 clampedPosition = ClampToPanel(imageRect, indicatorRect, localPoint);

        // 위치 적용
        indicatorRect.localPosition = clampedPosition;
    }
    
    private void UpdateIndicatorPositionToTouch()
    {
        if (indicatorType == IndicatorType.None|| Input.touchCount == 0)
        {
            return;
        }

        Vector2 localPoint;
        RectTransform imageRect = targetingPanel.GetComponent<RectTransform>();
        RectTransform indicatorRect = currentIndicator.GetComponent<RectTransform>();

        // 마우스 위치를 패널 기준 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRect,
            Input.touches[0].position,
            null,
            out localPoint
        );

        // 인디케이터 크기 맞춤으로 제작
        Vector2 clampedPosition = ClampToPanel(imageRect, indicatorRect, localPoint);

        // 위치 적용
        indicatorRect.localPosition = Vector2.SmoothDamp(
            indicatorRect.localPosition,
            clampedPosition,
            ref indicatorVelocity,
            0.05f);
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

    // 에디터용
    private bool IsMouseOnPanel()
    {
        RectTransform panelRect = targetingPanel.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition);
    }

    // 모바일용
    private bool IsTouchOnPanel(Vector2 touchPosition)
    {
        RectTransform panelRect = targetingPanel.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(panelRect, touchPosition);
    }

    private void EndTargeting()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
        isTargeting = false;
        targetingPanel.enabled = false;

        onTargetConfirmed = null;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    public Vector3 GetCurrentTargetPosition()
    {
        return SwithSkillTargetPosition();
    } 

    private Vector3 SwithSkillTargetPosition()
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
                Vector3 targetMousePos = GetTargetWorldPositionInEditor();
                spawnPosition = new Vector3(forwardPos.x, forwardPos.y, targetMousePos.z);
                break;
            case EnumSkillTargetType.ByMousePoint:
                spawnPosition = GetTargetWorldPositionInEditor();
                break;
        }

        return spawnPosition;

    }

    // 에디터에서 사용
    private Vector3 GetTargetWorldPositionInEditor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 30);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return new Vector3(hit.point.x, inGameGroundHeight, hit.point.z);
        }

        return Vector3.zero;
    }
    
    // 모바일에서 사용
    private Vector3 GetTargetWorldPositionInGame(Vector2 touchScreenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return new Vector3(hit.point.x, inGameGroundHeight, hit.point.z);
        }

        return Vector3.zero;
    }
}
