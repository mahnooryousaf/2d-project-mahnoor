using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Manager : MonoBehaviour
{

    private GameObject[] Enemies;

    public enum GameState
    {
        Running,Paused,PlayerHasWon_Running, BotHasWon_Running, Completed,PlayerHasDied,MiniGameRunning
    }
    public GameState CurrentGameState=GameState.Running;
    public static Manager instance;
    public GameObject Player;
    public NavMeshTriangulation Triangulation;

    

 

   

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }

       
      

    }
  

    public  List<StateMachine> AllCharacters;

 

    private void Start()
    {
       

        Triangulation = NavMesh.CalculateTriangulation();

    }
    public GameObject LastPositionObject;
   
   
    public void DoWanderAI()
    {
        for (int i = 0; i < AllCharacters.Count; i++)
        {
            
        }
    }
    
    private void Update()
    {
        if(Input.GetKey(KeyCode.R))
           
        {
            SetEnemyState(CurrentState.ChasingPlayer, LocoMotionState.Run);

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void DelayLevelFinished()
    {
       

        Invoke("LevelCompleted", 4f);
    }

   
    
    private void OnDisable()
    {
        Time.timeScale = 1;
    }        
    public void SetGameState(GameState state)
    {
        if (CurrentGameState == GameState.BotHasWon_Running || CurrentGameState == GameState.PlayerHasWon_Running)
            return;
        CurrentGameState = state;
        if (state == GameState.PlayerHasDied)
            DelayLevelFinished();

        if(state==GameState.PlayerHasWon_Running || state == GameState.BotHasWon_Running)
        {
            for (int i = 0; i < AllCharacters.Count; i++)
            {
                if (AllCharacters[i] != null)
                {
                 

                    if (AllCharacters[i].Attributes.Type == CharacterType.Enemy)
                    {
                        AllCharacters[i].ToogleDetectors(false);
                        }
                }
                   
              
            }
            if(state == GameState.PlayerHasWon_Running)
            {
              
            }
            
        }
    }

    
   
    public AnimationClip FindAnimation(Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }
    public void SetCharacterSpeed(StateMachine CHARACTER)

    {
        //if (CHARACTER.Attributes.Type == CharacterType.Enemy)
        //{
        //    CHARACTER.Attributes.WalkSpeed = 50;
        //    CHARACTER.Attributes.RunSpeed = 50;
        //    return;
        //}

        
        if (CHARACTER.Attributes.Type==CharacterType.Enemy)
        {
            CHARACTER.Attributes.WalkSpeed = 0.7f;
            CHARACTER.Attributes.RunSpeed = 1f;

            //if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 1|| PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 0 )
            //{
            //     CHARACTER.Attributes.WalkSpeed =3.5f;
            //     CHARACTER.Attributes.RunSpeed = 4.5f;

            //    //AnimationClip anim = FindAnimation(CHARACTER.Attributes.Anim, "Running");

            //    //CHARACTER.Attributes.Anim.speed = 0.65f;

            //}
            //else if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 2)
            //{
            //    CHARACTER.Attributes.WalkSpeed =3.5f;
            //    CHARACTER.Attributes.RunSpeed = 4.5f;
            //}
            //else
            //{
            //    CHARACTER.Attributes.WalkSpeed = 3.5f;
            //    CHARACTER.Attributes.RunSpeed = 4.5f;
            //}
            //CHARACTER.Attributes.Anim.SetFloat("WalkingSpeed", (float)0.186 * CHARACTER.Attributes.WalkSpeed);
            //CHARACTER.Attributes.Anim.SetFloat("RunningSpeed", (float)0.100 * CHARACTER.Attributes.RunSpeed);
        }



    }
    public void SetRange(Detector RangeDetector)
    {
        if (RangeDetector.TypeOfCharacter == CharacterType.Enemy)
        {
            RangeDetector.range = 50;
            //if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 0)
            //{
            //    RangeDetector.gameObject.SetActive(false);
            //}
            //else if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 1)
            //{
            //    RangeDetector.range = 10;
   
            //}
            //else if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 2)
            //{
            //    RangeDetector.range = 25;
            //}
            //else
            //{
            //    RangeDetector.range = 50;
            //}
        }
    }

    public void GetFieldOfView(FieldOfView fov)
    {
        
            if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 0)
            {
               fov.gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 1)
            {
                fov.viewRadius =5.8f;
                 fov.viewAngle = 24f;

            }
            else if (PlayerPrefs.GetInt("ENEMYDIFFICULTY", 1) == 2)
            {
                fov.viewRadius = 9.04f;
            fov.viewAngle = 76f;
            }
            else
            {
            fov.viewRadius = 12.87f;
            fov.viewAngle = 99f;
        }
        
    }


    public void SetEnemyState(CurrentState c,LocoMotionState l)
    {
        for (int i = 0; i < AllCharacters.Count; i++)
        {

            if (AllCharacters[i] != null)
            {
                AllCharacters[i].ConvertState(c, l);
            }
        }
    }
}
