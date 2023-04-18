using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    public bool isPlayer = false;

    public float speedValue;
    //private int animStatesID = 0; //  0 idle, walk 1 ,run 2,

    private int moveState = Animator.StringToHash("moveState");
    // hold object mask hands
    private int grabAnim = Animator.StringToHash("GrabHold");

    public Animator playerAnim;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnHoldObject(true);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            OnHoldObject(false);
        }
    }

    public void SetSpeedValue(float speed)
    {
        speedValue = speed;
    }

    void OnAnimatorMove()
    {
        if (isPlayer)
        {
            playerAnim.SetFloat(moveState, speedValue);
        }
      
    }

    public void SetToRun()
    {
        playerAnim.SetInteger(moveState, 2);
    }
    public void SetToIdle()
    {
        playerAnim.SetInteger(moveState, 0);
    }

    public void SetToWalk()
    {
        playerAnim.SetInteger(moveState, 1);
    }

    public void OnHoldObject(bool isHold)
    {
        playerAnim.SetBool(grabAnim, isHold);
    }
    public void SetAnimatorState(int i)
    {
        playerAnim.SetInteger("State", i);
    }
}
