using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;

public class PatrolState : BaseEnemyState
{
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

        // y is set to 0 so that the patrol point can be at any height.
        Vector3 destinationPos = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 currentPos = new Vector3(goblinAI.transform.position.x, 0, goblinAI.transform.position.z);
        bool hasReachedPatrolPoint = Vector3.Distance(destinationPos, currentPos) <= navMeshAgent.stoppingDistance;

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
