using System;
using TMPro;
using UnityEngine;

public class ReadyUIData : BaseUIData
{
    public string hpUpgradeButtonText;
    public string attackPowerUpgradeButtonText;
    public string skillButtonText;
    public string readyButtonText;
    public int hpUpgradeCoeffecient;
    public int attackPowerUpgradeCoeffecient;
}

public class ReadyUI : BaseUI
{
    public TextMeshProUGUI hpUpgradeButtonText;
    public TextMeshProUGUI hpUpgradeButtonExpenseText;
    public TextMeshProUGUI attackPowerUpgradeButtonText;
    public TextMeshProUGUI attackPowerUpgradeButtonExpenseText;
    public TextMeshProUGUI skillButtonText;
    public TextMeshProUGUI readyButtonText;

    public TextMeshProUGUI currentWisdomText;
    
    private int hpUpgradeCoeffecient;
    private int attackPowerUpgradeCoeffecient;

    private int hpUpgradeExpense;
    private int attackPowerUpgradeExpense;
    
    private ReadyUIData readyUIData;
    
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        
        readyUIData = uiData as ReadyUIData;
        
        hpUpgradeCoeffecient = readyUIData.hpUpgradeCoeffecient;
        attackPowerUpgradeCoeffecient = readyUIData.attackPowerUpgradeCoeffecient;

        hpUpgradeExpense = GetHpUpgradeExpense();
        attackPowerUpgradeExpense = GetAttackPowerUpgradeExpense();
        
        hpUpgradeButtonText.text = readyUIData.hpUpgradeButtonText;
        hpUpgradeButtonExpenseText.text = hpUpgradeExpense.ToString();
        
        attackPowerUpgradeButtonText.text = readyUIData.attackPowerUpgradeButtonText;
        attackPowerUpgradeButtonExpenseText.text = attackPowerUpgradeExpense.ToString();
        
        skillButtonText.text = readyUIData.skillButtonText;
        readyButtonText.text = readyUIData.readyButtonText;
        
    }

    public override void ShowUI()
    {
        base.ShowUI();
        SoundController.Instance.PlayBGM(BGMType.MainBGM);
        ChangeWisdomText();
    }

    public void OnClickReadyButton()
    {
        SoundController.Instance.PlaySFX(SFXType.UIClickSound);
        GameController.Instance.ChangeWaveState((int)WaveState.Start);
        CloseUI();
    }

    public void OnClickSkillButton()
    {
        SoundController.Instance.PlaySFX(SFXType.UIClickSound);
        UIManager.Instance.OpenUI<SkillSettingUI>(new SkillSettingUIData());
    }

    private int GetHpUpgradeExpense()
    {
        return GameController.Instance.GetHPUpgradeLevel() *  hpUpgradeCoeffecient;
    }
    
    public void OnClickHPUpgradeButton()
    {
        if (hpUpgradeExpense > GameController.Instance.GetCurrentWisdom())
        {
            SoundController.Instance.PlaySFX(SFXType.UpgradeNegativeSound);
            return;
        }
        
        GameController.Instance.UpgradeFortressHP(hpUpgradeExpense);
        SoundController.Instance.PlaySFX(SFXType.UpgradeSound);
        hpUpgradeExpense = GetHpUpgradeExpense();
        hpUpgradeButtonExpenseText.text = hpUpgradeExpense.ToString();
        ChangeWisdomText();
    }

    private int GetAttackPowerUpgradeExpense()
    {
        return GameController.Instance.GetAttackPowerUpgradeLevel() * attackPowerUpgradeCoeffecient;
    }
    
    public void OnClickAttackPowerUpgradeButton()
    {
        if (attackPowerUpgradeExpense > GameController.Instance.GetCurrentWisdom())
        {
            SoundController.Instance.PlaySFX(SFXType.UpgradeNegativeSound);
            return;
        }
        
        GameController.Instance.UpgradePlayerAttackPower(attackPowerUpgradeExpense);
        SoundController.Instance.PlaySFX(SFXType.UpgradeSound);
        attackPowerUpgradeExpense = GetAttackPowerUpgradeExpense();
        attackPowerUpgradeButtonExpenseText.text = attackPowerUpgradeExpense.ToString();
        ChangeWisdomText();
    }

    private void ChangeWisdomText()
    {
        currentWisdomText.text =  GameController.Instance.GetCurrentWisdom().ToString();
    }
}
