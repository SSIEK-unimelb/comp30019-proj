using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseEnemyState
{
    private float currentUpdateTime;
    private float idleTime;
    private float idleDuration;

    public IdleState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) : 
                    base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Idle State");
        navMeshAgent.destination = goblinAI.transform.position;
        navMeshAgent.speed = 0;
        idleDuration = goblinAI.GetIdleDuration();
        idleTime = idleDuration;
    }

    public override void UpdateState() {
        goblinAI.Idle();

        currentUpdateTime -= Time.deltaTime;
        if (currentUpdateTime > 0) {
            return;
        }
        currentUpdateTime = updateTimeStep;

        if (player == null) return;

        // If enough time has passed, transition to patrol state.
        idleTime -= updateTimeStep;
        if (idleTime <= 0) {
            goblinAI.TransitionToState(goblinAI.GetPatrolState());
        }
        // If the player is not dead.
        else if (player != null) {
            // If the enemy can see the player or hear something, transition to search state.
            if (goblinAI.CanSeePlayer() || goblinAI.GetCanHearSound()) {
                goblinAI.TransitionToState(goblinAI.GetSearchState());
            }
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();
    }
}
