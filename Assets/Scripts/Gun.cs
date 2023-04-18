using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 5f;
    public float range = 200f;
    public Camera camera;


    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward,out hit, range))
        {
            Debug.Log(hit.transform.name);
        }
    }
}
