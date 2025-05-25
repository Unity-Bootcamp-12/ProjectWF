using UnityEngine;

public class SkillSettingUIData : BaseUIData
{
    
}

public class SkillSettingUI : BaseUI
{
    public void OnClickSkillButtonExit()
    {
        SkillSystemManager.Instance.SaveSkillDataToJson();
        CloseUI();
    }
}


