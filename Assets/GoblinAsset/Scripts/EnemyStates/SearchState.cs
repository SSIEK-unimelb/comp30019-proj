using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : BaseEnemyState
{
    private float waitDuration;
    private float currentWaitTime;

    private float searchDuration;
    private float currentSearchTime;

    public SearchState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, FirstPersonControl player) : 
                        base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Search State");
        
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

        // y is set to 0 so that the search point can be at any height.
        Vector3 destinationPos = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 currentPos = new Vector3(goblinAI.transform.position.x, 0, goblinAI.transform.position.z);
        bool hasReachedSoundHeardPoint = Vector3.Distance(destinationPos, currentPos) <= navMeshAgent.stoppingDistance;

        // If the enemy can see the player, and some time has passed, transition to chase state.
        if (player != null && goblinAI.CanSeePlayer()) {
            currentWaitTime -= Time.deltaTime;
            if (currentWaitTime <= 0) {
                goblinAI.TransitionToState(goblinAI.GetChaseState());
            }
        }
        // Else if the enemy cannot see the player, 
        else if (!goblinAI.CanSeePlayer()) {
            currentSearchTime -= Time.deltaTime;

            // If the enemy has reached its destination, transition to idle state.
            if (hasReachedSoundHeardPoint) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }
            // If the enemy has spent too much time trying to reach its destination.
            else if (currentSearchTime <= 0) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }

            // Reset wait time.
            currentWaitTime = waitDuration;
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();

        goblinAI.GetQuestionMark().SetActive(false);
    }
}
