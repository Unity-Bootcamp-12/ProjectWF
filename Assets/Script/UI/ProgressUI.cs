using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIData : BaseUIData
{
    public string waveText;
}

public class ProgressUI : BaseUI
{
    public TextMeshProUGUI waveText;
    public Slider fortressHPSlider;
    private ProgressUIData progressUIData;
    
    private void Start()
    {
        GameController.Instance.OnChangeFortressHP += ChangeHPSlider;
        GameController.Instance.OnEndProgress += CloseUI;
    }

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        progressUIData = uiData as ProgressUIData;
        
        waveText.text = progressUIData.waveText;
    }

    public override void ShowUI()
    {
        int waveLevel = GameController.Instance.GetWaveLevel();
        waveText.text = $"Wave {waveLevel}";
        base.ShowUI();
    }

    private void ChangeHPSlider(int currentFortressHP, int maxFortressHP)
    {
        fortressHPSlider.value = (float)currentFortressHP /maxFortressHP;
    }

    public void OnClickPauseButton()
    {
        GameController.Instance.PauseGame();
        UIManager.Instance.OpenUI<PauseUI>(GameController.Instance.uiDataDictionary[UIType.PauseUI]);
    }
}
