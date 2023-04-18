using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
public class StateMachine : MonoBehaviour
{
        public States Attributes;            
        //Private Variables
        private Transform _transform;
        public Vector3 RandomLocation;        
        private GameObject LastPositionGameObject;
        public  Material Radar;
        public GameObject[] EnemyDetectors;
        private int StateChangeDelay = 0;
        public TextMeshProUGUI Name;
        public float CollisionCoolDown=0;

        public float BulletCoolDown = 1f;

        private bool isNavStarted = false;
        public GameObject BulletPosition;

    public Image HealthImg;

    void Awake()
    {
        Attributes.nav = this.GetComponent<NavMeshAgent>();
        Attributes.Anim = this.GetComponent<Animator>();

        if(Attributes.Type==CharacterType.Enemy)
        {
            if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 0)
            {
                for (int i = 0; i < EnemyDetectors.Length; i++)
                {
                    EnemyDetectors[i].SetActive(false);
                }
            }
        }
      
    }
    public void DecrementHealth(GameObject Targ)
    {
        Attributes.SetTarget(Targ.transform.position, Targ);
        RotateTowardsPlayer();
        ConvertState(CurrentState.ChasingPlayer, LocoMotionState.Run);

        Attributes.Health -= 5f;
        HealthImg.fillAmount = Attributes.Health / 100f;
        if(Attributes.Health<=0)
        {
            ConvertState(CurrentState.Dead, LocoMotionState.Dead);
        }
    }
    public void StartState()
    {
        isNavStarted = true;
     }
    void Start()
    {
        Manager.instance.SetCharacterSpeed(this.GetComponent<StateMachine>());
        _transform = this.transform;
      
        ConvertState(Attributes.State, Attributes.CurrentMovementState);

        Invoke("StartState", 0.5f);

       




        SetWanderPosition(RandomDirectionAttribute.Random);

    }
    public void Wandering()
    {       
        Attributes.nav.SetDestination(RandomLocation);    
        if (Vector3.Distance(this.transform.position, RandomLocation) < 0.5f && Attributes.nav.isStopped == false)
        {
            SetWanderPosition(RandomDirectionAttribute.Random);
        }
    }
    
    public void ToogleDetectors(bool Status)
    {
        for(int i=0;i<EnemyDetectors.Length;i++)
        {
            EnemyDetectors[i].gameObject.SetActive(Status);
        }
    }
    public void SetLocomotionState(LocoMotionState LocoState = LocoMotionState.Walk)
    {

        Attributes.Anim.SetInteger("State", (int) LocoState);        
        if (LocoState == LocoMotionState.Idle)
        {
            Attributes.nav.speed = 0;           
        }          
        else if (LocoState == LocoMotionState.Walk)
            Attributes.nav.speed = Attributes.WalkSpeed;
        else if (LocoState == LocoMotionState.Run)
            Attributes.nav.speed = Attributes.RunSpeed;
        else if (LocoState == LocoMotionState.Dead)
            {
                Attributes.nav.speed = 0;
                Attributes.Anim.SetTrigger("Dead");
            }
        Attributes.CurrentMovementState = LocoState;
    }


    public bool BlockChangeState(CurrentState NewState)
    {
        bool status = false;
        if(Attributes.Type==CharacterType.Enemy)
        {
            if (Attributes.State == CurrentState.GoingForTask && NewState != CurrentState.PerformingTask)
                status=true ;
        }
        return status;
    }
    public void ConvertState(CurrentState NewState, LocoMotionState LocoState=LocoMotionState.Walk)
    {
       

            Attributes.PreviousState = Attributes.State;

        if(NewState==CurrentState.ChasingPlayer)
        {
            if(isTargetReachAble()==false)
            {
                NewState = CurrentState.KillingPlayer;
            }
        }
       
        if (Attributes.Type == CharacterType.Enemy)
        {
            if (NewState == CurrentState.Dead)
            {
                tag = "Untagged";
                Attributes.nav.isStopped = true;
                this.GetComponent<CapsuleCollider>().enabled = false;
                this.GetComponent<Rigidbody>().isKinematic = true;
                SetLocomotionState(LocoMotionState.Dead);
                Destroy(this.gameObject, 5f);
            }

            if (NewState == CurrentState.Wander)
            {
                Attributes.nav.isStopped = false;
                SetLocomotionState(LocoState);
                Radar.SetColor("_Color", Color.green);
            }  
            if (NewState == CurrentState.ChasingPlayer )
            {
                //Destroy Last Position GameObject
                if (LastPositionGameObject != null)
                    Destroy(LastPositionGameObject);
                SetLocomotionState(LocoMotionState.Run);
                Radar.SetColor("_Color", Color.red);
                Attributes.nav.isStopped = false;

            }
            if(NewState==CurrentState.KillingPlayer)
            {
                Attributes.nav.speed = 0;
                Attributes.nav.isStopped = true;

                Attributes.Anim.SetInteger("State", (int)LocoMotionState.Shooting);

                //Attributes.Target.GetComponent<Character>().ConvertState(CurrentState.Dead);
                StateChangeDelay = 1;
               // Invoke("TaskCompleted", 2f);
            }
            if (NewState == CurrentState.PerformingTask)
            {
      

                //If It has peformed the slipping Task
                //
                Attributes.CurrentTask++;
                //If all Tasks are completed
   
                Attributes.nav.isStopped = true;
                Manager.instance.DelayLevelFinished();

             

            }
            if (NewState==CurrentState.CheckingLastPosition)
            {
                if (LastPositionGameObject != null)
                    Destroy(LastPositionGameObject);
                LastPositionGameObject = Instantiate(Manager.instance.LastPositionObject);
                LastPositionGameObject.transform.position = Attributes.TargetLastPosition;
                Color DetectionColour = Color.cyan;
                Radar.SetColor("_Color", DetectionColour);
                Attributes.nav.isStopped = false;
            }
            if (NewState==CurrentState.GoingForTask)
            {
                this.GetComponent<CapsuleCollider>().isTrigger = false;
               
              
           
            }
        }
       

        Attributes.State = NewState;

        if (NewState == CurrentState.PerformingTask && Attributes.Type != CharacterType.Enemy)
        {
            SetLocomotionState(LocoMotionState.Interect);
      




        }
        else
        {

        }
    }
    
    public void TaskCompleted()
    {      
     
        if (Attributes.Type==CharacterType.Enemy)
        {
            if(StateChangeDelay==1)
            {
                StateChangeDelay = 0;
                Attributes.nav.isStopped = false;
                //ConvertState(Attributes.PreviousState);


                ConvertState(CurrentState.Wander, LocoMotionState.Walk);

                this.GetComponent<CapsuleCollider>().isTrigger = true;
            }

        
        }
      
    }
    public void ReachActivity()
    {
        //Attributes.nav.SetDestination(Attributes.CurrentActivity.TaskPosition);
        //if (Vector3.Distance(this.transform.position, Attributes.CurrentActivity.TaskPosition) < 4 && Attributes.nav.isStopped == false)
        //{
          
        //    ConvertState(CurrentState.PerformingTask);
        //   // Attributes.nav.isStopped = true;
        //}
    }
    public void FollowPlayer()
    {
        if(Attributes.Target==null)
        {
            Attributes.Target = GameObject.FindGameObjectWithTag("Player");
        }
        Attributes.nav.SetDestination(Attributes.Target.transform.position);

        if ((Vector3.Distance(this.transform.position, Attributes.Target.transform.position) < GameManager.instance.DistanceForEnemyToShoot && Attributes.nav.isStopped == false) || isTargetReachAble() == false)
        {


            ConvertState(CurrentState.KillingPlayer);
          //  Debug.Log("SHOOTING");
            RotateTowardsPlayer();
        }
    }
    public ParticleSystem BulletMuzzleParticles;
    public void FollowLastPosition()
    {
        Attributes.nav.SetDestination(Attributes.TargetLastPosition);

        if (Vector3.Distance(this.transform.position, Attributes.TargetLastPosition) < 1 && Attributes.nav.isStopped == false)
        {
            //Delete Last Position Object
            if (LastPositionGameObject != null)
                Destroy(LastPositionGameObject);

            //Set Random Position

            //SetRandomPosition(true);            

            SetWanderPosition(RandomDirectionAttribute.TowardsCurrentDirection);

            ////If Bot Was Doing Some Task Then Resume That Task
            //if (Attributes.CurrentActivity!=null)
            //         ConvertState(CurrentState.GoingForTask,LocoMotionState.Run);
            ////Otherwise Return the Bot into Wander State
            //else
            ConvertState(CurrentState.Wander);

        }
    }

    public void ShootingPlayer()
    {
        Attributes.nav.SetDestination(Attributes.Target.transform.position);
      //  Debug.Log("DISTANCE:" + Vector3.Distance(this.transform.position, Attributes.Target.transform.position));

        RotateTowardsPlayer();

     
        if(BulletCoolDown<=0)
        {
            BulletMuzzleParticles.Play();
            Vector3 fwd = BulletPosition.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(BulletPosition.transform.position, fwd * 50, Color.green);
            RaycastHit objectHit;
            SoundManager.instance.EnemyFiringSound.Play();
            if (Physics.Raycast(BulletPosition.transform.position, fwd, out objectHit, 100))
            {
              


                if (objectHit.transform.gameObject.name == "Player")
                {
                    GameManager.instance.DecrementHealth();
                }
            }
            BulletCoolDown = 1f;
        }

        if (Vector3.Distance(this.transform.position, Attributes.Target.transform.position) < GameManager.instance.DistanceForEnemyToShoot && Attributes.nav.isStopped == false)
        {

            //ConvertState(CurrentState.Wander);
            Debug.Log("SHOOTING");
       
        }
        else if (Vector3.Distance(this.transform.position, Attributes.Target.transform.position) > GameManager.instance.DistanceForEnemyToShoot+3 && Attributes.nav.isStopped ==true)
        {
            ConvertState(CurrentState.ChasingPlayer,LocoMotionState.Run);
        }
    }

    public void RotateTowardsPlayer()
    {

        //Vector3 targetDirection = Attributes.Target.transform.position - transform.position;

        //// The step size is equal to speed times frame time.
        //float singleStep = 5 * Time.deltaTime;

        //// Rotate the forward vector towards the target direction by one step
        //Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        //// Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection, Color.red);


        //transform.rotation = Quaternion.LookRotation(newDirection);


        var lookPos = Attributes.Target.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }
    private void Update()
    {
        if (Attributes.Health>0 && Attributes.State != CurrentState.Dead && Manager.instance.CurrentGameState==Manager.GameState.Running)
        {
            if(BulletCoolDown>0)
            {
                BulletCoolDown -= Time.deltaTime;
            }

            if (Attributes.Type == CharacterType.Enemy)
                {
                    if (Attributes.State == CurrentState.Wander )
                        Wandering();
                }
        
           
            if (Attributes.Type == CharacterType.Enemy)
            {
                if (CollisionCoolDown > 0)
                    CollisionCoolDown -= Time.deltaTime;



                if (Attributes.State == CurrentState.ChasingPlayer)          
                    FollowPlayer();

                if (Attributes.State == CurrentState.CheckingLastPosition)
                    FollowLastPosition();

                if (Attributes.State == CurrentState.KillingPlayer)

                    ShootingPlayer();
            }
           


           
        }

    }
    //public void SetRandomPosition(bool isReversedDirection = false)
    //{

    //    RandomLocation = Attributes.GetRandomLocation(this.transform.position, 100, 1, 20, isReversedDirection);
    //}

    bool isTargetReachAble()
    {
        if (Attributes.Target == null)
        {
            Attributes.Target = GameObject.FindGameObjectWithTag("Player");
        }
        NavMeshPath navMeshPath = new NavMeshPath();
        navMeshPath.ClearCorners();

        Attributes.nav.CalculatePath(Attributes.Target.transform.position, navMeshPath);
        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            return true;
        else
            return false;
    }
    bool CalculateNewPath()
    {
        NavMeshPath navMeshPath = new NavMeshPath();
   

        Attributes.nav.CalculatePath(RandomLocation, navMeshPath);
       

        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
       //  print("PTAH FAILED:"+name+RandomLocation);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetWanderPosition(RandomDirectionAttribute Direction=RandomDirectionAttribute.Random)
    {

        Direction = RandomDirectionAttribute.Random;

        int counter = 0;
        do
        {
            RandomLocation = Attributes.GetLocation(this.transform, 50, 10, 10, Direction);

            counter++;
         
        } while (isNavStarted == true && CalculateNewPath() == false && counter < 100000);


        //LastPositionGameObject = Instantiate(Manager.instance.LastPositionObject);
        //LastPositionGameObject.transform.position = RandomLocation;
    }

    public CurrentState GetState()
    {
        return Attributes.State;
    }
    public StateMachine GetCharacterBeingFollowed()
    {
        return Attributes.Target.GetComponent<StateMachine>();
    }


    private void OnCollisionStay(Collision other)
    {

        Debug.Log("COLLIDED LAYER:" + other.gameObject.layer);
        if (Attributes.Type == CharacterType.Enemy && CollisionCoolDown <= 0)
        {
           
            if (Attributes.State == CurrentState.Wander )
            {
                if (other.gameObject.layer == 10)
                {
                    CollisionCoolDown = 1.5f;
                    Debug.Log("NAME:"+other.gameObject.name);
                    SetWanderPosition(RandomDirectionAttribute.ReverseCurrentDirection);
                }
            }
        }



    }
    //private void OnCollisionStay(Collision other)
    //{

    //    //if (Attributes.Type == CharacterType.Bot)
    //    //{
    //    //    //if (Attributes.State == CurrentState.Wander || Attributes.State == CurrentState.BeingChased)
    //    //    //{
    //    //    if (other.gameObject.tag == "Wall")
    //    //    {
    //    //        SetWanderPosition(RandomDirectionAttribute.Random);
    //    //    }

    //    //}



    //    }

    private void OnTriggerStay(Collider other)
    {
    

     
    }

    
}
