using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIActiveController : MonoBehaviour
{
    [SerializeField] private GameObject firstStartUI;
    [SerializeField] private GameObject readyUI;
    [SerializeField] private GameObject progressUI;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject defeatUI;
    [SerializeField] private TextMeshProUGUI currentWisdomPointText;
    [SerializeField] private TextMeshProUGUI clearEarnedWisdomPointText;
    [SerializeField] private TextMeshProUGUI defeatEarnedWisdomPointText;
    
    [SerializeField] private EarnedWisdom clearWisdomPrefab;
    [SerializeField] private Transform wisdomTargetPosition;
    [SerializeField] private Transform clearWisdomStartPosition;
    [SerializeField] private Transform clearParentCanvas;
    
    [SerializeField] private LostWisdom defeatWisdomPrefab;
    [SerializeField] private Transform defeatParentCanvas;
    [SerializeField] private Transform defeatWisdomTargetPosition;

    
    private void Start()
    {
        GameController.Instance.OnUIProgressToClear += ProgressToClear;
        GameController.Instance.OnUIProgressToDefeat += ProgressToDefeat;
    }

    public async UniTask ClearEarnWidomEffect(int effectCount)
    {
        for (int i = 0; i < effectCount; i++)
        {
            var wisdom = GameObject.Instantiate<EarnedWisdom>(clearWisdomPrefab, clearParentCanvas);
            wisdom.Explosion(clearWisdomStartPosition.position, wisdomTargetPosition.position, 150.0f);
            await UniTask.Delay(100);
        }
        
    }

    public async UniTask DefeatEarnedWisdomEffect(int effectCount)
    {
        for (int i = 0; i < effectCount; i++)
        {
            var wisdom = GameObject.Instantiate<LostWisdom>(defeatWisdomPrefab, defeatParentCanvas);
            wisdom.Explosion(defeatWisdomTargetPosition.position);
            await UniTask.Delay(100);
        }
        
    }




    public void FirstStartToReady()
    {
        currentWisdomPointText.text = $"{GameController.Instance.GetCurrentWisdomPoint()}";
        firstStartUI.SetActive(false);
        readyUI.SetActive(true);
    }

    public void ReadyToStart()
    {
        readyUI.SetActive(false);
        progressUI.SetActive(true);
    }

    public async void ProgressToClear()
    {
        clearEarnedWisdomPointText.text = $"획득한 위즈덤 :{GameController.Instance.GetEarnedWisdomPoint()} ";
        await ProgressToClearDelay();
    }

    public void ClearToReady()
    {
        clearUI.SetActive(false);
        readyUI.SetActive(true);
    }

    public async void ProgressToDefeat()
    {
        defeatEarnedWisdomPointText.text = $"획득화 위즈덤 :{GameController.Instance.GetEarnedWisdomPoint()} ";
        progressUI.SetActive(false);
        defeatUI.SetActive(true);

        await ProgressToDefeatDelay();
    }

    private async UniTask ProgressToDefeatDelay()
    {
        await UniTask.Delay(1000);
        DefeatEarnedWisdomEffect(5);
    }
    
    private async UniTask ProgressToClearDelay()
    {
        currentWisdomPointText.text = $"{GameController.Instance.GetCurrentWisdomPoint()}";
        await UniTask.Delay(1000);
        progressUI.SetActive(false);
        clearUI.SetActive(true);
        
        await UniTask.Delay(1000);
        ClearEarnWidomEffect(5);
    }
    
    
    
    public void QuitApplication()
    {
        Application.Quit();
    }

}
