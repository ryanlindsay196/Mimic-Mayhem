using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //player state machine
    enum PlayerState
    {
        None,
        Idle,
        Walk,
        Run,
        Hide,
        Fidget,
        Death,
        Eating

    }

    //Other Scripts
    ItemInventory itemInventory;
    Animator animator;

    PlayerState myPlayerState = PlayerState.None;
    public bool inEatingDistance = false;
    public bool villagerDeath = false;

    //Referencing SoundFX
    public GameObject soundFX;

    //Rigidbody myRigidbody;
    private float SpeedPerSecond;
    Vector3 desiredDirection = new Vector3();
    public float RotationSpeed = 0.25f;
    public float walkSpeed;
    public float runSpeed;

    
    // Use this for initialization
    void Start()
    {
        //myRigidbody = GetComponent<Rigidbody>();
        SetPlayerState(PlayerState.Idle);
        itemInventory = gameObject.GetComponent<ItemInventory>();
        animator = gameObject.GetComponent<Animator>();


        //InvokeRepeating("SpawnSoundEvent", 0.0f, 0.5f);
    }

    void FixedUpdate()
    {


        desiredDirection.x = Input.GetAxis("Horizontal");
        desiredDirection.z = Input.GetAxis("Vertical");

        transform.position += desiredDirection * SpeedPerSecond * Time.deltaTime;


        Vector3 movement = new Vector3(desiredDirection.x, 0.0f, desiredDirection.z);
        if (movement != Vector3.zero)
        {

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), RotationSpeed);
        }
           
        StandardPlayerStateChangeCode();

    }

    private void Update()
    {
        //SpawnSoundEvent();

    }
    // Update is called once per frame

    //Player/Mimic State Machine: Allows for State to change
    void StandardPlayerStateChangeCode()
    {
                
        switch (myPlayerState)
        {
            case PlayerState.Idle:
                if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0)
                {
                    animator.SetBool("isWalking", true);
                    SetPlayerState(PlayerState.Walk);
                }
                else if(Input.GetKey(KeyCode.K))
                {
                    SetPlayerState(PlayerState.Death);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    villagerDeath = true;
                    Debug.Log(villagerDeath);
                    SetPlayerState(PlayerState.Eating);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("itemEaten");
                    SetPlayerState(PlayerState.Eating);
                }

                break;
            case PlayerState.Walk:
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isWalking", false);
                    SetPlayerState(PlayerState.Run);
                }
                else if (!(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0))
                {
                    animator.SetBool("isWalking", false);
                    SetPlayerState(PlayerState.Idle);
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    animator.SetBool("isWalking", false);
                    SetPlayerState(PlayerState.Eating);
                }
                break;
            case PlayerState.Run:
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                    SetPlayerState(PlayerState.Walk);

                }
                else if (Input.GetKey(KeyCode.E))
                {
                    animator.SetBool("isRunning", false);
                    SetPlayerState(PlayerState.Eating);
                }
                break;
            case PlayerState.Hide:
                {
                    
                }
                break;
            case PlayerState.Death:
                {
                   
                }
                break;
            case PlayerState.Eating:
                {
                    if(!Input.GetKeyDown(KeyCode.E)){
                        villagerDeath = false;
                        SetPlayerState(PlayerState.Idle);
                    }
                }
                break;
            case PlayerState.Fidget:
                {
                    
                }
                break;
        }
    }
    void SetPlayerState(PlayerState newState)
    {
                
        if (newState != myPlayerState)
        {

            switch (myPlayerState)
            {
                case PlayerState.Run:
                    CancelInvoke("SpawnSoundEvent");
                    break;
            }

            myPlayerState = newState;
            switch (myPlayerState)
            {
                case PlayerState.Idle:
                    //Debug.Log(myPlayerState);

                    break;

                case PlayerState.Walk:
                    //Debug.Log(myPlayerState);

                    SpeedPerSecond = walkSpeed;

                    break;

                case PlayerState.Run:
                    //Debug.Log(myPlayerState);

                    InvokeRepeating("SpawnSoundEvent", 0.0f, 0.5f);

                    SpeedPerSecond = runSpeed;

                    break;

                case PlayerState.Hide:
                    break;

                case PlayerState.Death:
                    Destroy(gameObject);

                    break;

                case PlayerState.Eating:
                    //player eating animation

                    Eatable currentTarget = null;
                    float distanceToClosestTarget = float.MaxValue;

                    Eatable[] eatables = FindObjectsOfType<Eatable>();
                    foreach (Eatable eatable in eatables)
                    {
                        float distanceToTarget = (eatable.transform.position - transform.position).magnitude;
                        if ( distanceToTarget < 2.0f)
                        {
                            if(distanceToTarget < distanceToClosestTarget)
                            {
                                currentTarget = eatable;
                                distanceToClosestTarget = distanceToTarget;
                            }
                        }
                    }

                    if(currentTarget != null)
                    {
                        currentTarget.IsEaten = true;
                    }

                    break;

                case PlayerState.Fidget:
                    
                    break;

            }
        }
    }

    void SpawnSoundEvent()//right now this is mostly just for prototypeing purposes. We'll have these events Instantiating in the animator
    {
        Instantiate(soundFX, transform.position, Quaternion.identity);
    }


    void OnTriggerEnter( Collider other)
    {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        /*if (other.GetComponent<Eatable>() == true)
        {
            inEatingDistance = true;
            Debug.Log("inEatingDistance=" + inEatingDistance);
        }
        else
        {
            inEatingDistance = false;
            Debug.Log("inEatingDistance=" + inEatingDistance);
        }*/
    }

    public void ReceiveItem(ItemBase item)
    {
        ItemBase clone = item.Clone();
        itemInventory.AddItem(clone);
    }
}