using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MimicMain : MonoBehaviour
{
    public int controllerNumber;
    public bool controllerAssigned;
    [SerializeField]
    Camera camera;
    [SerializeField]
    GameObject spawnPointVisualizer;//when spawning, this object appears at the currently selected spawn point

    [SerializeField]
    float eatDistance;
    Eatable itemToEat;
    public Eatable GetItemToEat
    {
        get { return itemToEat; }
    }

    SpawnPoint[] spawnPoints;

    float lastVerticalStickValue;

    public int spawnPointIndex;

    
    public bool isDetectableWhenStill;

    [SerializeField]
    InLevelHoardManager hoardManager;
    //player state machine
    public enum PlayerState
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

    [SerializeField]
    int maxHealth;
    int health;
    float healthRegenerateTimer;
    [SerializeField]
    float healthRegenerateMaxTime;
    #region Transformation management variables
    //[SerializeField]
    //int currentTransformationIndex, previousTransformationIndex;
    //[SerializeField]
    //List<bool> unlockedTransformations;//When you unlock a transformation, set it to true in this list

    [SerializeField]//Set other booleans here to determine which transformations use which scripts
    bool canMove, canTransform;

    //bool toTransform;

    [SerializeField]
    Canvas transformUI;

    enum Transformations
    {
        Chest,
        Book,
        NumberOfTypes
    }

    enum ScriptNames
    {
        Move,
        Eat,
        NumberOfTypes
    }
    #endregion

    //Other Scripts
    ItemInventory itemInventory;
    [SerializeField]
    Animator animator;

    public PlayerState myPlayerState = PlayerState.None;
    public bool inEatingDistance = false;
    public bool villagerDeath = false;

    //Referencing SoundFX
    public GameObject soundFX;

    //Rigidbody myRigidbody;
    private float SpeedPerSecond;
    public Vector3 desiredDirection = new Vector3();
    public float RotationSpeed = 0.25f;
    public float walkSpeed;
    public float runSpeed;

    [SerializeField]
    float invulnerableMaxTime;
    float invulnerableTimer;
    bool isInvulnerable;

    // Use this for initialization
    void Start()
    {
        isInvulnerable = false;
        SpeedPerSecond = walkSpeed;
        //myRigidbody = GetComponent<Rigidbody>();
        SetPlayerState(PlayerState.Idle);
        itemInventory = gameObject.GetComponent<ItemInventory>();
        animator = gameObject.GetComponent<Animator>();
        spawnPoints = FindObjectsOfType<SpawnPoint>();

        health = maxHealth;
        if (eatDistance == 0)
            eatDistance = 2.5f;

        #region Set list of scripts for each transformation that are enabled (List<List<bool>>()) DISABLED
        //scriptSetsForTransformations = new List<List<bool>>();
        //for(int i = 0; i < (int)Transformations.NumberOfTypes; i++)
        //{//initialize all transformation script references to false
        //    scriptSetsForTransformations.Add(new List<bool>());
        //    for(int j = 0; j < (int)ScriptNames.NumberOfTypes; j++)
        //    {
        //        scriptSetsForTransformations[i].Add(new bool());
        //        scriptSetsForTransformations[i][j] = false;
        //    }
        //}

        ////Individually enable each script for each transformation
        //scriptSetsForTransformations[(int)Transformations.Chest][(int)ScriptNames.Eat] = true;
        //scriptSetsForTransformations[(int)Transformations.Chest][(int)ScriptNames.Move] = true;
        //scriptSetsForTransformations[(int)Transformations.Book][(int)ScriptNames.Eat] = false;
        //scriptSetsForTransformations[(int)Transformations.Book][(int)ScriptNames.Move] = false;

        #endregion
        //InvokeRepeating("SpawnSoundEvent", 0.0f, 0.5f); Sprinting not making sound events
    }

    void FixedUpdate()
    {
        if(isInvulnerable)
        {
            invulnerableTimer += Time.deltaTime;
            if (invulnerableTimer >= invulnerableMaxTime)
                isInvulnerable = false;
            else if(Mathf.Round(invulnerableTimer * 200) % 70 <= 30)
            {

                transform.localScale = new Vector3();
                //transform.position = new Vector3(transform.position.x, transform.position.y + 500, transform.position.z);
                //canMove = false;
            }
            else if(Mathf.Round(invulnerableTimer * 200) % 70 >= 31)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                //transform.position = new Vector3(transform.position.x, transform.position.y + 500, transform.position.z);
                //canMove = true;
            }

        }

        #region player movement code for current transformation
        if (myPlayerState != PlayerState.Death)
        {
            if (animator.GetBool("isEating"))
            {
                canMove = false;
            }
            else
                canMove = true;
            if (canMove)// &&
                        //!transformUI.GetComponent<Canvas>().enabled)
            {//if current transformation has movement enabled and the transform select canvas is not enabled
                desiredDirection.x = ControllerManager.MainHorizontal(controllerNumber);
                desiredDirection.z = -ControllerManager.MainVertical(controllerNumber);


                transform.position += desiredDirection * SpeedPerSecond * Time.deltaTime;
                //GetComponent<Rigidbody>().velocity = desiredDirection * SpeedPerSecond * Time.deltaTime * 100;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

                Vector3 movement = new Vector3(desiredDirection.x, 0.0f, desiredDirection.z);
                if (movement != Vector3.zero)
                {

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), RotationSpeed);
                }
            }
            else if (!canMove)
            {
                desiredDirection = new Vector3();
                //Debug.Log(desiredDirection);
                //Debug.Log(transform.position);
            }
            #endregion
            StandardPlayerStateChangeCode();//animation state manager

            if (health < maxHealth)
            {//if missing health
                healthRegenerateTimer += Time.deltaTime;
                if (healthRegenerateTimer >= healthRegenerateMaxTime)
                {//if enough time has passed since taking damage
                    health = maxHealth;
                    healthRegenerateTimer = 0;
                }
            }
        }
        else
        {//If myPlayerState is dead

            if(ControllerManager.MainVertical(controllerNumber) >= 0.4f && lastVerticalStickValue < 0.4)
            {
                ChangeSpawnPointIndex(1);
            }
            if (ControllerManager.MainVertical(controllerNumber) <= -0.4f && lastVerticalStickValue > -0.4)
            {
                ChangeSpawnPointIndex(-1);
            }
            spawnPointVisualizer.transform.localScale = Vector3.Lerp(spawnPointVisualizer.transform.localScale, new Vector3(1, 1, 1), 0.1f);
            lastVerticalStickValue = ControllerManager.MainVertical(controllerNumber);
            if (ControllerManager.EatButton(controllerNumber))
            {
                transform.position = spawnPoints[spawnPointIndex].transform.position;
                spawnPointVisualizer.transform.localScale = new Vector3(0, 0, 0);
                myPlayerState = PlayerState.Idle;
            }
        }
    }

    void ChangeSpawnPointIndex(int direction)
    {
        spawnPointIndex += (int)Mathf.Sign((float)direction);
        //spawnPointIndex = spawnPointIndex % (spawnPoints.Length);

        if (spawnPointIndex < 0)
            spawnPointIndex = spawnPoints.Length - 1;
        else if (spawnPointIndex >= spawnPoints.Length)
            spawnPointIndex = 0;
        //if (spawnPointIndex < 0)
          //  spawnPointIndex = spawnPoints.Length - 1;
        while (!spawnPoints[spawnPointIndex].spawnPointIsAccessible)
        {
            spawnPointIndex += (int)Mathf.Sign((float)direction);
        }
        spawnPointVisualizer.transform.position = spawnPoints[spawnPointIndex].transform.position;
        spawnPointVisualizer.transform.localScale = new Vector3(2, 2, 2);
    }

    internal void SetControllerNumber(int number)
    {

    }

    public void DamagePlayer()
    {
        if (!isInvulnerable)
        {
            Debug.Log("Damage Player");
            health -= 1;
            if (health <= 0)
                Die();

            isInvulnerable = true;
            invulnerableTimer = 0;
        }

    }

    public void Die()
    {
        //this.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        //gameObject.renderer.material.color.a = 0.4f;
        //GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0.4f);
        hoardManager.DropItems(transform.position);
        transform.position = new Vector3(transform.position.x, 500, transform.position.z);
        SetPlayerState(PlayerState.Death);
        ChangeSpawnPointIndex(0);
        health = maxHealth;

        
    }

    public void Respawn()
    {
        //GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 1f);
    }

    private void Update()
    {
        //SpawnSoundEvent();
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        if (myPlayerState == PlayerState.Death)
            camera.transform.position = new Vector3(Mathf.Lerp(camera.transform.position.x, spawnPoints[spawnPointIndex].transform.position.x, 0.2f), 
                                                    camera.transform.position.y,
                                                    Mathf.Lerp(camera.transform.position.z, spawnPoints[spawnPointIndex].transform.position.z - 3, 0.2f));
        else
            camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, transform.position.z - 10);
        //TODO: When respawning, lerp camera position to hover over currently selected spawn point
    }
    // Update is called once per frame

    //private void EnableTransform()
    //{//Set transformationUI to true
    //    transformUI.GetComponent<Canvas>().enabled = true;
    //    toTransform = false;
    //}

    //Player/Mimic State Machine: Allows for State to change
    void StandardPlayerStateChangeCode()
    {

        switch (myPlayerState)
        {
            case PlayerState.Idle:
                if (ControllerManager.MainVertical(controllerNumber) != 0  || ControllerManager.MainHorizontal(controllerNumber) != 0)
                {
                    animator.SetBool("isWalking", true);
                    SetPlayerState(PlayerState.Walk);
                }
                else if (Input.GetKey(KeyCode.K))
                {
                    SetPlayerState(PlayerState.Death);
                }
                else if (ControllerManager.EatButton(controllerNumber))
                {
                    villagerDeath = true;
                    //Debug.Log(villagerDeath);
                    SetPlayerState(PlayerState.Eating);
                    //Debug.Log("itemEaten");
                }

                if (ControllerManager.SprintButton(controllerNumber) && myPlayerState == PlayerState.Walk)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", true);
                    SetPlayerState(PlayerState.Run);
                }

                break;
            case PlayerState.Walk:
                if (!(ControllerManager.MainHorizontal(controllerNumber) != 0 || ControllerManager.MainVertical(controllerNumber) != 0))
                {//No movement buttons pressed
                    animator.SetBool("isWalking", false);
                    SetPlayerState(PlayerState.Idle);
                }
                else if (ControllerManager.SprintButton(controllerNumber))
                {//Run button pressed
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isWalking", false);
                    SetPlayerState(PlayerState.Run);
                }
                else if (ControllerManager.EatButton(controllerNumber))
                {//Eat button pressed
                    animator.SetBool("isWalking", false);
                    SetPlayerState(PlayerState.Eating);
                }
                break;
            case PlayerState.Run:
                if (!ControllerManager.SprintButton(controllerNumber) || (ControllerManager.MainHorizontal(controllerNumber) == 0 && ControllerManager.MainVertical(controllerNumber) == 0))
                {//No run button pressed and not moving
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                    SetPlayerState(PlayerState.Walk);

                }
                else if (ControllerManager.EatButton(controllerNumber))
                {//Eat button pressed
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
                    if (itemToEat != null)
                    {
                        GetComponent<Rigidbody>().isKinematic = false;
                        //find the vector pointing from our position to the target
                        Vector3 _direction = (itemToEat.transform.position - transform.position).normalized;

                        //create the rotation we need to be in to look at the target
                        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

                        itemToEat.transform.position = Vector3.Lerp(itemToEat.transform.position, transform.position, 0.12f);
                        itemToEat.transform.localScale = Vector3.Lerp(itemToEat.transform.localScale, new Vector3(), 0.08f);

                        //rotate us over time according to speed until we are in the required rotation
                        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, RotationSpeed);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
                        if (itemToEat.IsEaten || !itemToEat.enabled)
                        {
                            animator.SetBool("isEating", false);
                        }
                        if (!animator.GetBool("isEating"))
                        {//Immediately stops eating
                            SetPlayerState(PlayerState.Idle);
                        }
                    }
                    else
                    {
                        SetPlayerState(PlayerState.Idle);
                        animator.SetBool("isEating", false);
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
            {//if the player stops running, cancel invoked sound spawning
                case PlayerState.Run:
                    CancelInvoke("SpawnSoundEvent");
                    break;
                case PlayerState.Eating://if player stops eating, can move
                    //canMove = true;
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
                    //TODO: More fancy death stuff?
                    break;

                case PlayerState.Eating:
                    //player eating animation

                    Eatable currentTarget = null;
                    float distanceToClosestTarget = float.MaxValue;
                    canMove = false;
                    Eatable[] eatables = FindObjectsOfType<Eatable>();
                    foreach (Eatable eatable in eatables)
                    {//find closest eatable object/person
                        float distanceToTarget = (eatable.transform.position - transform.position).magnitude;
                        if (distanceToTarget < eatDistance)
                        {
                            if (distanceToTarget < distanceToClosestTarget)
                            {
                                itemToEat = currentTarget = eatable;
                                distanceToClosestTarget = distanceToTarget;
                            }
                        }
                    }

                    if (currentTarget != null)
                    {
                        animator.SetBool("isEating", true);
                        hoardManager.AddToHoard(currentTarget);

                        if (currentTarget.GetComponent<VillagerMain>() != null)
                        {
                            SpawnSoundEvent(SoundEvent.SoundType.EatNPC);
                        }
                    }

                    break;

                case PlayerState.Fidget:

                    break;

            }
        }
    }

    void SpawnSoundEvent(SoundEvent.SoundType inSoundType)//right now this is mostly just for prototypeing purposes. We'll have these events Instantiating in the animator
    {
        soundFX.GetComponent<SoundEvent>().thisSoundType = inSoundType;
        Instantiate(soundFX, transform.position, Quaternion.identity);
    }
    

    void SpawnSoundEvent()//right now this is mostly just for prototypeing purposes. We'll have these events Instantiating in the animator
    {
        soundFX.GetComponent<SoundEvent>().thisSoundType = SoundEvent.SoundType.SprintBounce;
        Instantiate(soundFX, transform.position, Quaternion.identity);
    }


    void OnTriggerEnter(Collider other)
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