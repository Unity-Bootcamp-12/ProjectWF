using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatUIData : BaseUIData
{
    public string waveDefeatText;
    public string maxWaveText;
    public string currentWaveText;
    public int defeatEarnedWisdomCoefficient;
    public int effectCount;
    public int effectDelay;
}

public class DefeatUI : BaseUI
{
    public TextMeshProUGUI waveDefeatText;
    public TextMeshProUGUI maxWaveText;
    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI currentWaveLevelText;
    public TextMeshProUGUI maxWaveLevelText;
    public TextMeshProUGUI defeatEarnedWisdomText;

    public TextMeshProUGUI currentWisdomText;
    
    public LostWisdom wisdomPrefab;
    public RectTransform wisdomTargetPosition;
    
    private DefeatUIData defeatUIData;

    private int skillCount;
    private int defeatEarnedWisdomCoefficient;
    
    private int effectCount;
    private int effectDelay;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);
        
        defeatUIData = uiData as DefeatUIData;
        
        defeatEarnedWisdomCoefficient =  defeatUIData.defeatEarnedWisdomCoefficient;
        waveDefeatText.text = defeatUIData.waveDefeatText;
        maxWaveText.text = defeatUIData.maxWaveText;
        currentWaveText.text = defeatUIData.currentWaveText;
        
        effectCount =  defeatUIData.effectCount;
        effectDelay =  defeatUIData.effectDelay;
    }

    public override async void ShowUI()
    {
        currentWaveLevelText.text = GameController.Instance.GetWaveLevel().ToString();
        maxWaveLevelText.text = PlayerPrefs.GetInt("MaxWave", 0).ToString();

        int defeatEarnedWisdom = GetDefeatEarnedWisdom();
        defeatEarnedWisdomText.text = defeatEarnedWisdom.ToString();
        GameController.Instance.SetCurrentWisdom(defeatEarnedWisdom);
        SoundController.Instance.StopBGMWithFade();
        SoundController.Instance.PlaySFX(SFXType.DefeatSound);
        base.ShowUI();
        await ShowDefeatEarnedWisdomEffect();
    }

    private int GetDefeatEarnedWisdom()
    {
        // skillCount 차후에 곱해줘야함
        return GameController.Instance.GetWaveLevel()*defeatEarnedWisdomCoefficient;
    }
    
    public void OnClickRestartButton()
    {
        SoundController.Instance.PlaySFX(SFXType.UIClickSound);
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        CloseUI();
    }

    public void OnClickQuitButton()
    {
        SoundController.Instance.PlaySFX(SFXType.UIClickSound);
        Application.Quit();
        CloseUI();
    }
    
    public async UniTask ShowDefeatEarnedWisdomEffect()
    {
        SoundController.Instance.PlaySFX(SFXType.LostWisdomSound);
        for (int i = 0; i < effectCount; i++)
        {
            var wisdom = GameObject.Instantiate<LostWisdom>(wisdomPrefab, transform);
            wisdom.Explosion(wisdomTargetPosition.position);
            await UniTask.Delay(effectDelay);
        }
        
        ChangeWisdomText();
    }
    
    private void ChangeWisdomText()
    {
        currentWisdomText.text = GameController.Instance.GetCurrentWisdom().ToString();
    }
}
