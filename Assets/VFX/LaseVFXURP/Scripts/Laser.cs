using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    float waitTime = 1f;
    float TimeT = 0;
    public GameObject laserImpact;
    public Transform ImpactPosition;
    void Update()
    {
        TimeT += Time.deltaTime;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.DrawLine(ray.origin, hit.point);
            if (hit.collider != null)
            {
                if (TimeT > 0.1f)
                {
                    GameObject muzzle = Instantiate(laserImpact);

                    //   muzzle.transform.position = hit.point-new Vector3(0,0,0.5f);
                    muzzle.transform.position = ImpactPosition.position;
                    muzzle.transform.localRotation = Quaternion.identity;
                    Destroy(muzzle, 1f);
                    TimeT = 0;
                }

            }
        }

    }

}
