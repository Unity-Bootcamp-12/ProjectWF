using System;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    private int fortressHPLevel;
    private int playerAttackPowerLevel;

    private void Start()
    {
        fortressHPLevel = 1;
        playerAttackPowerLevel = 1;
    }

    public void UpgradeFortressHP()
    {
        fortressHPLevel++;
        WaveController.Instance.IncreaseFortressHp(50);
        
    }

    public void UpgradePlayerAttackPower()
    {
        playerAttackPowerLevel++;
        WaveController.Instance.IncreasePlayerAttackPower(1);
    }
}
