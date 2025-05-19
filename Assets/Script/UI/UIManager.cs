using System;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    FirstStartUI,
    StartUI,
    ReadyUI,
    ProgressUI,
    ClearUI,
    DefeatUI,
    
}

public class UIManager : SingletonBehaviour<UIManager>
{
    // UI 화면을 렌더링할 canvas
    public Transform UICanvasTransform;
    // 비활성화할 canvas
    public Transform ClosedUITransform;

    //현재 가장 상단의 UI
    private BaseUI m_FrontUI;
    
    // 현재 열려있는 화면을 담고 있는 UI Pool
    private Dictionary<Type, GameObject> m_OpenUIPool = new Dictionary<Type, GameObject>();
    // 현재 닫혀있는 화면을 담고 있는 UI Pool
    private Dictionary<Type, GameObject> m_ClosedUIPool = new Dictionary<Type, GameObject>();

    // 열고 싶은 UI 화면의 실제 instance를 가져오는 함수
    private BaseUI GetUI<T>(out bool isAlreadyOpen)
    {
        Type uiType = typeof(T);
        BaseUI ui = null;
        isAlreadyOpen = false;

        if(m_OpenUIPool.ContainsKey(uiType) == true)
        {
            ui = m_OpenUIPool[uiType].GetComponent<BaseUI>();
            isAlreadyOpen = true;
        }
        else if(m_ClosedUIPool.ContainsKey(uiType) == true)
        {
            ui = m_ClosedUIPool[uiType].GetComponent<BaseUI>();
            m_ClosedUIPool.Remove(uiType);
        }
        else
        {
            //@tk 모든 UI들이 미리 씬에 세팅되어 있지 않고, 동적으로 처리
            // 아직 풀에 없으면 Resources파일에서 동적으로 가져옴
            var uiObj = Instantiate(Resources.Load($"UI/{uiType}", typeof(GameObject))) as GameObject;
            ui = uiObj.GetComponent<BaseUI>();
        }

        return ui;
    }

    public void OpenUI<T>(BaseUIData uiData)
    {
        Type uiType = typeof(T);
        bool isAlreadyOpen = false;
        var ui = GetUI<T>(out isAlreadyOpen);

        // 두 번의 유효성 검사 실행
        if (ui == null)
        {
            Logger.Error($"{uiType} prefab doesn't exist in Resources");
            return;
        }
        
        if(isAlreadyOpen == true)
        {
            return;
        }

        var siblingIndex = UICanvasTransform.childCount;
        ui.Init(UICanvasTransform);
        ui.transform.SetSiblingIndex(siblingIndex);
        ui.gameObject.SetActive(true);
        ui.SetInfo(uiData);
        ui.ShowUI();

        m_FrontUI = ui;
        m_OpenUIPool.Add(uiType, ui.gameObject);
    }

    public void CloseUI(BaseUI ui)
    {
        Type uiType = ui.GetType();
        ui.gameObject.SetActive(false);
        m_OpenUIPool.Remove(uiType);
        m_ClosedUIPool[uiType] = ui.gameObject;
        ui.transform.SetParent(ClosedUITransform);

        // m_FrontUI = null;
        // // 최상단 UI 초기화
        // var lastChild = UICanvasTransform.GetChild(UICanvasTransform.childCount - 1);
        // if (lastChild != null)
        // {
        //     m_FrontUI = lastChild.gameObject.GetComponent<BaseUI>();
        // }
    }

    private void Start()
    {
        // JSON 형태의 파일로 Dictionary에 저장
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("JsonData/UI");
        foreach (var jsonFile in jsonFiles)
        {
            // 파일 이름을 enum 타입으로 변환
            if (Enum.TryParse(jsonFile.name, out UIType type))
            {
                try
                {
                    BaseUIData data = new();
                    switch (type)
                    {
                        case UIType.FirstStartUI:
                            data = JsonUtility.FromJson<FirstStartUIData>(jsonFile.text);
                            break;
                        case UIType.StartUI:
                            data = JsonUtility.FromJson<StartUIData>(jsonFile.text);
                            break;
                        case UIType.ReadyUI:
                            data = JsonUtility.FromJson<ReadyUIData>(jsonFile.text);
                            break;
                        case UIType.ClearUI:
                            data = JsonUtility.FromJson<ClearUIData>(jsonFile.text);
                            break;
                        case UIType.DefeatUI:
                            data = JsonUtility.FromJson<DefeatUIData>(jsonFile.text);
                            break;
                        case UIType.ProgressUI:
                            data = JsonUtility.FromJson<ProgressUIData>(jsonFile.text);
                            break;
                    }
                    GameController.Instance.uiDataDictionary[type] = data;
                }
                catch (Exception e)
                {
                    Logger.Error($"JSON 변환 실패: {jsonFile.name} - {e.Message}");
                }
            }
            else
            {
                Logger.Warning($"Enum 변환 실패: {jsonFile.name}");
            }
        }
    }
}