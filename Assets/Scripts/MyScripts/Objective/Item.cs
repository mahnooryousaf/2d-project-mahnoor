using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string ItemPrompt;
    public Items ItemType;
    public Animator[] Anim;
    public AudioSource[] AudioSourcesToBeTurnedOff;

    public void TurnAnims(bool status)
    {
        for (int i = 0; i < Anim.Length; i++)
        {
            Anim[i].enabled = status;
        }
    }
    public void TurnOffAudios()
    {
        for (int i = 0; i < AudioSourcesToBeTurnedOff.Length; i++)
        {
            AudioSourcesToBeTurnedOff[i].enabled = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
