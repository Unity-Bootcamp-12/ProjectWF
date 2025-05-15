using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
    
    //웨이브 이벤트 관련 
    [SerializeField]private WaveState currentWaveState = WaveState.Ready;
    [SerializeField]private int waveLevel = 1;
    public event Action<int> OnReadyMonsterSpawn;
    public event Action<PlayerState> OnProgressPlayerControl;
    public event Action<MonsterWaveState> OnProgressMonsterActive;
    public event Action OnUIProgressToClear;
    public event Action OnUIProgressToDefeat;
    
    // 요새 스탯관리
    [SerializeField]private int fortressHp =0;
    [SerializeField]private int maxFortressHP = 50;
    [SerializeField] private Slider hpSlider;
    
    // 플레이어 공격력 관리
    [SerializeField] private int playerAttackPower;
    // 킬 카운트 관리
    [SerializeField]private int goalKillCount = 0;
    [SerializeField]private int killCount = 0;

    [SerializeField] private GameObject countdownObject;
    private void Start()
    {
        maxFortressHP = 50;
        playerAttackPower = 3;
        
    }

    public void IncreaseKillCount()
    {
        killCount++;
        if (killCount == goalKillCount)
        {
            ChangeWaveState(3);
        }
    }
    
    public int GetWaveLevel()
    {
        return waveLevel;
    }
    
    public void GetDamageToFortress(int monsterPower)// 포트리스 손상 주는 메서드 
    {
       
        fortressHp -= monsterPower;
        hpSlider.value = (float)fortressHp/maxFortressHP;
        Logger.Info($"요새HP : {fortressHp}");
        if (IsFortressDestoryed())
        {
            ChangeWaveState(3);
            //웨이브 종료 
        }
    }
    
    

    public bool IsFortressDestoryed()// 요새 승패 조건
    {
        if (fortressHp > 0)
        {
            return false;
        }

        else
        {
            return true;
        }
        
    }

    public void IncreaseFortressHp(int increaseValue)
    {
        maxFortressHP += increaseValue;
    }

    public void IncreasePlayerAttackPower(int increaseValue)
    {
        playerAttackPower +=  increaseValue;
    }

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
            // Hp값 보관하고 시작 
            fortressHp = maxFortressHP;
            
            // 킬 카운트 초기화
            killCount = 0;
            // 웨이브 레벨에 따라 공식 적용 필요
            goalKillCount = waveLevel * 2;
             OnReadyMonsterSpawn?.Invoke(goalKillCount);
             // UI 변경
             
             
        }
        else if (currentWaveState == WaveState.Start)
        {
            // 카운트 다운 이벤트 함수 실행
            countdownObject.SetActive(true);
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
            
            if (IsFortressDestoryed())
            {
                Logger.Info("웨이브 패배 로그");
                OnProgressMonsterActive?.Invoke(MonsterWaveState.Idle);
                // UI 변경
                OnUIProgressToDefeat?.Invoke();
            }
            else
            {
                Logger.Info("웨이브 승리 로그");
                //ui active - 승리
                OnUIProgressToClear?.Invoke();
                waveLevel++;
                // UI 변경
            }
            
        }
    }
}
