using UnityEngine;
using System;

public class SkillIndicator : MonoBehaviour
{
    [SerializeField] private GameObject targetingPanel;

    private GameObject currentIndicator;
    private Action<Vector3> onTargetConfirmed;
    private bool isTargeting = false;
    
    private Camera mainCamera;
    private float inGameGroundHeight = 1f;
    
    private void Start()
    {
        targetingPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isTargeting)
        {
            return;
        }
        UpdateIndicatorPositionToMouse();

        // 에디터에서 보여지는 경우
        if (Input.GetMouseButtonDown(0))
        {
            // 타겟패널안에 입력할 경우
            if (IsMouseOnPanel())
            {
                Vector3 targetWorldPosition = GetTargetWorldPositionInEditor();
                onTargetConfirmed?.Invoke(targetWorldPosition);
            }
            // 타겟패널 밖에 입력할 경우
            EndTargeting();
        }
        // 모바일에서 보여지는 경우
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchPosition = Input.GetTouch(0).position;
            if (IsTouchOnPanel(touchPosition))
            {
                Vector3 targetWorldPosition = GetTargetWorldPositionInGame(touchPosition);
                onTargetConfirmed?.Invoke(targetWorldPosition);
            }
            EndTargeting();
        }
    }

    public void StartTargeting(GameObject indicatorPrefab, Action<Vector3> onConfirm)
    {
        if (isTargeting)
        {
            return;
        }


        isTargeting = true;
        targetingPanel.SetActive(true);
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        
        onTargetConfirmed = onConfirm;
        currentIndicator = Instantiate(indicatorPrefab, targetingPanel.transform);

        UpdateIndicatorPositionToMouse();
    }
    
    private void UpdateIndicatorPositionToMouse()
    {
        Vector2 localPoint;
        RectTransform panelRect = targetingPanel.GetComponent<RectTransform>();
        RectTransform indicatorRect = currentIndicator.GetComponent<RectTransform>();

        // 마우스 위치를 패널 기준 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            Input.mousePosition,
            null,
            out localPoint
        );

        // 인디케이터 크기 맞춤으로 제작
        Vector2 clampedPosition = ClampToPanel(panelRect, indicatorRect, localPoint);

        // 위치 적용
        indicatorRect.localPosition = clampedPosition;
    }
    
    private Vector2 ClampToPanel(RectTransform panel, RectTransform indicator, Vector2 localPos)
    {
        Vector2 panelSize = panel.rect.size;
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
        isTargeting = false;
        targetingPanel.SetActive(false);

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        onTargetConfirmed = null;
        
        // 슬로우 모션 해제
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
    
    // 에디터에서 사용
    private Vector3 GetTargetWorldPositionInEditor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
