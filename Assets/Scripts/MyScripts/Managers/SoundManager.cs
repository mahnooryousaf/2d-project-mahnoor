using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource FiringSound;
    public AudioSource EnemyFiringSound;
    public AudioSource Reload;
    public AudioSource ObjectiveCompleteSound;
    public AudioSource FootStepSound;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    public void PlayFiringSound(bool status)
    {
        if(status)
        {
            if (FiringSound.isPlaying == false)
            {
                FiringSound.Play();
            }
        }
        else
        {
            FiringSound.Stop();
        }
       
    }
}
