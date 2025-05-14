using System.Linq;
using UnityEngine;

public class MagicDefaultAttackController : MonoBehaviour
{
    [SerializeField] private float magicBallSpeed = 5f;
    PlayerController playerController;
    Vector3 moveDirection;

    private Transform monsterTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void GetTarget(Transform targetPosition)
    {
        moveDirection = targetPosition.position - transform.position;
        moveDirection.Normalize();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        MagicBallMove();
    }

    void MagicBallMove()
    {
        /*
        if (Vector3.Distance(transform.position, monsterTarget.transform.position) < 0.01f)
        {
            return;
        }
        */


        transform.Translate(moveDirection * (magicBallSpeed * Time.deltaTime));
        

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Monster"))
        {
            Destroy(this.gameObject, 0.5f);
        }
    }
}