using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MonsterSpwaner;
using static SkillSystemManager;

public class SkillSystemManager : MonoBehaviour
{
    [SerializeField] private GameObject skillTreeButtonGroup;
    [SerializeField] private GameObject skillDialogue;
    static public SkillSystemManager Instance { get; private set; }
    [SerializeField] Dictionary<string, bool> skillEquipMap = new Dictionary<string, bool>();
    [SerializeField] private GameObject[] ownedSkillButtonSet;
    public event Action<int, int> OnTransferInfoButtonToDialgue;
    public event Action<int, int> OnTransferInfoDialgueToOwnedButtonSkill;
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public int skillCoolTime;
        public string skillExplainText;
        public int skillLevel;
        public int skillAttribute;
        public int skillGrade;
        public int skillDamagePower;

    }


    [System.Serializable]
    public class SkillDataList
    {
        public List<SkillData> skillDataList;
    }
    private SkillDataList skillJsonDataList;
    private SkillData[,] skillDataSet;
    private Sprite[,] skillSpriteSet;
    private int skillAttributeCount;
    private int skillGradeCount;
    private int initialAttibuteNumber = 0;

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

  
    void Start()
    {
        
       
        
       

    }

    void InitSkillManager()
    {
        skillAttributeCount = skillTreeButtonGroup.transform.childCount;
        skillGradeCount = skillTreeButtonGroup.transform.GetChild(0).childCount-1;
        Logger.Info($"스킬등급수 확인: {skillGradeCount}");
        skillEquipMap = new Dictionary<string, bool>();
        skillDataSet = new SkillData[skillAttributeCount,skillGradeCount];
        skillSpriteSet = new Sprite[skillAttributeCount,skillGradeCount];
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
            SkillData skillInputData  = skillJsonDataList.skillDataList[i];
            Logger.Info($"Skill 횟수 디버깅: {skillInputData.skillAttribute},{skillInputData.skillGrade}");
            skillDataSet[skillInputData.skillAttribute, skillInputData.skillGrade] = skillInputData;
            skillSpriteSet[skillInputData.skillAttribute, skillInputData.skillGrade] = Resources.Load<Sprite>($"IconData/{skillInputData.skillName}");
        }
        
    }

    public SkillData GetSkillData(int  skillAttributeNumber, int skillGradeNumber)
    {
        return skillDataSet[skillAttributeNumber,skillGradeNumber];
        
    }

    public Sprite GetSkillSprite(int skillAttributeNumber, int skillGradeNumber)
    {
        return skillSpriteSet[skillAttributeNumber,skillGradeNumber];
    }
    
    // 딕셔너리 관리 
    public void EquipSkill(string skillName)
    {
        if (skillEquipMap.ContainsKey(skillName))
        {
            skillEquipMap[skillName] = true;
        }

        else
        {
            
            skillEquipMap.Add(skillName, true);
        }
    }

    public void ReleaseSkill(string skillName)
    {
        skillEquipMap[skillName] = false;
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

