using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CountDownController : MonoBehaviour
{
    private TextMeshProUGUI countdownTMP;
    private int count;
    private async void OnEnable()
    {
        countdownTMP = GetComponent<TextMeshProUGUI>();
    
        count = 3;
        await StartCountDown();
    }

    async UniTask StartCountDown()
    {
        for (int i = 0; i < 3; i++)
        {
            countdownTMP.text = count.ToString();
            count--;
            await UniTask.Delay(1000);            
        }

        countdownTMP.text = "Start!";
        await UniTask.Delay(1000);
        WaveController.Instance.ChangeWaveState(2);
        gameObject.SetActive(false);
    } 
}
