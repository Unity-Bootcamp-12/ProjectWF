using System.Collections.Generic;
using UnityEngine;

public class MonsterSpwaner : MonoBehaviour
{
    [System.Serializable]
    public class MonsterData
    {
        public int monsterID;
        public string monsterName;
        public int monsterHP;
        public int monsterAttackPower;

        public float monsterSpeed;

        // 0 : None / 1 : Lightning / 2 : Fire / 3 : Water
        public int strengthElementalAttribute;
        public int weakElementalAttribute;
        public int isBoss;
        public int monsterAttackSpeed;
        public float monsterDistanceValue;
    }

    [System.Serializable]
    public class MonsterDataList
    {
        public List<MonsterData> monsterDataList;
    }

    [SerializeField] private Transform spawnPoint;
    private List<Vector3> mosterSpawnPointArray = new List<Vector3>();
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetZ;

    [SerializeField] private GameObject[] monsterPrefabs;
    private MonsterDataList monsterDataList;

    void Start()
    {
        // .json 확장자는 생략
        TextAsset jsonFile = Resources.Load<TextAsset>("JsonData/MonsterDataJson");

        if (jsonFile != null)
        {
            monsterDataList = JsonUtility.FromJson<MonsterDataList>(jsonFile.text);
        }
        else
        {
            Logger.Error("monsterData.json not found in Resources folder");
        }

        GameController.Instance.OnReadyMonsterSpawn += MakeMonsterSpawnSectionArray;
        GameController.Instance.OnReadyMonsterSpawn += SpawnMonsterSequence;
        GameController.Instance.OnBossSpawn += SpawnBossMonster;
    }

    void SpawnBossMonster(int waveLevel)
    {
        int bossIndex = waveLevel % 10 == 0 ? 3 : 4;
        GameObject spawnedBossMonster = Instantiate(monsterPrefabs[bossIndex],
            mosterSpawnPointArray[1], Quaternion.Euler(0, -90, 0));
        spawnedBossMonster.GetComponent<MonsterController>()
            .GetMonsterStatus(monsterDataList.monsterDataList[bossIndex]);
        
        mosterSpawnPointArray.Clear();
    }

    void SpawnMonsterSequence(int spawnMonsterCount, int waveLevel)
    {
        for (int i = 0; i < spawnMonsterCount; i++)
        {
            if (i == spawnMonsterCount - 1 && waveLevel % 5 == 0)
            {
                break;
            }
            else
            {
                int spawnIndex = Random.Range(0, 3);
                GameObject spawnedMonster = Instantiate(monsterPrefabs[spawnIndex], mosterSpawnPointArray[i],
                    Quaternion.Euler(0, -90, 0));
                spawnedMonster.GetComponent<MonsterController>()
                    .GetMonsterStatus(monsterDataList.monsterDataList[spawnIndex]);
            }
        }

        if (waveLevel % 5 != 0)
        {
            mosterSpawnPointArray.Clear();
        }
    }

    void MakeMonsterSpawnSectionArray(int spawnMonsterCount, int waveLevel)
    {
        for (int i = 0; i < spawnMonsterCount; i++)
        {
            if (i == spawnMonsterCount - 1 && (waveLevel % 5) == 0)
            {
                break;
            }
            else
            {
                float sectionLocationX = spawnPoint.position.x + offsetX * (i / 3);
                float sectionLocationZ = spawnPoint.position.z + offsetZ * (i % 3);
                mosterSpawnPointArray.Add(new Vector3(sectionLocationX, spawnPoint.position.y, sectionLocationZ));
            }
        }
    }
}