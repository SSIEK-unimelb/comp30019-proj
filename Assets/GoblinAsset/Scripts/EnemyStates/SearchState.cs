using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : BaseEnemyState
{
    private float currentUpdateTime;
    private float waitDuration;
    private float currentWaitTime;

    private float searchDuration;
    private float currentSearchTime;

    public SearchState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) : 
                        base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Search State");
        goblinAI.SetCanHearSound(false);
        
        navMeshAgent.speed = goblinAI.GetWalkSpeed();

        waitDuration = goblinAI.GetWaitDurationToChase();
        currentWaitTime = waitDuration;

        searchDuration = goblinAI.GetSearchDuration();
        currentSearchTime = searchDuration;

        goblinAI.GetQuestionMark().SetActive(true);

        goblinAI.GetGoblinSounds().clip = goblinAI.GetWalkAudio();
        goblinAI.GetGoblinSounds().Play();
    }

    public override void UpdateState() {
        goblinAI.Search();

        currentUpdateTime -= Time.deltaTime;
        if (currentUpdateTime > 0) {
            return;
        }
        currentUpdateTime = updateTimeStep;

        if (player == null) {
            goblinAI.TransitionToState(goblinAI.GetIdleState());
            return;
        }

        // If the enemy can see the player, and some time has passed, transition to chase state.
        if (goblinAI.CanSeePlayer()) {
            currentWaitTime -= updateTimeStep;
            if (currentWaitTime <= 0) {
                goblinAI.TransitionToState(goblinAI.GetChaseState());
            }
        }
        // Else if the enemy cannot see the player, 
        else {
            // y is set to 0 so that the search point can be at any height.
            Vector2 destinationPos = new Vector2(navMeshAgent.destination.x, navMeshAgent.destination.z);
            Vector2 currentPos = new Vector2(goblinAI.transform.position.x, goblinAI.transform.position.z);
            bool hasReachedSoundHeardPoint = Vector2.Distance(destinationPos, currentPos) <= navMeshAgent.stoppingDistance;

            bool inAttackRange = Vector3.Distance(goblinAI.transform.position, player.transform.position) <= goblinAI.GetAttackRange();

            currentSearchTime -= updateTimeStep;

            // If the enemy is within attack range, transition to attack range.
            if (inAttackRange) {
                goblinAI.TransitionToState(goblinAI.GetAttackState());
            }
            // If the enemy has reached its destination, transition to idle state.
            else if (hasReachedSoundHeardPoint) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }
            // If the enemy has spent too much time trying to reach its destination.
            else if (currentSearchTime <= 0) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }

            // Reset wait time.
            // currentWaitTime = waitDuration;
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();

        goblinAI.GetQuestionMark().SetActive(false);
    }
}
