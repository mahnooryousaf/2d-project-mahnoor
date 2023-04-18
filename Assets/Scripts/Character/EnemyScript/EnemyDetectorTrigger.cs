using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectorTrigger : MonoBehaviour
{
    private Transform LastPosition;
    public bool inarea = false;
    RaycastHit hit;
    public StateMachine EnemyCharacter;
    public enum TriggerType
    {
        //It will draw a raycast
       OuterTrigger,
       //It will not draw a raycase and will confirm following the enemy
       InnerTrigger
    }
    public TriggerType EnemyTriggerType;
    int layer_mask;

    void Start()
    {
            layer_mask = LayerMask.GetMask("Player", "Environment");

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {    
            if(EnemyTriggerType==TriggerType.OuterTrigger)
            {
                inarea = true;
                if (EnemyCharacter.GetState() == CurrentState.Wander)
                {
                    if (Physics.Raycast(transform.position, (new Vector3(other.transform.position.x, other.transform.position.y +5, other.transform.position.z) - transform.position), out hit, layer_mask))
                    {                           
                        if (hit.transform.tag == "Player")
                        {
                            LastPosition = hit.transform;
                            EnemyCharacter.ConvertState(CurrentState.ChasingPlayer, LocoMotionState.Run);
                                             
                                EnemyCharacter.Attributes.SetTarget(LastPosition.position, hit.collider.gameObject);
                                if (EnemyCharacter.GetState() != CurrentState.ChasingPlayer)
                                    EnemyCharacter.ConvertState(CurrentState.ChasingPlayer);
                        }
                    }
                    else
                    {
                        if (EnemyCharacter.GetState() == CurrentState.ChasingPlayer)
                        {
                            EnemyCharacter.GetCharacterBeingFollowed().SetWanderPosition(RandomDirectionAttribute.TowardsCurrentDirection);
                            EnemyCharacter.ConvertState(CurrentState.CheckingLastPosition);
                        }
                    }
                }                     
            }           
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            inarea = true;
            if (EnemyTriggerType == TriggerType.InnerTrigger)
            {

                EnemyCharacter.Attributes.SetTarget(other.transform.position, other.gameObject);

              EnemyCharacter.ConvertState(CurrentState.ChasingPlayer, LocoMotionState.Run);
                if (EnemyCharacter.GetState() != CurrentState.KillingPlayer)
                {
                    EnemyCharacter.gameObject.transform.LookAt(other.gameObject.transform);
                    EnemyCharacter.ConvertState(CurrentState.KillingPlayer);
                }


                //// if (Physics.Raycast(transform.position, (new Vector3(other.transform.position.x, other.transform.position.y + 2, other.transform.position.z) - transform.position), out hit, layer_mask))
                ////{
                //GameObject hit = other.gameObject;
                  
                //        if ( hit.transform.tag == "Player")
                //        {
                //            LastPosition = hit.transform;
                //            EnemyCharacter.ConvertState(CurrentState.ChasingPlayer, LocoMotionState.Run);                          
                //                if (EnemyCharacter.GetState() != CurrentState.KillingPlayer)
                //                {
                //        Debug.Log("HAROON");
                //                    EnemyCharacter.ConvertState(CurrentState.KillingPlayer);
                //                }                           
                //     //   }
                //}
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (EnemyTriggerType == TriggerType.InnerTrigger)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Bot")
            {
                inarea = false;
                if (EnemyCharacter.GetState() == CurrentState.ChasingPlayer)
                {

                   // EnemyCharacter.GetCharacterBeingFollowed().SetWanderPosition(RandomDirectionAttribute.ReverseCurrentDirection);
                    EnemyCharacter.ConvertState(CurrentState.CheckingLastPosition);
                }
            }
        }
            
    }
    // Update is called once per frame
    void Update()
    {






        //if (Physics.Raycast(transform.position, (new Vector3(Player.transform.position.x, Player.transform.position.y + 2, Player.transform.position.z) - transform.position), out hit, layer_mask))
        //{
        //    Debug.DrawRay(transform.position, (Player.transform.position - transform.position), Color.green);
        //   // Debug.Log("NAME:" + hit.transform.gameObject.name);
        //}
    }
}
