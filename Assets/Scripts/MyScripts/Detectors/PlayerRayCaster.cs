using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayCaster : MonoBehaviour
{
    public GameObject Cam;
    private RaycastHit objectHit;
    private bool IsObjectInView = false;
    public CharacterController Player;
    public void CheckForwardRaycast()
    {
        int layerId = 8;
        int layerMask = 1 << layerId;

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 2, layerMask);



        if(hitColliders.Length!=0)
        {
         
            for(int x=0;x<hitColliders.Length;x++)
            {
                if (IsObjectInView == false && hitColliders[x].gameObject.tag == "Item")
                {
                    IsObjectInView = true;
                    GameManager.instance.SetObjectInViewStatus(hitColliders[0].transform.gameObject, true, hitColliders[0].GetComponent<Item>().ItemPrompt);
                }
                if (IsObjectInView == false && hitColliders[x].gameObject.tag == "Door")
                {
                    IsObjectInView = true;
                    GameManager.instance.SetObjectInViewStatus(hitColliders[0].transform.gameObject, true, hitColliders[0].GetComponent<Door>().ItemPrompt);
                }
            }
              


        }


        else
        {
            if (IsObjectInView == true)
            {
                IsObjectInView = false;
                GameManager.instance.SetObjectInViewStatus(null, false);
            }
        }
    }

    void Start()
    {
        
    }
    void ExplosionDamage(Vector3 center, float radius)
    {
      
    }
    // Update is called once per frame
    void Update()
    {

        CheckForwardRaycast();
       // ExplosionDamage(this.transform.position, 10);
    }
}
