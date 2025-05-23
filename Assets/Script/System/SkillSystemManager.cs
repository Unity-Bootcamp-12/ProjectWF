using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MonsterSpwaner;
using static SkillSystemManager;

public enum EnumSkillAttribute
{
    Fire = 0,
    Lightning = 1,
    Water = 2,
}

[System.Serializable]
public class SkillData : BaseUIData
{
    public string skillName;
    public int skillCoolTime;
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
}


public class SkillSystemManager : MonoBehaviour
{
    static public SkillSystemManager Instance { get; private set; }

    private Dictionary<string, bool> skillEquipMap = new Dictionary<string, bool>();
    [SerializeField] private GameObject[] ownedSkillButtonSet;

    private EnumSkillAttribute currentSkillAttribute;
    private int currentSkillGradeNumber;
    public event Action<int,int> onSkillUnlockStateChanged;


    [System.Serializable]
    public class SkillDataList
    {
        public List<SkillData> skillDataList;
    }

    private SkillDataList skillJsonDataList;
    private SkillData[,] skillDataSet;
    private Sprite[,] skillSpriteSet;
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
            if (skillInputData.unlockState == 1)
            {
                isSkillUnlocked[skillInputData.skillAttribute, skillInputData.skillGrade] = true;
            }

            else if (skillInputData.unlockState == 0)
            {
                isSkillUnlocked[skillInputData.skillAttribute, skillInputData.skillGrade] = false;
            }

            else
            {
                Logger.Error($"skillData.unlockState={skillInputData.unlockState}");
            }
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

    public bool isSkillUsingUnloked(int skillAttributeNumber, int skillGradeNumber)
    {
        
        return isSkillUnlocked[skillAttributeNumber, skillGradeNumber];
    }

    public void UnlockSkill(int skillAttributeNumber, int skillGradeNumber)
    {
        isSkillUnlocked[skillAttributeNumber, skillGradeNumber] = true;
        onSkillUnlockStateChanged?.Invoke(skillAttributeNumber, skillGradeNumber);
    }


    // 다이얼로그 
    public void ShowDialogue(EnumSkillAttribute skillAttribute, int skillGradeNumber)
    {
        currentSkillAttribute = skillAttribute;
        currentSkillGradeNumber = skillGradeNumber;
        // UI 매니저에서 다이얼로그 받아옴
        UIManager.Instance.OpenUI<SkillDialogueUI>(skillDataSet[(int)currentSkillAttribute, currentSkillGradeNumber]);
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

                equipSkillData[i] = skillDataSet[(int)currentSkillAttribute, currentSkillGradeNumber];
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