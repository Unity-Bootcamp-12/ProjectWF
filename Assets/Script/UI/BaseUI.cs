using System;
using UnityEngine;

[System.Serializable]
public class BaseUIData
{
    //@tk : 개별 UI 클래스 내부에서 열고 닫을 때 뭘 할지를 짜는거 보다, 외부에서 뭘 할지를 넘기는게 더 좋을 수 있어서...
    [NonSerialized]
    public Action OnShow;
    [NonSerialized]
    public Action OnClose;
}

public class BaseUI : MonoBehaviour
{
    //@tk (25.02.23) 이거 dotween일 땐 어떻게 구조 짤지 고민
    // UI가 생성될 때 나오는 애니메이션
    // 우리 프로젝트에서는 필요 없을 듯 함
    public Animation uiShowAnim;

    private Action m_OnShow;
    private Action m_OnClose;

    // 배치할 canvas를 매개변수로 받음
    public virtual void Init(Transform anchor)
    {
        Logger.Info($"{GetType()}::Init");

        m_OnShow = null;
        m_OnClose = null;

        transform.SetParent(anchor);
        
        // RectTransform 초기화 
        //@tk : Q : why recttransform init?
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        //rectTransform.localScale = Vector3.one;
        rectTransform.localScale = anchor.localScale;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector3.zero;
        rectTransform.offsetMax = Vector3.zero;
    }

    // 각 UI가 보여질 때, 없어질 때 실행할 이벤트 초기화
    public virtual void SetInfo(BaseUIData uiData)
    {
        Logger.Info($"{GetType()}::SetInfo");
        
        m_OnShow = uiData.OnShow;
        m_OnClose = uiData.OnClose;
    }

    // 실제로 열어서 UI를 보여주는 함수
    public virtual void ShowUI()
    {
        if(uiShowAnim != null)
        {
            uiShowAnim.Play();
        }

        m_OnShow?.Invoke();
        m_OnShow = null;
    }

    // 실제로 UI를 닫는 함수
    public virtual void CloseUI(bool isCloseAll = false) 
    {
        // 열려 있는 UI를 전부 닫을 필요가 있는 경우 isCloaseAll = true
        if(false == isCloseAll)
        {
            m_OnClose?.Invoke();
        }
        m_OnClose = null;
        UIManager.Instance.CloseUI(this);
    }

    // 거의 모든 UI에 닫기 버튼이 있으므로 닫기 버튼에 대한 함수
    public virtual void OnClickCloseButton()
    {
        // 오디오 매니저 필요
        //AudioManager.Instance.PlaySFX(SFX.ui_button_click);
        CloseUI();
    }
}