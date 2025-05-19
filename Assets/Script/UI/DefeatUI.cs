using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatUIData : BaseUIData
{
    public string waveDefeatText;
    public string maxWaveText;
    public string currentWaveText;
    public string wisdomText;
    public int defeatEarnedWisdomCoefficient;
}

public class DefeatUI : BaseUI
{
    public TextMeshProUGUI waveDefeatText;
    public TextMeshProUGUI maxWaveText;
    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI wisdomText;
    public TextMeshProUGUI currentWaveLevelText;
    public TextMeshProUGUI maxWaveLevelText;
    public TextMeshProUGUI defeatEarnedWisdomText;
    
    private DefeatUIData defeatUIData;

    private int skillCount;
    private int defeatEarnedWisdomCoefficient;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        
        defeatUIData = uiData as DefeatUIData;
        
        waveDefeatText.text = defeatUIData.waveDefeatText;
        maxWaveText.text = defeatUIData.maxWaveText;
        currentWaveText.text = defeatUIData.currentWaveText;
        wisdomText.text = defeatUIData.wisdomText;
    }

    public override void ShowUI()
    {
        currentWaveLevelText.text = GameController.Instance.GetWaveLevel().ToString();
        maxWaveLevelText.text = PlayerPrefs.GetInt("MaxWave", 0).ToString();
        defeatEarnedWisdomText.text = GetDefeatEarnedWisdom().ToString();
        base.ShowUI();
    }

    private int GetDefeatEarnedWisdom()
    {
        // skillCount 차후에 곱해줘야함
        return GameController.Instance.GetWaveLevel()*defeatEarnedWisdomCoefficient;
    }
    
    public void OnClickRestartButton()
    {
        SceneManager.LoadScene(0);
        CloseUI();
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
        CloseUI();
    }
}
