using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isBallSpawned = false;
    public GameObject magicBall;
    public Transform magicballSpawnTransform;
    Animator playerAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        playerAnimator = GetComponent<Animator>();
        await initailGap();
    }

    // Update is called once per frame
    async void Update()
    {
       await MagicBallAttack();
    }

    async UniTask initailGap()
    {
        await UniTask.Delay(500);
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

        await UniTask.Delay(500);

        Instantiate(magicBall,magicballSpawnTransform.position,Quaternion.identity);

        await UniTask.Delay(700);

        isBallSpawned=false;





    }
}
