using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static MonsterSpwaner;
using static SkillSystemManager;

public enum EnumSkillAttribute
{
    Fire = 0,
    Lightning = 1,
    Water = 2,
    None= 3,
}

public enum EnumSkillTargetType
{
    Buff = 0,
    InFrontOfPlayer = 1,
    ByMousePoint = 2
}

public enum EnumSkillType
{
    Buff = 0,
    Normal = 1,
    DeBuff = 2
}


[System.Serializable]
public class SkillData : BaseUIData
{
    public string skillName;
    public float skillCoolTime;
    public string skillExplainText;
    public int skillLevel;
    public int skillAttribute;
    public int skillGrade;
    public int skillDamagePower;
    public int skillType;
    public int skillRangeType;
    public int skillRangeVertical;
    public int skillRangeHorizontal;
    public int skillRangeRadius;
    public int skillSideEffect;
    public int continuousSkillState;
    public int unlockState;
    public int equippedIndexPosition;
    public string skillPrefabPath;
    public int skillTargetType;
}


public class SkillSystemManager : MonoBehaviour
{
    static public SkillSystemManager Instance { get; private set; }

    private Dictionary<string, bool> skillEquipMap = new Dictionary<string, bool>();
    [SerializeField] private GameObject[] ownedSkillButtonSet;

    public EnumSkillAttribute CurrentSkillAttribute { get; private set; }
    private int currentSkillGradeNumber;
    public event Action<int, int> onSkillUnlockStateChanged;
    [System.Serializable]
    public class SkillDataList
    {
        public List<SkillData> skillDataList;
    }

    private SkillDataList skillJsonDataList;
    private SkillData[,] skillDataSet;
    private Sprite[,] skillSpriteSet;
    private AudioResource[,] skillAudioSet;
    private bool[,] isSkillUnlocked;
    private int skillAttributeCount;
    private int skillGradeCount;
    private int initialAttibuteNumber = 0;
    

    //장착 스킬
    [SerializeField] private int equipSkillCount;
    public SkillData[] equipSkillData;

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

        InitSkillManager();
    }


    void InitSkillManager()
    {
        //하드코딩 제거 필요
        skillAttributeCount = 3;
        skillGradeCount = 3;
        skillEquipMap = new Dictionary<string, bool>();
        skillDataSet = new SkillData[skillAttributeCount, skillGradeCount];
        skillSpriteSet = new Sprite[skillAttributeCount, skillGradeCount];
        isSkillUnlocked = new bool[skillAttributeCount, skillGradeCount];
        equipSkillData = new SkillData[ownedSkillButtonSet.Length];

        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/SkillDataJson");
        if (jsonFile != null)
        {
            skillJsonDataList = JsonUtility.FromJson<SkillDataList>(jsonFile.text);
        }
        else
        {
            Logger.Error("skillData.json not found in Resources folder");
        }

        InitSkillJsonData();
    }

    // 스킬 버튼 초기설정
    void InitSkillJsonData()
    {
        for (int i = 0; i < skillJsonDataList.skillDataList.Count; i++)
        {
            SkillData skillInputData = skillJsonDataList.skillDataList[i];
            skillDataSet[skillInputData.skillAttribute, skillInputData.skillGrade] = skillInputData;
            skillSpriteSet[skillInputData.skillAttribute, skillInputData.skillGrade] =
                Resources.Load<Sprite>($"IconData/{skillInputData.skillName}");
            InitUnLockSkillSetting(skillInputData);
            InitEquipSkillSetting(skillInputData);
        }
    }

    public SkillData GetSkillData(int skillAttributeNumber, int skillGradeNumber)
    {
        return skillDataSet[skillAttributeNumber, skillGradeNumber];
    }

    public Sprite GetSkillSprite(int skillAttributeNumber, int skillGradeNumber)
    {
        return skillSpriteSet[skillAttributeNumber, skillGradeNumber];
    }

    public void InitUnLockSkillSetting(SkillData skillData)
    {
        if (skillData.unlockState == 1)
        {
            isSkillUnlocked[skillData.skillAttribute, skillData.skillGrade] = true;
        }

        else if (skillData.unlockState == 0)
        {
            isSkillUnlocked[skillData.skillAttribute, skillData.skillGrade] = false;
        }

        else
        {
            Logger.Error($"skillData.unlockState={skillData.unlockState}");
        }
    }

    public void InitEquipSkillSetting(SkillData skillData)
    {
        if (skillData.equippedIndexPosition != -1)
        {
            equipSkillData[skillData.equippedIndexPosition] = skillData;
            skillEquipMap.Add(skillData.skillName, true);
        }
    }

    public bool isSkillUsingUnloked(int skillAttributeNumber, int skillGradeNumber)
    {
        return isSkillUnlocked[skillAttributeNumber, skillGradeNumber];
    }
    
    public void UnlockSkill(int skillAttributeNumber, int skillGradeNumber)
    {
        int UnlockCost = GameController.Instance.GetcurrentSkillUnlockgradeWisdom(skillGradeNumber);
        int wisdomSubtractValue = GameController.Instance.GetCurrentWisdom() - UnlockCost;
                                  
        if (wisdomSubtractValue < 0)
        {
            SoundController.Instance.PlaySFX(SFXType.UpgradeNegativeSound);
            Logger.Info("재화가 부족합니다.");
            return;
        }
        
        SoundController.Instance.PlaySFX(SFXType.UpgradeSound);
        GameController.Instance.SetCurrentWisdom(wisdomSubtractValue);
        GameController.Instance.AccumlateConsumedWisdom(UnlockCost);
        
        isSkillUnlocked[skillAttributeNumber, skillGradeNumber] = true;
        skillDataSet[skillAttributeNumber, skillGradeNumber].unlockState = 1;
        onSkillUnlockStateChanged?.Invoke(skillAttributeNumber, skillGradeNumber);
    }

    public EnumSkillTargetType GetSkillTargetType(SkillData data)
    {
        return (EnumSkillTargetType)data.skillTargetType;
    }

    // 스킬 업그레이드

    public void UpgradeSkill(int skillAttributeNumber, int skillGradeNumber)
    {
        if (!isSkillUnlocked[skillAttributeNumber, skillGradeNumber])
        {
            return;
        }
        int skillLevel = skillDataSet[skillAttributeNumber, skillGradeNumber].skillLevel;
        int wisdomUpgradeCost = GameController.Instance.GetSkillUpgradeWisdom(skillLevel,skillGradeNumber);
        int wisdomSubtractValue = GameController.Instance.GetCurrentWisdom() - wisdomUpgradeCost;
        
        if (wisdomSubtractValue < 0)
        {
            SoundController.Instance.PlaySFX(SFXType.UpgradeNegativeSound);
            Logger.Info("재화가 부족합니다.");
            return;
        }

        SoundController.Instance.PlaySFX(SFXType.UpgradeSkillSound);
        GameController.Instance.SetCurrentWisdom(wisdomSubtractValue);
        GameController.Instance.AccumlateConsumedWisdom(wisdomUpgradeCost);
        skillDataSet[skillAttributeNumber, skillGradeNumber].skillLevel += 1;
        skillDataSet[skillAttributeNumber, skillGradeNumber].skillDamagePower += 1;
        skillDataSet[skillAttributeNumber, skillGradeNumber].skillCoolTime-=0.1f*(skillLevel+1);
        if (skillDataSet[skillAttributeNumber, skillGradeNumber].skillCoolTime <= 0)
        {
            skillDataSet[skillAttributeNumber, skillGradeNumber].skillCoolTime=0.01f;
        }
        
        
        

    }

    // 다이얼로그 
    public void ShowDialogue(EnumSkillAttribute skillAttribute, int skillGradeNumber)
    {
        CurrentSkillAttribute = skillAttribute;
        currentSkillGradeNumber = skillGradeNumber;
        // UI 매니저에서 다이얼로그 받아옴
        UIManager.Instance.OpenUI<SkillDialogueUI>(skillDataSet[(int)CurrentSkillAttribute, currentSkillGradeNumber]);
    }


    // 딕셔너리 관리 
    public void EquipSkill(string skillName)
    {
        for (int i = 0; i < equipSkillCount; i++)
        {
            if (equipSkillData[i] == null)
            {
                if (skillEquipMap.ContainsKey(skillName))
                {
                    skillEquipMap[skillName] = true;
                }
                else
                {
                    skillEquipMap.Add(skillName, true);
                }

                equipSkillData[i] = skillDataSet[(int)CurrentSkillAttribute, currentSkillGradeNumber];
                skillDataSet[(int)CurrentSkillAttribute, currentSkillGradeNumber].equippedIndexPosition = i;
                return;
            }
        }
    }

    public void ReleaseSkill(string skillName)
    {
        skillEquipMap[skillName] = false;
        for (int i = 0; i < equipSkillCount; i++)
        {
            if (equipSkillData[i] != null && equipSkillData[i].skillName == skillName)
            {
                equipSkillData[i] = null;
                skillDataSet[(int)CurrentSkillAttribute, currentSkillGradeNumber].equippedIndexPosition = -1;
                
                
                return;
            }
        }
    }

    public bool IsSkillEquipped(string skillName)
    {
        if (skillEquipMap.ContainsKey(skillName))
        {
            if (skillEquipMap[skillName])
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
    }
    
}