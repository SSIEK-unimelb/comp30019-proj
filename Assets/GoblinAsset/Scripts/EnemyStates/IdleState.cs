using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseEnemyState
{
    private float idleTime;
    private float idleDuration;

    public IdleState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, FirstPersonControl player) : 
                    base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Idle State");
        idleDuration = goblinAI.GetIdleDuration();
        idleTime = idleDuration;
    }

    public override void UpdateState() {
        goblinAI.Idle();

        if (player == null) return;

        // If enough time has passed, transition to patrol state.
        idleTime -= Time.deltaTime;
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
