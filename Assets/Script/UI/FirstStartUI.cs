using System;
using TMPro;
using UnityEngine;

[System.Serializable]
public class FirstStartUIData : BaseUIData
{
    public string startButtonText;
}

public class FirstStartUI : BaseUI
{
    public TextMeshProUGUI startButtonText;
    private FirstStartUIData firstStartUIData;
    private Action<int> OnStartButtonClick;

    private void Start()
    {
        OnStartButtonClick += GameController.Instance.ChangeWaveState;
    }

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        firstStartUIData = uiData as FirstStartUIData;

        startButtonText.text = firstStartUIData.startButtonText;
    }

    public void OnClickStartButton()
    {
        OnStartButtonClick?.Invoke((int)WaveState.Ready);
        CloseUI();
    }
}
