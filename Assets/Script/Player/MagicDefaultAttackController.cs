using System.Linq;
using UnityEngine;

public class MagicDefaultAttackController : MonoBehaviour
{
    [SerializeField] private float magicBallSpeed = 5f;
    PlayerController playerController;
    Vector3 moveDirection;

    private Transform monsterTarget;

    public void GetTarget(Transform targetPosition)
    {
        moveDirection = targetPosition.position - transform.position;
        moveDirection.Normalize();
        
    }

    void Update()
    {
        MagicBallMove();
    }

    void MagicBallMove()
    {
        transform.Translate(moveDirection * (magicBallSpeed * Time.deltaTime));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Monster"))
        {
            SoundController.Instance.PlaySFX(SFXType.PlayerAttackEffectSound);
            Destroy(this.gameObject);
        }
    }
}