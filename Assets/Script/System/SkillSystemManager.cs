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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skillEquipMap = new Dictionary<string, bool>();
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/SkillDataJson");
        if (jsonFile != null)
        {
            skillDataList = JsonUtility.FromJson<SkillDataList>(jsonFile.text);
            foreach (var skills in skillDataList.skillDataList)
            {
                //Logger.Info($"스킬명 : {skills.skillName},{skills.skillExplainText}");
            }
        }
        else
        {
            Logger.Error("monsterData.json not found in Resources folder");
        }
        SetSkillButtonStatus();
        //testSkillButton.GetComponent<SkillButtonController>().skillButtonImage = GetComponent<Image>();
        //testSkillButton.GetComponent<SkillButtonController>().GetSkillStatus(skillDataList.skillDataList[0]);
    }

    void SetSkillButtonStatus()
    {
        int indexOffset = 0;

        for (int i = 0; i < skillTreeButtonGroup.transform.childCount; i++)
        {
            for (int j = 0; j < skillTreeButtonGroup.transform.GetChild(i).childCount - 1; j++)
            {
                Transform skillButtonTransForm = skillTreeButtonGroup.transform.GetChild(i).GetChild(j + 1).GetChild(0);
                Logger.Info("버튼 객체 디버깅 확인:" + skillButtonTransForm.gameObject.name);
                string skillName = skillDataList.skillDataList[j + indexOffset].skillName;
                Sprite skillSprite = Resources.Load<Sprite>($"IconData/{skillName}");
                skillButtonTransForm.gameObject.GetComponent<SkillButtonController>().GetSkillStatus(skillDataList.skillDataList[j + indexOffset], skillSprite);
            }
            indexOffset += 3;
        }
    }

    public void EquipSkill(string skillName)//스킬 장착
    {
        if (skillEquipMap.ContainsKey(skillName))
        {
            skillEquipMap[skillName] = true;
        }

        else
        {
            Logger.Info("메인 매니저 장착프로세스 확인절차");
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
            Logger.Info("장착 여부 확인");
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
    // Update is called once per frame
    void Update()
    {

    }


}

