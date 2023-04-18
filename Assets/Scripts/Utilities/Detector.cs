using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public StateMachine CharacterObj;
    private Transform LastPosition;
    public CharacterType TypeOfCharacter;
    public float range;
    private float Delay = 0;
    void Start()
    {
        Manager.instance.SetRange(this);
    }
    void Update()
    {       
        RaycastHit hit;
        int layer_mask;        
        if (TypeOfCharacter==CharacterType.Enemy)
        {
            layer_mask = LayerMask.GetMask("Player","Environment");

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, range,layer_mask))
            {
                if ((hit.transform.gameObject.tag != "Player"))
                    {
                       

                        if (CharacterObj.GetState()==CurrentState.ChasingPlayer)
                            {
                                CharacterObj.GetCharacterBeingFollowed().SetWanderPosition(RandomDirectionAttribute.ReverseCurrentDirection);
                                CharacterObj.ConvertState(CurrentState.CheckingLastPosition);
                            }
                       
                        return;
                    }


                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    LastPosition = hit.collider.transform;
                    CharacterObj.Attributes.Target = hit.collider.gameObject;

                    
                    
                    if (hit.distance < GameManager.instance.DistanceForEnemyToShoot)
                    {
                        if (CharacterObj.GetState() != CurrentState.KillingPlayer)
                        {
                            CharacterObj.ConvertState(CurrentState.KillingPlayer);

                        }
                    }
                    else
                    {

                        CharacterObj.Attributes.SetTarget(LastPosition.position, hit.collider.gameObject);
                        if (CharacterObj.GetState() != CurrentState.ChasingPlayer)
                            CharacterObj.ConvertState(CurrentState.ChasingPlayer);
                    }            
            }
            else
            {
                if (CharacterObj.GetState() == CurrentState.ChasingPlayer)
                {
                   
                    //CharacterObj.GetCharacterBeingFollowed().Getew(true);
                    CharacterObj.ConvertState(CurrentState.CheckingLastPosition);
                }

                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);

            }
        }        
    
    }

   
}
