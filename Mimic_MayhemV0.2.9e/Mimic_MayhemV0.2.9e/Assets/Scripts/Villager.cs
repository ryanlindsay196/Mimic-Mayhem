using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    //Villager State Machine
    enum VillagerState
    {
        None,
        Wander,
        Suspicious,
        Alert,
        CalmDown,
        Flee,
        CallForHelp,
        Eaten
    }
    VillagerState thisVillagerState = VillagerState.None;

    //Variables for base Behavior
    public Transform player;
    public float maxAngle;
    public float maxRadius;
    private bool isInFOV = false;
    public Transform navDestination;
    NavMeshAgent navMeshAgent;
    int currentPatrolIndex;
    float waitTimer;
    bool traveling; //Turn the following into a switch later on
    bool waiting; // ^^^^^^
    bool patrolForward; // ^^^^^^^
    GameObject mimic;
    bool playerHit = false;
    Vector3 newPos;


    //Other Scripts
    Listener listener;
    Timers timers;
    PlayerMovement playerMov;
    Animator mimicAnim;
    Animator animator;


    //Is this NPC waiting at a WayPoint
    [SerializeField]
    bool patrolWaiting;
    //How long are they waiting?
    [SerializeField]
    float totalWaitTime = 3.0f;
    //Probability of switching Directions mid-Patrol
    [SerializeField]
    float switchProbability = 0.2f;
    //List of All Possible WayPoints to Visit
    [SerializeField]
    List<WayPoint> patrolWayPoints;
    [SerializeField]
    float villSpeed;

   


            
    // Use this for initialization
    void Start ()
    {
        listener =gameObject.GetComponent<Listener>();
        timers = gameObject.GetComponent<Timers>();
        animator = gameObject.GetComponent<Animator>();


        mimic = GameObject.FindGameObjectWithTag("Player");
        playerMov = mimic.GetComponent<PlayerMovement>();
        mimicAnim = mimic.GetComponent<Animator>();


        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            if(patrolWayPoints != null && patrolWayPoints.Count >= 2)
            {
                currentPatrolIndex = 0;
                SetDestination();
            }
            else
            {
                Debug.Log("Insuffiecient Patrol Waypoints for patrol behavior");
            }
        }
       
        SetVillagerState(VillagerState.Wander);

    }

    // Update is called once per frame
    void Update ()
    {
        
        isInFOV = InFOV(transform, player, maxAngle, maxRadius);
        timers.currentTimer += Time.deltaTime;
        villSpeed = navMeshAgent.speed;
        
        if (thisVillagerState != VillagerState.None)
        {
            //Wander Behavior Working while VillagerState.Wander
            if (thisVillagerState == VillagerState.Wander) 
            {
                //Check if we're close to the destination
                if (traveling && navMeshAgent.remainingDistance <= 1.0f)
                {
                    traveling = false;
                    //If we're going to wait, then wait
                    if (patrolWaiting)
                    {
                        waiting = true;
                        waitTimer = 0f;
                    }
                    else
                    {
                        ChangePatrolPoint();
                        SetDestination();
                    }
                }
                //Instead if we're waiting
                if (waiting)
                {
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= totalWaitTime)
                    {
                        waiting = false;
                        SetDestination();
                        ChangePatrolPoint();
                    }
                }

            }

            if (thisVillagerState == VillagerState.Flee)
            {
                float farthestWay = 0;

                WayPoint fleePoint = null;

                foreach (WayPoint patrolWaypoint in patrolWayPoints)
                {
                    float currentWayDis = (patrolWaypoint.transform.position - mimic.transform.position).magnitude;

                    if (currentWayDis > farthestWay)
                    {
                        fleePoint = patrolWaypoint;
                        farthestWay = currentWayDis;
                    }
                }
                if (fleePoint != null)
                {
                    newPos = fleePoint.transform.position;
                }

                if (timers.currentTimer >= timers.lookBackTimer && isInFOV == true)
                {
                    navMeshAgent.SetDestination(newPos);
                    timers.currentTimer = 0;

                    if (timers.currentTimer >= timers.fleeTimer && isInFOV == true)
                    {
                        timers.currentTimer = 0;
                    }
                }
            }
            /*else if (thisVillagerState == VillagerState.Suspicious)
            {
                timers.SusTimer();
            }
            else if(thisVillagerState==VillagerState.Alert)
            {
                timers.AlerTimer();
            }
            else if(thisVillagerState==VillagerState.Flee)
            {
                timers.FleTimer();
                timers.LBTimer();
                 
            }*/
        }
        else
        {
            Debug.Log("An Error has Occurred within the Villager State Machine");
        }
        VillagerStateMachine();




    }
        
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isInFOV)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }
        
    //checks if player is in detection sphere, then FOV cone
    public static bool InFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overLaps = new Collider[20];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overLaps);

        for (int i = 0; i < count + 1; i++)
        {
            if(overLaps[i] != null)
            {
                if(overLaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if(angle <= maxAngle)
                    {
                        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray,out hit, maxRadius))
                        {
                            if (hit.transform == target)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    //In earshot
    public static bool InEarShot (Transform checkingOjbect, Transform target, float maxRadius)
    {

        return false;
    }

    //State Machine for the Villager 
    void SetVillagerState(VillagerState newState)
    {
        if (newState != thisVillagerState)
        {
            timers.currentTimer = 0.0f;
            playerHit = false;

            thisVillagerState = newState;
            switch (thisVillagerState)
            {
                case VillagerState.Wander:
                    Debug.Log("Villager State:" + thisVillagerState);
                    navMeshAgent.speed = 3.5f;
                    break;
                case VillagerState.Suspicious:
                    Debug.Log("Villager State:" + thisVillagerState);
                    //start animation

                    break;
                case VillagerState.Alert:
                    Debug.Log("Villager State:" + thisVillagerState);
                    break;
                case VillagerState.Flee:
                    Debug.Log("Villager State:" + thisVillagerState);
                    navMeshAgent.speed = 10;
                    break;
                case VillagerState.CalmDown:
                    Debug.Log("Villager State:" + thisVillagerState);
                    break;
                case VillagerState.Eaten:
                    Debug.Log("Villager State:" + thisVillagerState);
                    Destroy(gameObject);
                    break;
            }
        }
    }



    //Call This to Change the State
    void VillagerStateMachine()
    {
        bool amIEaten = false;
        Eatable eatable = GetComponent<Eatable>();
        if(eatable != null)
        {
            amIEaten = eatable.IsEaten;
        }

        switch (thisVillagerState)
        {
            case VillagerState.Wander:
                if (amIEaten)
                {
                    animator.SetBool("isWalking", false);
                    SetVillagerState(VillagerState.Eaten);
                }
                else if (isInFOV == true || playerHit==true )
                {
                    mimicAnim.SetBool("Seen", true);
                    animator.SetBool("isWalking", false);
                    SetVillagerState(VillagerState.Alert);
                }
                else if(listener.inEarShot==true){

                    animator.SetBool("isWalking", false);
                    SetVillagerState(VillagerState.Suspicious);
                }

                //else if player activates eat mechanic and villager is within radius set to Eaten 

                //else if guard callsout mimic set to Flee

                break;
            
            case VillagerState.Suspicious:
                if (amIEaten)
                {
                   
                    SetVillagerState(VillagerState.Eaten);
                }
                else if (isInFOV == true)
                {
                    mimicAnim.SetBool("Seen", true);
                    
                    SetVillagerState(VillagerState.Alert);
                }
                else if(timers.currentTimer >= timers.suspiciousTimer){
                    timers.currentTimer = 0;
                    animator.SetBool("isCalmingDown", true);
                    SetVillagerState(VillagerState.CalmDown);
                }
                else if(playerHit==true)
                {

                    SetVillagerState(VillagerState.Alert);
                }


                //else if player activates eat mechanic and villager is within radius set to Eaten


                //else if guard callsout mimic set to Flee


                break;
            case VillagerState.Alert:
                if (amIEaten)
                {
                    mimicAnim.SetBool("Seen", false);
                    SetVillagerState(VillagerState.Eaten);
                }
                else if (isInFOV == false)
                {
                    if (listener.inEarShot == true)
                    {
                        mimicAnim.SetBool("Seen", false);
                        SetVillagerState(VillagerState.Suspicious);
                    }
                    else if(playerHit==true)
                    {
                        mimicAnim.SetBool("Seen", false);
                        animator.SetBool("isRunning", true);
                        SetVillagerState(VillagerState.Flee);
                    }
                    else
                    {
                        mimicAnim.SetBool("Seen", false);
                        animator.SetBool("isCalmingDown", true);
                        timers.currentTimer = 0;
                        SetVillagerState(VillagerState.CalmDown);
                    }
                }
                //else if(timers.alerTimerCheck==true){
                else if (timers.currentTimer >= timers.alertTimer)
                {
                    animator.SetBool("isRunning", true);
                    SetVillagerState(VillagerState.Flee);
                }
               

                //else if player activates eat mechanic and villager is within radius set to Eaten


                //else if guard callsout mimic set to Flee



                break;
            case VillagerState.Flee:
                if (amIEaten)
                {
                    animator.SetBool("isRunning", false);
                    SetVillagerState(VillagerState.Eaten);
                }
                else if(timers.currentTimer >= timers.lookBackTimer && isInFOV != true){
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isCalmingDown", true);
                    SetVillagerState(VillagerState.CalmDown);
                }
                else if(timers.currentTimer >= timers.fleeTimer)
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isCalmingDown", true);
                    SetVillagerState(VillagerState.CalmDown);
                }


                //else if player activates eat mechanic and villager is within radius set to Eaten

                break;
            case VillagerState.CalmDown:

                //else if player activates eat mechanic and villager is within radius set to Eaten

                //else if guard callsout mimic set to Flee
                if (amIEaten)
                {
                    animator.SetBool("isCalmingDown", false);
                    SetVillagerState(VillagerState.Eaten);
                }

                else if (isInFOV == true || playerHit==true)
                {
                    mimicAnim.SetBool("Seen", true);
                    animator.SetBool("isCalmingDown", false);
                    SetVillagerState(VillagerState.Alert);

                }
                else if(listener.inEarShot==true){
                    animator.SetBool("isCalmingDown", false);
                    SetVillagerState(VillagerState.Suspicious);
                }
                else if(timers.currentTimer >= timers.calmdownTimer){
                    animator.SetBool("isCalmingDown", false);
                    animator.SetBool("isWalking", true);
                    SetVillagerState(VillagerState.Wander);
                }
               
                break;


            case VillagerState.Eaten:
                

                break;
        }
    }

    //
    private void SetDestination()
    {
        if(patrolWayPoints !=null)
        {
            Vector3 targetVector = patrolWayPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination(targetVector);
            traveling = true;
        }
    }
    //
    private void ChangePatrolPoint()
    {
        if (UnityEngine.Random.Range(0f,1f) <= switchProbability)
        {
            patrolForward = !patrolForward;
        }
        if (patrolForward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolWayPoints.Count;
        }
        else
        {
            if(--currentPatrolIndex < 0)
            {
                currentPatrolIndex = patrolWayPoints.Count - 1;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Player")
        {
            playerHit = true;
        }
        else
        {
            playerHit = false;
        }
    }



}
