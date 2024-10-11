using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;

public class PatrolState : BaseEnemyState
{
    private float currentUpdateTime;

    public PatrolState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) : 
                        base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Patrol State");
        navMeshAgent.destination = goblinAI.GetCurrentPatrolPoint().position;
        navMeshAgent.speed = goblinAI.GetWalkSpeed();

        goblinAI.GetGoblinSounds().clip = goblinAI.GetWalkAudio();
        goblinAI.GetGoblinSounds().Play();
    }

    public override void UpdateState() {
        goblinAI.Patrol();

        currentUpdateTime -= Time.deltaTime;
        if (currentUpdateTime > 0) {
            return;
        }
        currentUpdateTime = updateTimeStep;

        // y is set to 0 so that the patrol point can be at any height.
        Vector2 destinationPos = new Vector2(navMeshAgent.destination.x, navMeshAgent.destination.z);
        Vector2 currentPos = new Vector2(goblinAI.transform.position.x, goblinAI.transform.position.z);
        bool hasReachedPatrolPoint = Vector2.Distance(destinationPos, currentPos) <= navMeshAgent.stoppingDistance;

        // If the enemy can see the player or hear something, transition to search state.
        if (goblinAI.CanSeePlayer() || goblinAI.GetCanHearSound()) {
            goblinAI.TransitionToState(goblinAI.GetSearchState());
        }
        // Else if the patrol point was reached, transition to idle state.
        else if (hasReachedPatrolPoint) {
            goblinAI.NextPatrolPoint();
            goblinAI.TransitionToState(goblinAI.GetIdleState());
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();
    }
}
