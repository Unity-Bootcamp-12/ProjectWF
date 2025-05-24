using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterState
{
    Move,
    Attack,
    Die
}

public enum MonsterWaveState
{
    Idle,
    Active
}

public class MonsterController : MonoBehaviour
{
    private int monsterHp;
    private int monsterAttackPower;
    private float monsterSpeed = 1.0f;
    private ElementalAttribute strengthAttribute;
    private ElementalAttribute weakAttribute;
    private bool isBoss;
    private int monsterAttackSpeed;
    [SerializeField] private Slider monsterHpSlider;
    
    
    bool isMoving = false;
    Vector3 targetPostion;
    Animator monsterAnimator;
    public ParticleSystem monsterDamageEffect;

    private Vector3 fortressPosition;

    private MonsterState currentState = MonsterState.Move;
    
    [SerializeField] private float fortressPositionX;
    
    private bool isMonsterAttacking = false;

    private GameObject monsterAttackHitMap;
    
    private MonsterWaveState currentWaveState = MonsterWaveState.Idle;
    
    private int currentMonsterHP;
    private int playerAttackPower;

    void Start()
    {
        GameController.Instance.OnProgressMonsterActive += ChangeMonsterWaveState;
        
        isMoving = true;
        monsterAnimator = GetComponent<Animator>();
        monsterAnimator.SetBool("IsMoving", isMoving);
        targetPostion = transform.position + new Vector3(-11, 0, 0);
        fortressPosition = new Vector3(fortressPositionX, transform.position.y, transform.position.z);
        monsterAttackHitMap = transform.GetChild(0).gameObject;
        
        transform.GetChild(0).GetComponent<MonsterHitMap>().SetparentMonsterPower(monsterAttackPower);
        currentMonsterHP = monsterHp;
        
        if (isBoss)
        {
            currentWaveState = MonsterWaveState.Active;
            playerAttackPower = GameController.Instance.GetPlayerAttackPower();
        }
    }

    void Update()
    {
        if (currentWaveState == MonsterWaveState.Idle)
        {
            return;
        }
        
        if (currentState == MonsterState.Move)
        {
            MonsterMove();
        }
        else if (currentState == MonsterState.Attack)
        {
            monsterAttack();
        }
        else if (currentState == MonsterState.Die)
        {
            // 죽는 uniTask 실행
        }

    }

    async UniTask monsterAttack()
    {
        if (isMonsterAttacking)
        {
            return;
        }
        
        isMonsterAttacking = true;
        //공격 시퀀스
        monsterAnimator.SetTrigger("Attack");
        await UniTask.Delay(monsterAttackSpeed);
        isMonsterAttacking = false;
    }

    void MonsterMove()
    {
        if (Mathf.Abs(transform.position.x - fortressPosition.x) < 0.01f)
        {
            currentState = MonsterState.Attack;
            monsterAnimator.SetBool("IsMoving", false);
            return;
        }

        transform.position += new Vector3(-1, 0, 0) * monsterSpeed * Time.deltaTime;
    }

    public void GetMonsterStatus(MonsterSpwaner.MonsterData status)
    {
        int waveLevel = GameController.Instance.GetWaveLevel();
        monsterHp = status.monsterHP + waveLevel;
        monsterAttackPower = status.monsterAttackPower + waveLevel;
        monsterSpeed = status.monsterSpeed;
        strengthAttribute = (ElementalAttribute)status.strengthElementalAttribute;
        weakAttribute =  (ElementalAttribute)status.weakElementalAttribute;
        isBoss = status.isBoss == 1 ? true : false;
        monsterAttackSpeed = status.monsterAttackSpeed;
        fortressPositionX = status.monsterDistanceValue;
    }

    public void TakeDamage(bool isSkill, int amount)
    {
        if (isSkill && GameController.Instance.GetCurrentWaveState() == WaveState.Progress)
        {
            currentMonsterHP -= amount;
        }
        else if(!isSkill && GameController.Instance.GetCurrentWaveState() == WaveState.Progress)
        {
            currentMonsterHP -= amount;            
        }
        monsterHpSlider.value = (float)currentMonsterHP/monsterHp;

        if (currentMonsterHP <= 0)
        {
            MonsterDead();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("PlayerNoramalAttack"))
        {
            monsterDamageEffect.Play();
            TakeDamage(false, playerAttackPower);
        }
    }   

    private void MonsterDead()
    {
        if (currentState == MonsterState.Die)
        {
            return;   
        }
        
        currentState = MonsterState.Die;
        isMoving = false;
        DeActvieMonsterAttackHitMap();
        GetComponent<Collider>().enabled = false;
        monsterHpSlider.gameObject.SetActive(false);
        GameController.Instance.IncreaseKillCount();
        Destroy(gameObject, 0.1f);
    }

    public void ActvieMonsterAttackHitMap()
    {
        monsterAttackHitMap.SetActive(true);
    }

    public void DeActvieMonsterAttackHitMap()
    {
        //Logger.Info("히트맵 비활성화 함수 작동확인 ");
        monsterAttackHitMap.SetActive(false);
    }

    private void ChangeMonsterWaveState(MonsterWaveState newState)
    {
        currentWaveState = newState;
        playerAttackPower = GameController.Instance.GetPlayerAttackPower();
    }
}
