using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    int monsterHp = 5;
    float monsterAtk = 2;
    float monsterSpd = 1.0f;
    string strengthAttribute;
    string weakAttribute;
    bool isMoving = false;
    Vector3 targetPostion;
    Animator monsterAnimator;
    public ParticleSystem monsterDamageEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoving = true;
        monsterAnimator = GetComponent<Animator>();
        monsterAnimator.SetBool("IsMoving", isMoving);
        targetPostion = transform.position + new Vector3(-11, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MonsterMove();

    }

    void MonsterMove()
    {
        if (Vector3.Distance(transform.position, targetPostion) < 0.01f)
        {
            return;
        }
        Debug.Log("몬스터 이동중");

        transform.position = Vector3.Lerp(transform.position, Vector3.MoveTowards(transform.position, targetPostion, monsterSpd * Time.deltaTime), 3.0f);
        Debug.Log("타겟 위치 디버깅 " + targetPostion);


    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("PlayerNoramalAttack"))
        {
            monsterDamageEffect.Play();
        }
    }
}
