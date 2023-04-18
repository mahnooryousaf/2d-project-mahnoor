using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourHandler : MonoBehaviour
{
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static BehaviourHandler instance;
   

    public LocoMotionState GetIdleRandomeLocoMotion()
    {
        LocoMotionState[] IdleLocmotions ={ LocoMotionState.Idle};
        
        return IdleLocmotions[Random.RandomRange(0, IdleLocmotions.Length)];
    }
    public LocoMotionState GetRandomeLocomotion(CurrentState CharacterObj)
    {

        if(CharacterObj==CurrentState.Idle)
        {
            return GetIdleRandomeLocoMotion();
        }
        else if (CharacterObj== CurrentState.Wander)
        {
            LocoMotionState NewRandomLocomotion = 0;
            int r = Random.RandomRange(0, 100);
            if (r < 40)
            {
                NewRandomLocomotion = LocoMotionState.Walk;
            }
            if (r >= 40)
            {
                NewRandomLocomotion = LocoMotionState.Run;
            }
            return NewRandomLocomotion;
        }
        else
        {
            return LocoMotionState.Run;
        }

       
    }
    public CurrentState GetRandomState(CurrentState CharacterObj)

            {
                CurrentState NewRandomState = 0;
                int r = Random.RandomRange(0, 100);
                if(r<40 && CharacterObj!=CurrentState.Idle)
                    {
                        NewRandomState=CurrentState.Idle;                        
                    }
                if(r>=40)
                    {
                        NewRandomState = CurrentState.Wander;   
                    }
                return NewRandomState;  

            }


    public double GetRandomStateChangeTimer(CurrentState CharacterObj)
    {
        double timer;
        if(CharacterObj==CurrentState.Idle)
        {
            timer= GetPseudoDoubleWithinRange(3, 5);
        }
        else
        {
            timer = GetPseudoDoubleWithinRange(5,10);
        }
        return timer;
    }


    public double GetPseudoDoubleWithinRange(double lowerBound, double upperBound)
    {
        var random = new System.Random();
        var rDouble = random.NextDouble();
        var rRangeDouble = rDouble * (upperBound - lowerBound) + lowerBound;
        return rRangeDouble;
    }

    public bool IsStateChangeAble(CurrentState CURRENTSTATE)
    {
        if (CURRENTSTATE == CurrentState.Idle || CURRENTSTATE == CurrentState.Wander)
            return true ;
        else
            return false;
    }
    public  void  SetRandomMotion(StateMachine CharacterObj)
            {
               
            }
}
