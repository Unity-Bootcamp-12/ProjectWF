using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MonsterSpwaner;
using static SkillSystemManager;

public class SkillSystemManager : MonoBehaviour
{
    [SerializeField] GameObject skillTreeButtonGroup;
    [SerializeField] GameObject testSkillButton;
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
            foreach (var skills in skillDataList.skillDataList)
            {
                //Logger.Info($"��ų�� : {skills.skillName},{skills.skillExplainText}");
            }
        }
        else
        {
            Logger.Error("monsterData.json not found in Resources folder");
        }
        SetSkillButtonStatus();
      
    }

    void SetSkillButtonStatus()
    {
        int indexOffset = 0;

        for (int i = 0; i < skillTreeButtonGroup.transform.childCount; i++)
        {
            for (int j = 0; j < skillTreeButtonGroup.transform.GetChild(i).childCount - 1; j++)
            {
                Transform skillButtonTransForm = skillTreeButtonGroup.transform.GetChild(i).GetChild(j + 1).GetChild(0);
                //Logger.Info("��ư ��ü ����� Ȯ��:" + skillButtonTransForm.gameObject.name);
                string skillName = skillDataList.skillDataList[j + indexOffset].skillName;
                Sprite skillSprite = Resources.Load<Sprite>($"IconData/{skillName}");
                skillButtonTransForm.gameObject.GetComponent<SkillButtonController>().DownloadSkillStatus(skillDataList.skillDataList[j + indexOffset], skillSprite);
            }
            indexOffset += 3;
        }
    }
    // 딕셔너리 관리 
    public void EquipSkill(string skillName)//��ų ����
    {
        if (skillEquipMap.ContainsKey(skillName))
        {
            skillEquipMap[skillName] = true;
        }

        else
        {
            //Logger.Info("���� �Ŵ��� �������μ��� Ȯ������");
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
            //Logger.Info("���� ���� Ȯ��");
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
               
                ownedSkillButtonSet[i].GetComponent<OwendSkillButtonController>().DownloadSkillStatus(skillData, sprite);
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

