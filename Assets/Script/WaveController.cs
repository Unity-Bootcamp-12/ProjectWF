using System;
using UnityEngine;

public enum WaveState
{
    Ready = 0,
    Start = 1,
    Progress = 2,
    End = 3
}

public class WaveController : MonoBehaviour
{   
    static public WaveController Instance { get; private set; }
    
    private WaveState currentWaveState = WaveState.Ready;
    
    private int waveLevel = 1;
    
    public event Action<int> OnReadyMonsterSpawn;

    public event Action<PlayerState> OnProgressPlayerControl;
    public event Action<MonsterWaveState> OnProgressMonsterActive;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeWaveState(int state)
    {
        currentWaveState = (WaveState)state;
        if (currentWaveState == WaveState.Ready)
        {
            // 웨이브 레벨에 따라 공식 적용 필요
             OnReadyMonsterSpawn?.Invoke(5);
             // UI 변경
        }
        else if (currentWaveState == WaveState.Start)
        {
            // 카운트 다운 이벤트 함수 실행
            // UI 변경
        }
        else if (currentWaveState == WaveState.Progress)
        {
            OnProgressPlayerControl?.Invoke(PlayerState.Attack);
            OnProgressMonsterActive?.Invoke(MonsterWaveState.Active);
            // UI 변경
        }
        else if (currentWaveState == WaveState.End)
        {
            OnProgressPlayerControl?.Invoke(PlayerState.Idle);
            // UI 변경
        }
    }
}
