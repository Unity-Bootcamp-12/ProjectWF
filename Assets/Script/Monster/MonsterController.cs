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

public enum ElementalAttribute
{
    None,
    Lightning,
    Fire,
    Water
}

public class MonsterController : MonoBehaviour
{
    private int monsterHp;
    private int monsterAttackPower;
    private float monsterSpeed = 1.0f;
    [SerializeField]private ElementalAttribute strengthAttribute;
    [SerializeField]private ElementalAttribute weakAttribute;
    private bool isBoss;
    private int monsterAttackSpeed;
    [SerializeField] private Slider monsterHpSlider;


    bool isMoving = false;
    Vector3 targetPostion;
    Animator monsterAnimator;
    public ParticleSystem monsterDamageEffect;
    [SerializeField] private ParticleSystem weakDamageEffect;
    [SerializeField] private ParticleSystem strengthDamageEffect;

    private Vector3 fortressPosition;

    private MonsterState currentState = MonsterState.Move;

    [SerializeField] private float fortressPositionX;

    private bool isMonsterAttacking = false;

    private GameObject monsterAttackHitMap;

    private MonsterWaveState currentWaveState = MonsterWaveState.Idle;

    private int currentMonsterHP;
    private int playerAttackPower;

    private ElementalAttribute baseAttackAttribute;
    
    void Start()
    {
        GameController.Instance.OnProgressMonsterActive += ChangeMonsterWaveState;
        GameController.Instance.OnChangeBaseAttackAttribute += ChangeBaseAttackAttribute;
        GameController.Instance.OnChangePlayerAttackPower += ChangePlayerAttackPower;
        
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
        if (currentWaveState == MonsterWaveState.Idle || currentState == MonsterState.Die)
        {
            return;
        }

        if (currentState == MonsterState.Move)
        {
            MonsterMove();
        }
        else if (currentState == MonsterState.Attack)
        {
            MonsterAttack();
        }
    }


    async UniTask MonsterAttack()
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
        transform.position += new Vector3(-1, 0, 0) * monsterSpeed * Time.deltaTime;
    }

    public void GetMonsterStatus(MonsterSpwaner.MonsterData status)
    {
        int waveLevel = GameController.Instance.GetWaveLevel();
        monsterHp = status.monsterHP + waveLevel;
        monsterAttackPower = status.monsterAttackPower + waveLevel;
        monsterSpeed = status.monsterSpeed;
        strengthAttribute = (ElementalAttribute)status.strengthElementalAttribute;
        weakAttribute = (ElementalAttribute)status.weakElementalAttribute;
        isBoss = status.isBoss == 1 ? true : false;
        monsterAttackSpeed = status.monsterAttackSpeed;
        fortressPositionX = status.monsterDistanceValue;
    }

    public void TakeDamage(bool isSkill, int amount, ElementalAttribute attribute)
    {
        if (currentState == MonsterState.Die)
        {
            return;
        }
        if (isSkill && GameController.Instance.GetCurrentWaveState() == WaveState.Progress)
        {
            if (attribute == weakAttribute)
            {
                weakDamageEffect.Play();
                amount++;
            }
            else if(attribute == strengthAttribute)
            {
                strengthDamageEffect.Play();
                amount--;
            }
            else
            {
                monsterDamageEffect.Play();
            }
            
            currentMonsterHP -= amount;
        }
        else if (!isSkill && GameController.Instance.GetCurrentWaveState() == WaveState.Progress)
        {
            monsterDamageEffect.Play();
            currentMonsterHP -= amount;
        }

        monsterHpSlider.value = (float)currentMonsterHP / monsterHp;

        if (currentMonsterHP <= 0)
        {
            MonsterDead();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("PlayerNoramalAttack"))
        {
            TakeDamage(false, playerAttackPower, baseAttackAttribute);
        }
        else if (other.gameObject.tag.Contains("MonsterAttackPosition"))
        {
            currentState = MonsterState.Attack;
            monsterAnimator.SetBool("IsMoving", false);
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

        MonsterDeadTask();
    }

    async UniTask MonsterDeadTask()
    {
        monsterAnimator.SetTrigger("Death");
        monsterSpeed = 0;
        
        float elapsedTime = 0.0f;
        float duration = 2f;
        float fallSpeed = 0.01f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.timeScale;
            await UniTask.DelayFrame(1);
        }

        Destroy(gameObject);
    }

    public void ActvieMonsterAttackHitMap()
    {
        monsterAttackHitMap.SetActive(true);
    }

    public void DeActvieMonsterAttackHitMap()
    {
        monsterAttackHitMap.SetActive(false);
    }

    private void ChangeMonsterWaveState(MonsterWaveState newState)
    {
        currentWaveState = newState;
        playerAttackPower = GameController.Instance.GetPlayerAttackPower();
    }

    private void ChangeBaseAttackAttribute(ElementalAttribute attribute)
    {
        baseAttackAttribute = attribute;
    }

    public void ChangePlayerAttackPower(int value)
    {
        playerAttackPower = value;
    }
}