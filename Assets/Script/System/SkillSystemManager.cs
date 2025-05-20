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
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public int skillCoolTime;
        public string skillExplainText;
        public int skillLevel;
        public int skillAttribute;
        public int skillDamagePower;

    }


    [System.Serializable]
    public class SkillDataList
    {
        public List<SkillData> skillDataList;
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
    private SkillDataList skillDataList;

    void Start()
    {
        skillEquipMap = new Dictionary<string, bool>();
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/SkillDataJson");
        if (jsonFile != null)
        {
            skillDataList = JsonUtility.FromJson<SkillDataList>(jsonFile.text);
        }
        else
        {
            Logger.Error("monsterData.json not found in Resources folder");
        }
        InitSkillButtonStatus();
      
    }
    // 스킬 버튼 초기설정
    void InitSkillButtonStatus()
    {
        int indexOffset = 0;

        for (int i = 0; i < skillTreeButtonGroup.transform.childCount; i++)
        {
            for (int j = 0; j < skillTreeButtonGroup.transform.GetChild(i).childCount - 1; j++)
            {
                Transform skillButtonTransForm = skillTreeButtonGroup.transform.GetChild(i).GetChild(j + 1).GetChild(0);
                string skillName = skillDataList.skillDataList[j + indexOffset].skillName;
                Sprite skillSprite = Resources.Load<Sprite>($"IconData/{skillName}");
                skillButtonTransForm.gameObject.GetComponent<SkillButtonController>().SetSkillButtonStatusInfo(skillDataList.skillDataList[j + indexOffset], skillSprite);
            }
            indexOffset += 3;
        }
    }
    
    // 스킬 설명창 설정 
    public void InitDialogueInfo(SkillSystemManager.SkillData skillStatus, Sprite skillSprite)
    {
        skillDialogue.SetActive(true);
        skillDialogue.GetComponent<SkillDialogueManager>().SetDialogueSkillStatusInfo(skillStatus, skillSprite);
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
    
    public void EquipSkillToSkillSet(SkillSystemManager.SkillData skillData,Sprite sprite) //스킬 장착
    {
        for (int i = 0; i < ownedSkillButtonSet.Length; i++)
        {
            Logger.Info($"{i} 번째 버튼 ");
            if (ownedSkillButtonSet[i].GetComponent<OwendSkillButtonController>().IsSkillButtonNull())
            {   
               
                ownedSkillButtonSet[i].GetComponent<OwendSkillButtonController>().SetOwnedSkillButtonSkillStatusInfo(skillData, sprite);
                break;  
            }
           
        }
    }
    
    

    public void ReleaseSkillFromSkillSet(string skillName)
    {
        for (int i = 0; i < ownedSkillButtonSet.Length; i++)
        {
            if (ownedSkillButtonSet[i].GetComponent<OwendSkillButtonController>().IsSkillButtonNull())
            {
                continue;
            }
            if (ownedSkillButtonSet[i].GetComponent<OwendSkillButtonController>().IsThatSKillName(skillName))
            {
                ownedSkillButtonSet[i].GetComponent<OwendSkillButtonController>().RemoveSKillStatus();
                break;  
            }
        }
        
    }
    
    



}

