using TMPro;
using UnityEngine;

public class PauseUIData : BaseUIData
{
    public string titleText;
    public string resumeButtonText;
    public string exitButtonText;
}

public class PauseUI : BaseUI
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI resumeButtonText;
    public TextMeshProUGUI exitButtonText;

    private PauseUIData pauseUIData;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        pauseUIData = uiData as PauseUIData;
        
        titleText.text = pauseUIData.titleText;
        resumeButtonText.text = pauseUIData.resumeButtonText;
        exitButtonText.text = pauseUIData.exitButtonText;
    }

    public void OnClickResumeButton()
    {
        GameController.Instance.ResumeGame();
        CloseUI();
    }

    public void OnClickExitButton()
    {
        GameController.Instance.ExitGame();
    }
}
