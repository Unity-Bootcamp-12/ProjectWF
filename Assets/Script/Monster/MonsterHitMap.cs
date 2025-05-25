using System;
using UnityEngine;

public class MonsterHitMap : MonoBehaviour
{
    private int parentMonsterPower;
    public void SetparentMonsterPower(int monsterPower)
    {
        parentMonsterPower = monsterPower;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Fortress"))
        {
            SoundController.Instance.PlaySFX(SFXType.PlayerDamagedSound);
            GameController.Instance.GetDamageToFortress(parentMonsterPower);
            Logger.Info($"데미지파워 : {parentMonsterPower}");
            
        }
    }
}
