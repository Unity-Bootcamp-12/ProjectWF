using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum WaveState
{
    Ready = 0,
    Start = 1,
    Progress = 2,
    End = 3
}

public class GameController : MonoBehaviour
{   
    static public GameController Instance { get; private set; }
    
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
    
    // 재화(위즈덤 관리)
    [SerializeField] private int earnedWisdomPoint = 0;
    [SerializeField] private int currentWisdomPoint;
    // 요새강화 
    private int fortressHPLevel;
    private int playerAttackPowerLevel;
    private int increasePlayerAttackPowerValue =1;
    private int increaseFortressHpValue =50;

    private int skillTemp = 4;
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
    private void Start()
    {
        maxFortressHP = 50;
        playerAttackPower = 3;
        currentWisdomPoint = PlayerPrefs.GetInt("CurrentWisdomPoint",500);
        //currentWisdomPoint = 500;
        fortressHPLevel = 1;
        playerAttackPowerLevel = 1;
        
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

    // public void IncreaseFortressHp(int increaseValue)
    // {
    //     maxFortressHP += increaseValue;
    // }

    // public void IncreasePlayerAttackPower(int increaseValue)
    // {
    //     playerAttackPower +=  increaseValue;
    // }
    
    
    private void EarnWisdom()
    {
        if (IsFortressDestoryed())
        {
            earnedWisdomPoint =250 * (waveLevel - 1) * skillTemp;
            currentWisdomPoint = earnedWisdomPoint;
            PlayerPrefs.SetInt("CurrentWisdomPoint", currentWisdomPoint);
        }
        else
        {
            earnedWisdomPoint = 1000 * waveLevel;
            currentWisdomPoint += earnedWisdomPoint;
            PlayerPrefs.SetInt("CurrentWisdomPoint", currentWisdomPoint);
        }
       
    }

    public int GetEarnedWisdomPoint()
    {
        return earnedWisdomPoint;
    }

    public int GetCurrentWisdomPoint()
    {
        return currentWisdomPoint;  
    }
    
    public void UpgradeFortressHP()
     {
         fortressHPLevel++;
         maxFortressHP +=increaseFortressHpValue;
         
     }

     public void UpgradePlayerAttackPower()
     {
         playerAttackPowerLevel++;
         playerAttackPower += increasePlayerAttackPowerValue;
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
                EarnWisdom();  
                OnProgressMonsterActive?.Invoke(MonsterWaveState.Idle);
                // UI 변경
                OnUIProgressToDefeat?.Invoke();
            }
            else
            {
                Logger.Info("웨이브 승리 로그");
                //ui active - 승리
                EarnWisdom();
                OnUIProgressToClear?.Invoke();
                
                waveLevel++;
                // UI 변경
            }
            
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
