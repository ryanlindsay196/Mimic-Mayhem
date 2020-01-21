using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerMain : MonoBehaviour
{

    [SerializeField]
    float runSpeed, walkSpeed;

    public MimicMain currentlyDetectedPlayer;
    //Villager State Machine
    public enum VillagerState
    {
        None,
        Wander,
        Suspicious,
        Alert,
        CalmDown,
        Flee,
        CallForHelp,
        Eaten,
        Hide,
        Chase//GUARD ONLY
    }
    VillagerState thisVillagerState = VillagerState.None;

    public bool endOfGame;//set to true from UITimerCountdown

    //Variables for base Behavior
    MimicMain[] player;
    public float maxAngle;
    public float maxRadius;
    private bool isInFOV = false;
    public Transform navDestination;
    protected NavMeshAgent navMeshAgent;
    protected int currentPatrolIndex;
    protected float waitTimer;
    protected bool traveling; //Turn the following into a switch later on
    protected bool waiting; // ^^^^^^
    bool patrolForward; // ^^^^^^^
    GameObject mimic;
    bool playerHit = false;
    Vector3 newPos;

    //Calling g_PlaySFX -- Using the child gameobject to reference the script
    public g_PlaySFX guardSFX;

    //Other Scripts
    Listener listener;
    protected Timers timers;
    PlayerMovement playerMov;
    Animator mimicAnim;
    Animator animator;


    //Is this NPC waiting at a WayPoint
    [SerializeField]
    protected bool patrolWaiting;
    //How long are they waiting?
    [SerializeField]
    protected float totalWaitTime = 3.0f;
    //Probability of switching Directions mid-Patrol
    [SerializeField]
    float switchProbability = 0.2f;
    //List of All Possible WayPoints to Visit
    [SerializeField]
    protected List<WayPoint> patrolWayPoints;
    [SerializeField]
    float villSpeed;

    //float villagerAlertTimer;//when the villager is alerted, set to zero and increment until reaching villagerAlertMaxTime. Afterwards, SetVillagerState(flee)
    //[SerializeField]
    //float villagerAlertMaxTime;//how long a villager is alerted before fleeing or becoming suspicious again


    VillagerUIManager ui;

    public VillagerState GetVillagerState
    {
        get { return thisVillagerState; }
    }

    // Use this for initialization
    void Start()
    {
        ui = GetComponent<VillagerUIManager>();

        listener = gameObject.GetComponent<Listener>();
        timers = gameObject.GetComponent<Timers>();
        animator = gameObject.GetComponent<Animator>();


        //mimic = GameObject.FindGameObjectWithTag("Player");
        //playerMov = mimic.GetComponent<PlayerMovement>();
        //mimicAnim = mimic.GetComponent<Animator>();

        player = FindObjectsOfType<MimicMain>();


        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            if (patrolWayPoints != null && patrolWayPoints.Count >= 2)
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

    bool IsStillThusUndetectable()
    {
        if (currentlyDetectedPlayer == null)
            return true;
        return (!currentlyDetectedPlayer.isDetectableWhenStill &&
                currentlyDetectedPlayer.desiredDirection == new Vector3() &&
                currentlyDetectedPlayer.myPlayerState == MimicMain.PlayerState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (!endOfGame)
        {
            MimicMain currentPlayer1 = player[0];
            MimicMain currentPlayer2 = player[1];

            currentPlayer1.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            currentPlayer2.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            //get the transform of the currently enabled player prefab for the second parameter
            isInFOV = InFOV(transform, player[0].transform, maxAngle, maxRadius) ||
                        InFOV(transform, player[1].transform, maxAngle, maxRadius);

            if (InFOV(transform, currentPlayer1.transform, maxAngle, maxRadius))
                currentlyDetectedPlayer = currentPlayer1;

            if (InFOV(transform, currentPlayer2.transform, maxAngle, maxRadius))
                currentlyDetectedPlayer = currentPlayer2;

            if (currentlyDetectedPlayer != null && IsStillThusUndetectable() && thisVillagerState == VillagerState.Wander)
            {//if can't detect player when still, and the player is still, then "undetect the player"
                currentlyDetectedPlayer = null;
            }

            timers.currentTimer += Time.deltaTime;
            villSpeed = navMeshAgent.speed;

            if (thisVillagerState != VillagerState.None)
            {
                //Wander Behavior Working while VillagerState.Wander
                if (thisVillagerState == VillagerState.Wander)
                {
                    VillagerWander();
                }

                if (thisVillagerState == VillagerState.Flee)
                {
                    VillagerFlee();
                }
                else if (thisVillagerState == VillagerState.Hide)
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
                else if (thisVillagerState == VillagerState.Chase)
                {
                    timers.ChaseTimer();

                    if (timers.chaseTimerCheck)
                    {
                        SetVillagerState(VillagerState.CalmDown);
                    }

                    VillagerChase();
                }
                else if (thisVillagerState == VillagerState.Suspicious)
                {/*
                timers.SusTimer();

                if(timers.susTimerCheck)
                {
                    if(isInFOV)
                    {
                        SetVillagerState(VillagerState.Alert);
                    }
                    else
                    {
                        SetVillagerState(VillagerState.Wander);
                    }
                }
                */
                }
                else if (thisVillagerState == VillagerState.Alert)
                {

                    timers.AlerTimer();
                    navMeshAgent.SetDestination(transform.position);

                    //transform.Rotate(transform.forward, Mathf.Acos(Vector3.Dot((currentPlayer.transform.position - transform.position).normalized)));
                    Vector3.RotateTowards(transform.position, currentlyDetectedPlayer.transform.position, 0.34f, 1f);
                    /*
                    if (timers.alerTimerCheck)
                    {
                        if (isInFOV)
                        {
                            SetVillagerState(VillagerState.Flee);
                        }
                        else
                        {
                            SetVillagerState(VillagerState.Suspicious);
                        }
                    }*/
                }
                else if (thisVillagerState == VillagerState.Flee)
                {
                    //timers.FleTimer();
                    //timers.LBTimer();

                }
            }
            else
            {
                Debug.Log("An Error has Occurred within the Villager State Machine");
            }
            VillagerStateMachine();



        }
        else
        {
            navMeshAgent.SetDestination(transform.position);
            SetVillagerState(VillagerState.None);
        }
        ChangeAnimatorState();

    }
    public virtual void ChangeAnimatorState()
    {

    }

    public virtual void VillagerChase()
    {
        SetVillagerState(VillagerState.Flee);
    }

    public virtual void VillagerFlee()
    {
        Vector3 currentPlayerPosition = currentlyDetectedPlayer.transform.position;
        navMeshAgent.SetDestination(-((currentPlayerPosition - transform.position).normalized * 3) + transform.position);
    }

    public virtual void VillagerWander()
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
        else if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= totalWaitTime)
            {
                waiting = false;
                SetDestination();
                ChangePatrolPoint();
            }
        }
        else
        {
            //SetDestination();
            //ChangePatrolPoint();
        }

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
        //The second parameter includes the currently enabled player from the player prefabs list
        //Gizmos.DrawRay(transform.position, (player[0].GetComponent<TransformSelect>().playerPrefabs[player[0].GetComponent<TransformSelect>().playerTransformationIndex].transform.position - transform.position).normalized * maxRadius);
        //Gizmos.DrawRay(transform.position, (player[1].GetComponent<TransformSelect>().playerPrefabs[player[1].GetComponent<TransformSelect>().playerTransformationIndex].transform.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    //checks if player is in detection sphere, then FOV cone
    public static bool InFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overLaps = new Collider[50];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overLaps);
        for (int i = 0; i < count + 1; i++)
        {
            if (overLaps[i] != null)
            {
                if (overLaps[i].gameObject.transform.position == target.position)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                            {
                                //Debug.Log("InFov = true");
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    //In earshot//DUD?
    public static bool InEarShot(Transform checkingOjbect, Transform target, float maxRadius)
    {//DUD?

        return false;
    }

    //State Machine for the Villager 
    protected void SetVillagerState(VillagerState newState)
    {
        if (newState != thisVillagerState)
        {
            #region reset timers
            timers.currentTimer = 0.0f;

            timers.calmdownTimerCheck = false;
            timers.alerTimerCheck = false;
            timers.fleeCoolTimerCheck = false;
            timers.fleeTimerCheck = false;
            timers.lbTimerCheck = false;
            timers.susTimerCheck = false;
            timers.chaseTimerCheck = false;
            #endregion
            playerHit = false;

            listener.inEarShot = false;


            //Debug.Log("Villager State:" + thisVillagerState);

            thisVillagerState = newState;
            switch (thisVillagerState)
            {
                case VillagerState.Wander:
                    navMeshAgent.speed = walkSpeed;
                    ui.ChangeUISprite(-1);
                    break;
                case VillagerState.Suspicious:
                    ui.ChangeUISprite(1);
                    //start animation
                    //Calling G_PLAYSFX
                    guardSFX.G_Suspicious();

                    break;
                case VillagerState.Alert:
                    //villagerAlertTimer = 0;
                    ui.ChangeUISprite(1);
                    break;
                case VillagerState.Chase:
                    ui.ChangeUISprite(2);
                    guardSFX.G_Alert();
                    break;
                case VillagerState.Flee:
                    navMeshAgent.speed = runSpeed;
                    ui.ChangeUISprite(2);
                    break;
                case VillagerState.Hide:
                    ChangePatrolPoint();
                    SetDestination();

                    break;
                case VillagerState.CalmDown:
                    ui.ChangeUISprite(-1);
                    break;
                case VillagerState.Eaten:
                    ui.ChangeUISprite(-1);
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
        if (eatable != null)
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
                else if ((isInFOV == true || playerHit == true) && !IsStillThusUndetectable())
                {
                    //mimicAnim.SetBool("Seen", true);
                    animator.SetBool("isWalking", false);
                    SetVillagerState(VillagerState.Alert);
                }
                else if (listener.inEarShot)
                {

                    animator.SetBool("isWalking", false);
                    SetVillagerState(VillagerState.Suspicious);
                    navMeshAgent.SetDestination(listener.lastSoundHeardPosition);
                }

                //else if player activates eat mechanic and villager is within radius set to Eaten 

                //else if guard callsout mimic set to Flee

                break;

            case VillagerState.Suspicious:
                if (amIEaten)
                {

                    SetVillagerState(VillagerState.Eaten);
                }
                else if ((isInFOV || playerHit) && !IsStillThusUndetectable())
                {
                    //mimicAnim.SetBool("Seen", true);

                    SetVillagerState(VillagerState.Alert);
                    //1st argument is the currently activated player's position
                    SetNavMeshOnSuspicious(player[0].transform.position);
                    SetNavMeshOnSuspicious(player[1].transform.position);
                }
                else if (timers.currentTimer >= timers.suspiciousTimer)
                {
                    timers.currentTimer = 0;
                    animator.SetBool("isCalmingDown", true);
                    SetVillagerState(VillagerState.CalmDown);
                }


                //else if player activates eat mechanic and villager is within radius set to Eaten


                //else if guard callsout mimic set to Flee


                break;
            case VillagerState.Alert:
                if (amIEaten)
                {
                    //mimicAnim.SetBool("Seen", false);
                    SetVillagerState(VillagerState.Eaten);
                }
                else if (isInFOV || playerHit)
                {
                    if (listener.inEarShot)
                    {
                        //mimicAnim.SetBool("Seen", false);
                        //SetVillagerState(VillagerState.Suspicious);
                    }
                    if (playerHit == true || timers.alerTimerCheck)
                    //if()
                    {
                        //mimicAnim.SetBool("Seen", false);
                        animator.SetBool("isRunning", true);
                        SetVillagerState(VillagerState.Flee);
                    }
                    else
                    {
                       // mimicAnim.SetBool("Seen", false);
                    //    animator.SetBool("isCalmingDown", true);
                        //timers.currentTimer = 0;
                        //SetVillagerState(VillagerState.Wander);
                    }
                }
                //else if(timers.alerTimerCheck==true){
                else if (timers.currentTimer >= timers.alertTimer)
                {
                    animator.SetBool("isRunning", false);
                    SetVillagerState(VillagerState.Wander);
                }


                //else if player activates eat mechanic and villager is within radius set to Eaten


                //else if guard callsout mimic set to Flee



                break;
            case VillagerState.Flee:
                if (amIEaten)
                {
                    animator.SetBool("isRunning", true);
                    SetVillagerState(VillagerState.Eaten);
                }
                else if (timers.currentTimer >= timers.lookBackTimer && !isInFOV)
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isCalmingDown", true);
                    SetVillagerState(VillagerState.Hide);
                }
                else if (timers.currentTimer >= timers.fleeTimer)
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isCalmingDown", true);
                    SetVillagerState(VillagerState.CalmDown);
                }


                //else if player activates eat mechanic and villager is within radius set to Eaten

                break;
            case VillagerState.Hide:
                animator.SetBool("isRunning", true);
                if(timers.currentTimer >= timers.hideTimer)
                {
                    SetVillagerState(VillagerState.CalmDown);
                }
                break;
            case VillagerState.CalmDown:

                //else if player activates eat mechanic and villager is within radius set to Eaten

                //else if guard callsout mimic set to Flee
                if (amIEaten)
                {
                    animator.SetBool("isCalmingDown", false);
                    SetVillagerState(VillagerState.Eaten);
                }

                else if ((isInFOV == true || playerHit == true) && !IsStillThusUndetectable())
                {
                    //mimicAnim.SetBool("Seen", true);
                    animator.SetBool("isCalmingDown", false);
                    SetVillagerState(VillagerState.Alert);

                }
                else if (listener.inEarShot == true)
                {
                    animator.SetBool("isCalmingDown", false);
                    SetVillagerState(VillagerState.Suspicious);
                }
                else if (timers.currentTimer >= timers.calmdownTimer)
                {
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
    protected void SetDestination()
    {
        if (patrolWayPoints != null)
        {
            Vector3 targetVector = patrolWayPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination(targetVector);
            traveling = true;
        }
    }
    //
    protected void ChangePatrolPoint()
    {
        if (UnityEngine.Random.Range(0f, 1f) <= switchProbability)
        {
            patrolForward = !patrolForward;
        }
        if (patrolForward)
        {
            //currentPatrolIndex = (currentPatrolIndex + 1) % patrolWayPoints.Count;
            currentPatrolIndex = (int)Mathf.Round(UnityEngine.Random.Range(0f, (int)patrolWayPoints.Count));
        }
        else
        {
            if (--currentPatrolIndex < 0)
            {
                currentPatrolIndex = patrolWayPoints.Count - 1;
            }
        }
    }

    public virtual void SetNavMeshOnSuspicious(Vector3 inPosition)
    {
        navMeshAgent.SetDestination(inPosition);
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
            //playerHit = false;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerHit = false;
        }
    }



}
