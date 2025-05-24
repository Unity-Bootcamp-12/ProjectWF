using UnityEngine;

public class FireExplosion : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
