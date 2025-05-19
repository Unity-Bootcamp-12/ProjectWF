using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartUIData : BaseUIData
{
    public string countDownText;
    public int count;
    public int countDelay;
}

public class StartUI : BaseUI
{
    public TextMeshProUGUI countDownText;
    private StartUIData startUIData;
    private int count;
    private int countDelay;
    private Action<int> OnShow;

    private void Start()
    {
        OnShow += StartCountDown;
    }

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        startUIData = uiData as StartUIData;
        
        count = startUIData.count;
        countDelay = startUIData.countDelay;
        countDownText.text = startUIData.countDownText;
    }

    public override async void ShowUI()
    {
        await CountDown(count);
    }
    
    public async void StartCountDown(int countDown)
    {
        Logger.Info("StartCountDown");
        await CountDown(countDown);
    }
    
    private async UniTask CountDown(int countDown)
    {
        for (int i = countDown; i >0; i--)
        {
            countDownText.text = i.ToString();
            await UniTask.Delay(countDelay);
        }
        
        GameController.Instance.ChangeWaveState((int)WaveState.Progress);
        CloseUI();
    }
}
