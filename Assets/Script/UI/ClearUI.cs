using System;
using Cysharp.Threading.Tasks;
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
    public int effectCount;
    public int effectDelay;
}

public class ClearUI : BaseUI
{
    public TextMeshProUGUI waveClearText;
    public TextMeshProUGUI waveLevelText;
    public TextMeshProUGUI clearEarnedWisdomText;
    public TextMeshProUGUI nextWaveButtonText;

    public TextMeshProUGUI currentWisdomText;
    
    public RectTransform wisdomFromPosition;
    public RectTransform wisdomToPosition;
    public EarnedWisdom clearWisdomPrefab;
    
    private ClearUIData clearUIData;
    
    private int earnedWisdomCoefficient;
    private int defaultEarnedWisdom;
    private int effectCount;
    private int effectDelay;
    
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
        effectCount = clearUIData.effectCount;
        effectDelay = clearUIData.effectDelay;
    }

    public override async void ShowUI()
    {
        waveLevelText.text = GameController.Instance.GetWaveLevel().ToString();
        int earnedWisdom = GetEarnedWisdom();
        clearEarnedWisdomText.text = earnedWisdom.ToString();
        GameController.Instance.SetCurrentWisdom(GameController.Instance.GetCurrentWisdom() + earnedWisdom);
        SoundController.Instance.StopBGMWithFade();
        SoundController.Instance.PlaySFX(SFXType.ClearSound);
        base.ShowUI();
        await ClearEarnWidomEffect();
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
    
    public async UniTask ClearEarnWidomEffect()
    {
        for (int i = 0; i < effectCount; i++)
        {
            var wisdom = GameObject.Instantiate<EarnedWisdom>(clearWisdomPrefab, transform);
            wisdom.Explosion(wisdomFromPosition.position, wisdomToPosition.position, 150.0f);
            await UniTask.Delay(effectDelay);
        }

        ChangeWisdomText();
    }

    private void ChangeWisdomText()
    {
        currentWisdomText.text = GameController.Instance.GetCurrentWisdom().ToString();
    }
}
