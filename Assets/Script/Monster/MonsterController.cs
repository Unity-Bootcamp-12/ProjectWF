using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField]private int monsterHp = 5;
    [SerializeField]private float monsterAttackPower = 2;
    [SerializeField]private float monsterSpeed = 1.0f;
    [SerializeField]private ElementalAttribute strengthAttribute;
    [SerializeField]private ElementalAttribute weakAttribute;
    [SerializeField]private bool isBoss;
    [SerializeField]private int monsterAttackSpeed;
    
    
    bool isMoving = false;
    Vector3 targetPostion;
    Animator monsterAnimator;
    public ParticleSystem monsterDamageEffect;

    private Vector3 fortressPosition;

    private MonsterState currentState = MonsterState.Move;
    
    [SerializeField] private float fortressPositionX;
    
    [Header("AttackRelated")]
    private bool isMonsterAttacking = false;

    private GameObject monsterAttackHitMap;
    
    private MonsterWaveState currentWaveState = MonsterWaveState.Idle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaveController.Instance.OnProgressMonsterActive += ChangeMonsterWaveState;
        
        isMoving = true;
        monsterAnimator = GetComponent<Animator>();
        monsterAnimator.SetBool("IsMoving", isMoving);
        targetPostion = transform.position + new Vector3(-11, 0, 0);
        fortressPosition = new Vector3(fortressPositionX, transform.position.y, transform.position.z);
        monsterAttackHitMap = transform.GetChild(0).gameObject;
        
    }

    // Update is called once per frame
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

        transform.position += new Vector3(-1, 0, 0) * monsterSpeed * Time.deltaTime;;
    }

    public void GetMonsterStatus(MonsterSpwaner.MonsterData status)
    {
        monsterHp = status.monsterHP;
        monsterAttackPower = status.monsterAttackPower;
        monsterSpeed = status.monsterSpeed;
        strengthAttribute = (ElementalAttribute)status.strengthElementalAttribute;
        weakAttribute =  (ElementalAttribute)status.weakElementalAttribute;
        isBoss = status.isBoss == 1 ? true : false;
        monsterAttackSpeed = status.monsterAttackSpeed;
        fortressPositionX = status.monsterDistanceValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("PlayerNoramalAttack"))
        {
            monsterDamageEffect.Play();
            // Destroy(gameObject, 0.5f);
        }
        
        
    }

    public void ActvieMonsterAttackHitMap()
    {
        monsterAttackHitMap.SetActive(true);
    }

    public void DeActvieMonsterAttackHitMap()
    {
        Logger.Info("히트맵 비활성화 함수 작동확인 ");
        monsterAttackHitMap.SetActive(false);
    }

    private void ChangeMonsterWaveState(MonsterWaveState newState)
    {
        currentWaveState = newState;
    }
}
