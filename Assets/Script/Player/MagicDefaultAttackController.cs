using System.Linq;
using UnityEngine;

public class MagicDefaultAttackController : MonoBehaviour
{
    MonsterController monsterController;
    float magicBallSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        monsterController = FindAnyObjectByType<MonsterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MagicBallMove();
    }

    void MagicBallMove()
    {
        if (Vector3.Distance(transform.position, monsterController.transform.position) < 0.01f)
        {
            return;
        }
        Debug.Log("매직볼 이동중");

        transform.position = Vector3.Lerp(transform.position, Vector3.MoveTowards(transform.position, monsterController.transform.position,magicBallSpeed * Time.deltaTime), 3.0f);
        Debug.Log("매직볼 위치 디버깅 " + monsterController.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Contains("Monster"))
        {
            Destroy(this.gameObject,0.5f);

        }
    }
}
