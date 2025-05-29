using Cysharp.Threading.Tasks;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Attack
}

public class PlayerController : MonoBehaviour
{
    private int attackTime = 1200;
    bool isBallSpawned = false;
    public GameObject magicBall;
    public Transform magicballSpawnTransform;
    Animator playerAnimator;

    [Header("SearchCast")] 
    private RaycastHit[] monsterHits;
    private Vector3 searchPosition = new Vector3(1.5f, 1f, 0f);
    private Vector3 halfExtents = new Vector3(6f, 0.5f, 6f);
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private Transform playerPosition;

    public GameObject monsterTarget;

    [SerializeField]private PlayerState state = PlayerState.Idle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        playerAnimator = GetComponent<Animator>();

        GameController.Instance.OnProgressPlayerControl += ChangePlayerState;
        
        
        await SearchMonsterByDuration();
    }
    

    async UniTask SearchMonsterByDuration()
    {
        while (true)
        {
            await UniTask.Delay(500);
            SearchNearestMonster();
        }
        
    }

    async UniTask MagicBallAttack()
    {
        if (isBallSpawned)
        {
            return;
        }
        
        isBallSpawned = true;

        await UniTask.Delay(100);

        playerAnimator.SetTrigger("Attack");

        await UniTask.Delay(attackTime);
        
        if (monsterTarget != null)
        {
            GameObject spawnedMagicBall=Instantiate(magicBall, magicballSpawnTransform.position, Quaternion.identity);
            spawnedMagicBall.GetComponent<MagicDefaultAttackController>().GetTarget(monsterTarget.transform);
        }
        await UniTask.Delay(700);

        isBallSpawned = false;
    }

    async void SearchNearestMonster()
    {
        if (state == PlayerState.Idle)
        {
            return;
        }
        monsterHits = Physics.BoxCastAll(searchPosition, halfExtents, Vector3.forward, Quaternion.identity, 0,
            monsterLayer);

        if (monsterHits.Length > 0)
        { 
            RaycastHit nearestMonsterHit = monsterHits[0];
            float minDistance = Vector3.Distance(transform.position, nearestMonsterHit.transform.position);
            foreach (var monsterHit in monsterHits)
            {
                float Distance = Vector3.Distance(transform.position, monsterHit.transform.position);
                if (minDistance > Distance)
                {
                    nearestMonsterHit = monsterHit;
                    minDistance = Distance;
                }
            }

            monsterTarget = nearestMonsterHit.transform.gameObject;
            await MagicBallAttack();
        }

        else
        {
            return;
        }
    }

    private void ChangePlayerState(PlayerState state)
    {
        this.state = state;
    }

    public void PlayerAttackSound()
    {   
        SoundController.Instance.PlaySFX(SFXType.PlayerAttackSound);
    }
}