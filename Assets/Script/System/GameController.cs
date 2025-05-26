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

public class DefaultValue
{
    public int waveLevel;
    public int maxFortressHP;
    public int playerAttackPower;
    public int defaultWisdomPoint;
    public int defaultFortressHPLevel;
    public int defaultAttackPowerLevel;
    public int increasePlayerAttackPowerValue;
    public int increaseFortressHpValue;
    public int goalKillCountCoefficient;
}

public class GameController : MonoBehaviour
{   
    static public GameController Instance { get; private set; }
    
    //웨이브 이벤트 관련 
    [SerializeField]private WaveState currentWaveState = WaveState.Ready;
    
    public WaveState GetCurrentWaveState()
    {
        return currentWaveState;
    }
    private int waveLevel;
    
    public event Action<int, int> OnReadyMonsterSpawn;
    public event Action<PlayerState> OnProgressPlayerControl;
    public event Action<MonsterWaveState> OnProgressMonsterActive;
    // 스킬 리셋
    public event Action OnSkillReset;
    // 카운트 다운 이후 게임 시작
    public event Action OnInGameResetSkillCoolTime;
    // Fortress HP 변화 -> UI 변경
    public event Action<int, int> OnChangeFortressHP;
    // Progress UI 닫기
    public event Action<bool> OnEndProgress;

    public event Action<int> OnBossSpawn;
    
    // 요새 스탯관리
    [SerializeField]private int fortressHp;
    [SerializeField]private int maxFortressHP;
    
    // 플레이어 공격력 관리
    [SerializeField]private int playerAttackPower;
    public int GetPlayerAttackPower()
    {
        return playerAttackPower;
    }
    
    // 킬 카운트 관리
    private int goalKillCount;
    private int killCount;
    private int goalKillCountCoefficient;

    // 재화(위즈덤 관리)
    [SerializeField]private int currentWisdomPoint;
    
    // 요새강화 
    private int fortressHPLevel;
    private int playerAttackPowerLevel;
    private int increasePlayerAttackPowerValue;
    private int increaseFortressHpValue;

    
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
        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        // Json으로 기본값 파싱
        TextAsset defaultValueJson =  Resources.Load<TextAsset>("JsonData/DefaultValueJson");
        DefaultValue defaultValue = JsonUtility.FromJson<DefaultValue>(defaultValueJson.text);
        waveLevel = defaultValue.waveLevel;
        maxFortressHP = defaultValue.maxFortressHP;
        playerAttackPower = defaultValue.playerAttackPower;
        currentWisdomPoint = PlayerPrefs.GetInt("CurrentWisdomPoint",defaultValue.defaultWisdomPoint);
        fortressHPLevel = defaultValue.defaultFortressHPLevel;
        playerAttackPowerLevel = defaultValue.defaultAttackPowerLevel;
        increasePlayerAttackPowerValue = defaultValue.increasePlayerAttackPowerValue;
        increaseFortressHpValue = defaultValue.increaseFortressHpValue;
        goalKillCountCoefficient = defaultValue.goalKillCountCoefficient;
    }
    
    public void IncreaseKillCount()
    {
        killCount++;
        if (killCount == goalKillCount- 3 && waveLevel % 5 == 0)
        {
            OnBossSpawn?.Invoke(waveLevel);
        }
        if (killCount == goalKillCount)
        {
            ChangeWaveState((int)WaveState.End);
        }
    }

    public int GetCurrentWisdom()
    {
        return currentWisdomPoint;
    }
    
    public int GetWaveLevel()
    {
        return waveLevel;
    }
    
    public void GetDamageToFortress(int monsterPower)// 포트리스 손상 주는 메서드 
    {
        fortressHp -= monsterPower;

        OnChangeFortressHP?.Invoke(fortressHp, maxFortressHP);
        if (IsFortressDestoryed())
        {
            ChangeWaveState((int)WaveState.End);
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

    public void SetCurrentWisdom(int value)
    {
        currentWisdomPoint = value;
        PlayerPrefs.SetInt("CurrentWisdomPoint",  currentWisdomPoint);
    }

    public int GetHPUpgradeLevel()
    {
        return fortressHPLevel;
    }

    public int GetAttackPowerUpgradeLevel()
    {
        return playerAttackPowerLevel;
    }
    public void UpgradeFortressHP(int expense)
    {
        currentWisdomPoint -= expense;
        fortressHPLevel++;
        maxFortressHP += increaseFortressHpValue;
    }

     public void UpgradePlayerAttackPower(int expense)
     {
         currentWisdomPoint -= expense;
         playerAttackPowerLevel++;
         playerAttackPower += increasePlayerAttackPowerValue;
     }

    public void ChangeWaveState(int state)
    {
        currentWaveState = (WaveState)state;
        if (currentWaveState == WaveState.Ready)
        {
            // 킬 카운트 초기화
            killCount = 0;
            // 웨이브 레벨에 따라 공식 적용 필요
            goalKillCount = waveLevel * goalKillCountCoefficient;
            if (waveLevel % 5 == 0)
            {
                goalKillCount++;
            }
            // 테스트
            OnReadyMonsterSpawn?.Invoke(goalKillCount, waveLevel);
             // UI 변경
             UIManager.Instance.OpenUI<ReadyUI>(uiDataDictionary[UIType.ReadyUI]);
        }
        else if (currentWaveState == WaveState.Start)
        {
            // Hp값 보관하고 시작 
            fortressHp = maxFortressHP;
            OnChangeFortressHP?.Invoke(fortressHp, maxFortressHP);
            // UI 변경
            UIManager.Instance.OpenUI<StartUI>(uiDataDictionary[UIType.StartUI]);
        }
        else if (currentWaveState == WaveState.Progress)
        {
            
            OnProgressPlayerControl?.Invoke(PlayerState.Attack);
            OnProgressMonsterActive?.Invoke(MonsterWaveState.Active);
            // 스킬 쿨타임 초기화
            OnSkillReset?.Invoke();
            // UI 변경
            UIManager.Instance.OpenUI<ProgressUI>(uiDataDictionary[UIType.ProgressUI]);
            OnInGameResetSkillCoolTime?.Invoke();
            OnInGameResetSkillCoolTime = null;
        }
        else if (currentWaveState == WaveState.End)
        {
            // progress ui close
            OnEndProgress?.Invoke(false);
            // 플레이어 공격 멈춤
            OnProgressPlayerControl?.Invoke(PlayerState.Idle);
            
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            
            if (IsFortressDestoryed())
            {
                OnProgressMonsterActive?.Invoke(MonsterWaveState.Idle);
                // UI 변경
                UIManager.Instance.OpenUI<DefeatUI>(uiDataDictionary[UIType.DefeatUI]);
            }
            else
            {
                //ui active - 승리
                // UI 변경
                UIManager.Instance.OpenUI<ClearUI>(uiDataDictionary[UIType.ClearUI]);
                
                // 최고 웨이브 갱신
                int maxWave = PlayerPrefs.GetInt("MaxWave", 0);
                if (waveLevel > maxWave)
                {
                    PlayerPrefs.SetInt("MaxWave", waveLevel);
                }
                // 웨이브 레벨 증가
                waveLevel++;
            }
        }
    }


    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
#endif
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
}