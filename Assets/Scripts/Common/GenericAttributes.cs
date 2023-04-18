using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public enum CharacterType
{
    Enemy
}
public enum CurrentState
{
    Idle, Wander,GoingForTask,  PerformingTask, Dead, ChasingPlayer, CheckingLastPosition,KillingPlayer
}
public enum LocoMotionState
{
    Idle,Walk,Run, Interect,Jump,Crouch,Dead,Shooting
}
public enum RandomDirectionAttribute
{
    TowardsCurrentDirection,ReverseCurrentDirection,Random
}
[Serializable]
public class States
{
    public float Health;

    public float WalkSpeed;
    public float RunSpeed;
    public CharacterType Type;
    public CurrentState  State;
    public CurrentState  PreviousState;
    public LocoMotionState CurrentMovementState;

    public GameObject Target;

    public Vector3 TargetLastPosition;    
    [HideInInspector]
    public Animator Anim;    
    public int CurrentTask = 0;    
    [HideInInspector]
    public NavMeshAgent nav;
    public float StateChangeDelay=0;
    public string Name;
    public double AutoStateChangeDelay = 0; 
    public void SetTarget(Vector3 obj,GameObject Ch)
    {
        Target = Ch;
        TargetLastPosition = obj;
    }



    private Vector3 GetRandomGameBoardLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;

        // pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        int secondVertexSelected = UnityEngine.Random.Range(0, maxIndices);

        // spawn on verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];

        // eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x || (int)firstVertexPosition.z == (int)secondVertexPosition.z)
        {
            point = GetRandomGameBoardLocation(); // re-roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // select a random point on it
            point = Vector3.Lerp(firstVertexPosition, secondVertexPosition, UnityEngine.Random.Range(0.05f, 0.95f));
        }

        return point;
    }



    public void GetLocation()
    {

    }
    NavMeshHit navHit;
    public Vector3 GetLocation(Transform origin, float distance, int layermask, float minimumnewdistance = 10, RandomDirectionAttribute Direction = RandomDirectionAttribute.Random)
    {

        //   int VertexIndex = UnityEngine.Random.Range(0, Manager.instance.Triangulation.vertices.Length);



 

        int trylimites = 1000;
        int locationtrylimits = 100;
        int currenttry = 0;
       
        do
        {
            float radius = UnityEngine.Random.RandomRange(10, 70);
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += origin.position;
            locationtrylimits = 1000;
            bool res = false;

            NavMeshQueryFilter filter = new NavMeshQueryFilter();
            filter.areaMask = NavMesh.GetAreaFromName("Ground");

            while (res==false &&locationtrylimits>0)
            {
             //   if (NavMesh.SamplePosition(randomDirection, out navHit, radius, 1, filter))
                if (NavMesh.SamplePosition(randomDirection, out navHit, radius,  filter))
                {
                    res = true;
                    if (Direction != RandomDirectionAttribute.Random)
                    {

                        Vector3 dirFromAtoB = (navHit.position - origin.transform.position).normalized;
                        float dotProd = Vector3.Dot(dirFromAtoB, origin.transform.forward);
                        // Debug.Log(dotProd);
                        if (Direction == RandomDirectionAttribute.TowardsCurrentDirection)
                        {
                            if (dotProd > 0.9f)
                            {
                                Direction = RandomDirectionAttribute.Random;
                            }
                            else
                            {
                                currenttry++;
                                if (currenttry >= trylimites)
                                {

                                    Direction = RandomDirectionAttribute.Random;
                                }
                            }
                        }
                        else
                        {


                            if (dotProd < 0.5f)
                            {
                                // ObjA is looking mostly opposite to  ObjB
                                Direction = RandomDirectionAttribute.Random;
                            }
                            else
                            {
                                currenttry++;
                                if (currenttry >= trylimites)
                                {
                                    Debug.Log("IGNORED");
                                    Direction = RandomDirectionAttribute.Random;
                                }
                            }
                        }
                    }
                }
                else
                {
                    locationtrylimits--;
                  
                }
            }
              if(res==false)
            {

               Debug.LogError("LOCATION GETTING FAILED. RETURNING WITH RANDOM POSITION IN ");
                navHit.position = origin.position + new Vector3(UnityEngine.Random.RandomRange(-10, 10), 0, 0);
                return navHit.position;
            }




        }
        while ((Vector3.Distance(origin.transform.position, navHit.position) < minimumnewdistance) || Direction != RandomDirectionAttribute.Random);
       // while ( Direction != RandomDirectionAttribute.Random);

        //navHit.position = new vector3(navHit.position.x,origin.transform.position.y,navHit.position.z);
        return navHit.position;
//        return finalPosition;
    }
    public double GetTaskDelay()
    {
        int lowrange = 0;
        int highrange =0;
        if(PlayerPrefs.GetInt("BOTDIFFICULTY",1) ==1)
                {
                    lowrange = 20;
                    highrange = 40;
                }
        else if (PlayerPrefs.GetInt("BOTDIFFICULTY", 1) == 2)
                {
                    lowrange = 10;
                    highrange = 20;
                }
        else{
                    lowrange = 5;
                    highrange = 10;
                }
        var random = new System.Random();
        var rDouble = random.NextDouble();
        var rRangeDouble = rDouble * (highrange - lowrange) + lowrange;       
        return rRangeDouble;       
    }        
}
