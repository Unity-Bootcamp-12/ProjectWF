using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIActiveController : MonoBehaviour
{
    [SerializeField] private GameObject firstStartUI;
    [SerializeField] private GameObject readyUI;
    [SerializeField] private GameObject progressUI;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject defeatUI;


    private void Start()
    {
        WaveController.Instance.OnUIProgressToClear += ProgressToClear;
        WaveController.Instance.OnUIProgressToDefeat += ProgressToDefeat;
    }

    public void FirstStartToReady()
    {
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
        await ProgressToClearDelay();
    }

    public void ClearToReady()
    {
        clearUI.SetActive(false);
        readyUI.SetActive(true);
    }

    public void ProgressToDefeat()
    {
        progressUI.SetActive(false);
        defeatUI.SetActive(true);
    }
    
    private async UniTask ProgressToClearDelay()
    {
        await UniTask.Delay(1000);
        progressUI.SetActive(false);
        clearUI.SetActive(true);
    }
}
