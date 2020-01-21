using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timers : MonoBehaviour
{

    //Timers
    public float alertTimer = 4;
    public float suspiciousTimer = 6;
    public float lookBackTimer = 2;
    public float fleeTimer = 6;
    public float fleeCooldownTimer = 2;
    public float currentTimer = 0;
    public float coolDownPeriodInSeconds = 6;
    public float currentLBTimer = 0;
    public float calmdownTimer = 6.0f;
    public float hideTimer = 0;//no hideTimerCheck
    public float chaseTimer = 5f;

    //bools
    public bool susTimerCheck = false;
    public bool alerTimerCheck = false;
    public bool fleeTimerCheck = false;
    public bool lbTimerCheck = false;
    public bool fleeCoolTimerCheck = false;
    public bool calmdownTimerCheck = false;
    public bool chaseTimerCheck = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void SusTimer()
    {
        //Debug.Log("susTimer");
        if (currentTimer < suspiciousTimer)
        {
            currentTimer += Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentTimer >= suspiciousTimer)
        {
            susTimerCheck = true;
        }
    }

    public void ChaseTimer()
    {
        //Debug.Log("susTimer");
        if (currentTimer < chaseTimer)
        {
            currentTimer += Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentTimer >= chaseTimer)
        {
            chaseTimerCheck = true;
        }
    }

    public void AlerTimer()
    {
        //Debug.Log("susTimer");
        if (currentTimer < alertTimer)
        {
            currentTimer += Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentTimer >= alertTimer)
        {
            alerTimerCheck = true;
        }
    }

    public void CalmTimer()
    {
        //Debug.Log("susTimer");
        if (currentTimer >= alertTimer)
        {
            currentTimer -= Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentTimer <= 0)
        {
            calmdownTimerCheck = true;
        }
    }


    public void FleTimer()
    {
        //Debug.Log("susTimer");
        if (currentTimer < fleeTimer)
        {
            currentTimer += Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentTimer >= fleeTimer)
        {
            fleeTimerCheck = true;
        }
    }

    public void LBTimer()
    {
        //Debug.Log("susTimer");
        if (currentLBTimer < lookBackTimer)
        {
            currentLBTimer += Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentLBTimer >= lookBackTimer)
        {
            lbTimerCheck = true;
        }
    }

    public void FleeCoolDownTimer()
    {
        //Debug.Log("susTimer");
        if (currentTimer < fleeCooldownTimer)
        {
            currentTimer += Time.deltaTime;
            //Debug.Log(currentTimer);
        }
        if (currentTimer >= fleeCooldownTimer)
        {
            fleeCoolTimerCheck = true;
        }
    }
}
