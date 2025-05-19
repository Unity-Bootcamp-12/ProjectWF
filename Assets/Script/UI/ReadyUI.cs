using System;
using TMPro;
using UnityEngine;

public class ReadyUIData : BaseUIData
{
    public string hpUpgradeButtonText;
    public string attackPowerUpgradeButtonText;
    public string skillButtonText;
    public string readyButtonText;
}

public class ReadyUI : BaseUI
{
    public TextMeshProUGUI hpUpgradeButtonText;
    public TextMeshProUGUI attackPowerUpgradeButtonText;
    public TextMeshProUGUI skillButtonText;
    public TextMeshProUGUI readyButtonText;
    
    private ReadyUIData readyUIData;
    private Action<int> OnReadyButtonClick;
    private Action OnSkillButtonClick;
    private Action OnHPUpgradeButtonClick;
    private Action OnAttackPowerUpgradeButtonClick;

    private void Start()
    {
        OnReadyButtonClick += GameController.Instance.ChangeWaveState;
    }

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        
        readyUIData = uiData as ReadyUIData;
        
        hpUpgradeButtonText.text = readyUIData.hpUpgradeButtonText;
        attackPowerUpgradeButtonText.text = readyUIData.attackPowerUpgradeButtonText;
        skillButtonText.text = readyUIData.skillButtonText;
        readyButtonText.text = readyUIData.readyButtonText;
    }

    public void OnClickReadyButton()
    {
        OnReadyButtonClick?.Invoke((int)WaveState.Start);
        CloseUI();
    }
}
