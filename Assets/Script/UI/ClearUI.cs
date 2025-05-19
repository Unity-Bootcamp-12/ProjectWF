using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearUIData : BaseUIData
{
    public string waveClearText;
    public string waveLevelText;
    public string nextWaveButtonText;
    public int earnedWisdomCoefficient;
    public int defaultEarnedWisdom;
}

public class ClearUI : BaseUI
{
    public TextMeshProUGUI waveClearText;
    public TextMeshProUGUI waveLevelText;
    public TextMeshProUGUI clearEarnedWisdomText;
    public TextMeshProUGUI nextWaveButtonText;

    private int earnedWisdomCoefficient;
    private int defaultEarnedWisdom;
    private ClearUIData clearUIData;
    private Action<int> OnNextWaveButtonClick;

    private void Start()
    {
        OnNextWaveButtonClick += GameController.Instance.ChangeWaveState;
    }
    
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        
        clearUIData = uiData as ClearUIData;
        
        waveClearText.text = clearUIData.waveClearText;
        waveLevelText.text = clearUIData.waveLevelText;
        nextWaveButtonText.text = clearUIData.nextWaveButtonText;
        earnedWisdomCoefficient =  clearUIData.earnedWisdomCoefficient;
        defaultEarnedWisdom = clearUIData.defaultEarnedWisdom;
    }

    public override void ShowUI()
    {
        waveLevelText.text = GameController.Instance.GetWaveLevel().ToString();
        clearEarnedWisdomText.text = GetEarnedWisdom().ToString();
        base.ShowUI();
    }

    public void OnClickNextWaveButton()
    {
         OnNextWaveButtonClick?.Invoke((int)WaveState.Ready);
         CloseUI();
    }

    private int GetEarnedWisdom()
    {
        int currentWave = GameController.Instance.GetWaveLevel();
        return defaultEarnedWisdom + earnedWisdomCoefficient *  currentWave;
    }
}
