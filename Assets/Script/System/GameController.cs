using System;
using System.Collections.Generic;
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
    private int waveLevel;
    
    public event Action<int> OnReadyMonsterSpawn;
    public event Action<PlayerState> OnProgressPlayerControl;
    public event Action<MonsterWaveState> OnProgressMonsterActive;
    // 스킬 리셋
    public event Action OnSkillReset;
    // 카운트 다운 이후 게임 시작
    public event Action OnInGameStart;
    // Fortress HP 변화 -> UI 변경
    public event Action<int, int> OnChangeFortressHP;
    // Progress UI 닫기
    public event Action<bool> OnEndProgress;
    
    // 요새 스탯관리
    private int fortressHp =0;
    private int maxFortressHP = 50;
    
    // 플레이어 공격력 관리
    private int playerAttackPower;
    
    // 킬 카운트 관리
    private int goalKillCount;
    private int killCount;

    // 재화(위즈덤 관리)
    private int earnedWisdomPoint;
    private int currentWisdomPoint;
    
    // 요새강화 
    private int fortressHPLevel;
    private int playerAttackPowerLevel;
    private int increasePlayerAttackPowerValue =1;
    private int increaseFortressHpValue =50;

    private int skillTemp = 4;
    
    //UI Data
    public Dictionary<UIType, BaseUIData> uiDataDictionary = new Dictionary<UIType, BaseUIData>();
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
        waveLevel = 1;
        maxFortressHP = 50;
        playerAttackPower = 3;
        currentWisdomPoint = PlayerPrefs.GetInt("CurrentWisdomPoint",500);
        //currentWisdomPoint = 500;
        fortressHPLevel = 1;
        playerAttackPowerLevel = 1;

        UIManager.Instance.OpenUI<FirstStartUI>(uiDataDictionary[UIType.FirstStartUI]);
    }

    public void IncreaseKillCount()
    {
        killCount++;
        if (killCount == goalKillCount)
        {
            ChangeWaveState((int)WaveState.End);
        }
    }
    
    public int GetWaveLevel()
    {
        return waveLevel;
    }
    
    public void GetDamageToFortress(int monsterPower)// 포트리스 손상 주는 메서드 
    {
        fortressHp -= monsterPower;

        OnChangeFortressHP?.Invoke(fortressHp, maxFortressHP);
        Logger.Info($"요새HP : {fortressHp}");
        if (IsFortressDestoryed())
        {
            ChangeWaveState((int)WaveState.End);
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
            OnChangeFortressHP?.Invoke(fortressHp, maxFortressHP);
            
            // 킬 카운트 초기화
            killCount = 0;
            // 웨이브 레벨에 따라 공식 적용 필요
            goalKillCount = waveLevel * 2;
            OnReadyMonsterSpawn?.Invoke(goalKillCount);
             // UI 변경
             UIManager.Instance.OpenUI<ReadyUI>(uiDataDictionary[UIType.ReadyUI]);
        }
        else if (currentWaveState == WaveState.Start)
        {
            // 카운트 다운 이벤트 함수 실행
            // countdownObject.SetActive(true);
            // 카운트 다운 이후 스킬 사용가능하도록
            OnInGameStart?.Invoke();
            // UI 변경
            UIManager.Instance.OpenUI<StartUI>(uiDataDictionary[UIType.StartUI]);
        }
        else if (currentWaveState == WaveState.Progress)
        {
            
            OnProgressPlayerControl?.Invoke(PlayerState.Attack);
            OnProgressMonsterActive?.Invoke(MonsterWaveState.Active);
            // UI 변경
            UIManager.Instance.OpenUI<ProgressUI>(uiDataDictionary[UIType.ProgressUI]);
        }
        else if (currentWaveState == WaveState.End)
        {
            // progress ui close
            OnEndProgress?.Invoke(false);
            // 플레이어 공격 멈춤
            OnProgressPlayerControl?.Invoke(PlayerState.Idle);
            // 스킬 쿨타임 초기화
            OnSkillReset?.Invoke();
            
            if (IsFortressDestoryed())
            {
                Logger.Info("웨이브 패배 로그");
                EarnWisdom();  
                OnProgressMonsterActive?.Invoke(MonsterWaveState.Idle);
                // UI 변경
                // OnUIProgressToDefeat?.Invoke();
                UIManager.Instance.OpenUI<DefeatUI>(uiDataDictionary[UIType.DefeatUI]);
            }
            else
            {
                Logger.Info("웨이브 승리 로그");
                //ui active - 승리
                EarnWisdom();
                // UI 변경
                // OnUIProgressToClear?.Invoke();
                UIManager.Instance.OpenUI<ClearUI>(uiDataDictionary[UIType.ClearUI]);
                
                // 웨이브 레벨 증가
                waveLevel++;
            }
            
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}