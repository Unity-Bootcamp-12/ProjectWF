using UnityEngine;

public class SkillSettingUIData : BaseUIData
{
    
}

public class SkillSettingUI : BaseUI
{
    public void OnClickSkillButtonExit()
    {
        SoundController.Instance.PlaySFX(SFXType.UIClickSound);
        SkillSystemManager.Instance.SaveSkillDataToJson();
        CloseUI();
    }
}


