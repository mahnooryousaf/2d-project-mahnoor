using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string ItemPrompt;
    public Items ItemType;
    public Items RequiredItemType;
    public bool isItemRequired = true;

    public Animator AnimController;
    public bool CheckLockStatus()
    {
       return  GameManager.instance.IsItemContained(RequiredItemType);
    }
    public void OpenDoor()
    {
        AnimController.enabled = true;
        this.gameObject.tag = "Untagged";
        this.GetComponent<BoxCollider>().enabled = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
