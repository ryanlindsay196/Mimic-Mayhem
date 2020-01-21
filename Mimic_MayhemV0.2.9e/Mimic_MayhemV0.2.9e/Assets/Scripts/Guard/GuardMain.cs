using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMain : VillagerMain {

    [SerializeField]
    Animator swordAnimator;
    bool waitingAfterAttack;
    float waitAfterAttackTimer;
    [SerializeField]
    float waitAfterAttackMaxTime;
    [SerializeField]
    Animator guardAnimator;

    [SerializeField]
    float maxSwingRange;//When chasing, swing at player within this range 

    [SerializeField]
    GameObject swordBoxCollider;

    //public override void Update()
    //{
        
    //}

    public override void VillagerWander()
    {
        //guardAnimator.SetBool("isWalking", true);
        guardAnimator.SetBool("isRunning", false);
        guardAnimator.SetBool("isCalmingDown", false);
        guardAnimator.SetBool("spottedMimic", false);
        guardAnimator.SetBool("mimicInAttackRange", false);
        guardAnimator.SetBool("startChasing", false);
        //Check if we're close to the destination
        if (base.traveling && base.navMeshAgent.remainingDistance <= 1.0f)
        {
            base.traveling = false;
            //If we're going to wait, then wait
            if (base.patrolWaiting)
            {
                base.waiting = true;
                guardAnimator.SetBool("toIdle", true);
                guardAnimator.SetBool("isWalking", false);

                base.waitTimer = 0f;
            }
            else
            {
                guardAnimator.SetBool("toIdle", false);
                guardAnimator.SetBool("isWalking", true);
                waiting = false;
                base.SetDestination();
                base.currentPatrolIndex++;
                base.currentPatrolIndex = base.currentPatrolIndex % base.patrolWayPoints.Count;

            }
        }
        //Instead if we're waiting
        if (waiting)
        {
            waitTimer += Time.deltaTime;
            guardAnimator.SetBool("isWalking", false);
            Debug.Log("set false walking");
            if (waitTimer >= totalWaitTime)
            {
                waiting = false;
                base.SetDestination();
                base.currentPatrolIndex++;
                base.currentPatrolIndex = base.currentPatrolIndex % base.patrolWayPoints.Count;
            }
        }
        else
            guardAnimator.SetBool("isWalking", true);
    }

    public override void VillagerFlee()
    {
        base.SetVillagerState(VillagerMain.VillagerState.Chase);
    }

    public override void ChangeAnimatorState()
    {
        //guardAnimator.SetBool("isWalking", false);
        //guardAnimator.SetBool("isRunning", false);
        //guardAnimator.SetBool("isCalmingDown", false);
        //guardAnimator.SetBool("spottedMimic", false);
        //guardAnimator.SetBool("mimicInAttackRange", false);
        //guardAnimator.SetBool("startChasing", true);
        guardAnimator.SetBool("toAlert", false);
        base.ChangeAnimatorState();
        switch(base.GetVillagerState)
        {
            case VillagerState.Wander:
                //guardAnimator.SetBool("isWalking", true);
                //guardAnimator.SetBool("isRunning", false);
                //guardAnimator.SetBool("isCalmingDown", false);
                //guardAnimator.SetBool("spottedMimic", false);
                //guardAnimator.SetBool("mimicInAttackRange", false);
                //guardAnimator.SetBool("startChasing", false);
                guardAnimator.SetBool("isCalmingDown", false);
                //guardAnimator.SetBool("toIdle", false);
                break;
            case VillagerState.Chase:
                //guardAnimator.SetBool("isWalking", false);
                guardAnimator.SetBool("toRun", true);
                //guardAnimator.SetBool("isCalmingDown", false);
                //guardAnimator.SetBool("spottedMimic", true);
                //guardAnimator.SetBool("mimicInAttackRange", false);
                //guardAnimator.SetBool("startChasing", true);
                break;
            case VillagerState.Suspicious:
                
                break;
            case VillagerState.Alert:
                guardAnimator.SetBool("toAlert", true);
                guardAnimator.SetBool("isCalmingDown", false);
                break;
            case VillagerState.CalmDown:
                guardAnimator.SetBool("toRun", false);
                guardAnimator.SetBool("isCalmingDown", true);
                break;
        }
        //if(guardAnimator.GetBool("toAttack") && GetVillagerState == VillagerState.Chase &&  && (currentlyDetectedPlayer.transform.position - transform.position).magnitude <= 2)
        {
            //currentlyDetectedPlayer.DamagePlayer();
        }

        //if(navDestination.position == transform.position)
        //{

        //    guardAnimator.SetBool("isWalking", false);
        //    guardAnimator.SetBool("isRunning", false);
        //    guardAnimator.SetBool("isCalmingDown", true);
        //    guardAnimator.SetBool("spottedMimic", false);
        //    guardAnimator.SetBool("mimicInAttackRange", false);
        //    guardAnimator.SetBool("startChasing", true);
        //}
    }

    public override void VillagerChase()
    {
        guardAnimator.SetBool("toRun", true);
        guardAnimator.SetBool("toAttack", false);
        //guardAnimator.SetBool("isWalking", false);
        //guardAnimator.SetBool("isRunning", true);
        //guardAnimator.SetBool("isCalmingDown", false);
        //guardAnimator.SetBool("spottedMimic", false);
        //guardAnimator.SetBool("mimicInAttackRange", false);
        //guardAnimator.SetBool("startChasing", true);
        //Gets position of currently activated player
        navMeshAgent.SetDestination(currentlyDetectedPlayer.transform.position);

        if (!waitingAfterAttack)
        {//if not waiting after attack
            if ((transform.position - currentlyDetectedPlayer.transform.position).magnitude <= maxSwingRange)
            {//if close enough, swing sword, then wait
                //swordAnimator.SetTrigger("Base_Attack");
                guardAnimator.SetBool("toAttack", true);
                guardAnimator.SetBool("toRun", false);
                if ((swordBoxCollider.transform.position - currentlyDetectedPlayer.transform.position).magnitude <= 2.4f)
                {
                    if (currentlyDetectedPlayer != null)
                        currentlyDetectedPlayer.DamagePlayer();
                }
                navMeshAgent.SetDestination(transform.position);
                waitingAfterAttack = true;
                waitAfterAttackTimer = 0;
            }
        }
        else
        {//wait after attack

            //guardAnimator.SetBool("isWalking", true);
            //guardAnimator.SetBool("isRunning", false);
            //guardAnimator.SetBool("isCalmingDown", false);
            //guardAnimator.SetBool("spottedMimic", false);
            //guardAnimator.SetBool("mimicInAttackRange", true);
            //guardAnimator.SetBool("startChasing", false);
            //guardAnimator.SetBool("toIdle", true);
            guardAnimator.SetBool("toAlert", false);
            guardAnimator.SetBool("toRun", false);
            guardAnimator.SetBool("toAttack", false);
            //swordBoxCollider.GetComponent<BoxCollider>().gameObject.SetActive(false);
            navMeshAgent.SetDestination(transform.position);
            waitAfterAttackTimer += Time.deltaTime;
            if (waitAfterAttackTimer >= waitAfterAttackMaxTime)
                waitingAfterAttack = false;
        }
    }
}
