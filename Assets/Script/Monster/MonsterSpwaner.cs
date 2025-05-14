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
            foreach (var monster in monsterDataList.monsterDataList)
            {
                Logger.Info($"ID: {monster.monsterID}, Name: {monster.monsterName}, HP: {monster.monsterHP}, AttackPower : {monster.monsterAttackPower}," +
                       $"Speed: {monster.monsterSpeed}, strength : {monster.strengthElementalAttribute}, weak : {monster.weakElementalAttribute},  " +
                       $"isBoss : {monster.isBoss}");
            }
        }
        else
        {
            Logger.Error("monsterData.json not found in Resources folder");
        }
        
        WaveController.Instance.OnReadyMonsterSpawn += MakeMonsterSpawnSectionArray;
        WaveController.Instance.OnReadyMonsterSpawn += SpawnMonsterSequence;
    }

    void SpawnMonsterSequence(int spawnMonsterCount)
    {
        for (int i = 0; i < spawnMonsterCount; i++)
        {
            // 필드몹만 소환
            int spawnIndex = Random.Range(0, 5);
            GameObject spawnedMonster =  Instantiate(monsterPrefabs[i], mosterSpawnPointArray[i], Quaternion.Euler(0,-90,0));
            spawnedMonster.GetComponent<MonsterController>().GetMonsterStatus(monsterDataList.monsterDataList[i]);
        }
    }

    void MakeMonsterSpawnSectionArray(int spawnMonsterCount)
    {
        for (int i = 0; i < spawnMonsterCount; i++)
        {
            float sectionLocationX = spawnPoint.position.x + offsetX * (i / 3);
            float  sectionLocationZ = spawnPoint.position.z + offsetZ * (i % 3); 
            mosterSpawnPointArray.Add(new Vector3(sectionLocationX, spawnPoint.position.y, sectionLocationZ));
        }
    }
}
