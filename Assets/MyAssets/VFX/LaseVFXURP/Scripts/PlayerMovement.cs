using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public CameraShake cameraShake;
    private Ray rayMouse;
    public RaycastHit hit;
    public Transform shootingpoint;
    public GameObject[] laser;
    int i = 0;
    GameObject LaserObj;
    LineRenderer line;

    void Start()
    {
        Instance = this;
        LaserObj = Instantiate(laser[i], shootingpoint);
        line = LaserObj.GetComponentInChildren<LineRenderer>();
        i++;
       
        LaserObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(LaserObj);
            LaserObj = Instantiate(laser[i], shootingpoint);

            line = LaserObj.GetComponentInChildren<LineRenderer>();
            LaserObj.SetActive(false);
            i++;
            if (i == laser.Length)
                i = 0;
        }
        var mousePos = Input.mousePosition;
        rayMouse = Camera.main.ScreenPointToRay(mousePos);
        if(Physics.Raycast(rayMouse,out hit, 200))
        {
            Debug.DrawLine(shootingpoint.transform.position, hit.point);

        }

        if (Input.GetMouseButtonDown(0))
        {
            LaserObj.SetActive(true);
            StartCoroutine(cameraShake.Shake(2f, 0.1f));
        }

            if (Input.GetMouseButtonUp(0))
            LaserObj.SetActive(false);
    }
    private void OnPointerDown(UnityEngine.UIElements.PointerDownEvent evt)
    {
       
    }

    
}
