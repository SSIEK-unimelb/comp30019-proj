using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseEnemyState
{
    private float chaseTime;
    private float chaseDuration;
    private bool firstTime = true;

    public ChaseState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) : 
                    base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Chase State");

        navMeshAgent.speed = goblinAI.GetChaseSpeed();

        chaseDuration = goblinAI.GetChaseDuration();
        chaseTime = chaseDuration;

        goblinAI.GetExclamationMark().SetActive(true);

        goblinAI.GetGoblinSounds().clip = goblinAI.GetChaseAudio();
        goblinAI.GetGoblinSounds().pitch = 3.0f;
        goblinAI.GetGoblinSounds().Play();

        if (firstTime) {
            goblinAI.GetSoundEffects().clip = goblinAI.GetDiscoverAudio();
            goblinAI.GetSoundEffects().Play();
            firstTime = false;
        }
    }

    public override void UpdateState() {
        goblinAI.Chase();

        // y is set to 0 to check for horizontal distance only.
        Vector3 enemyPos = new Vector3(goblinAI.transform.position.x, 0f, goblinAI.transform.position.z);
        Vector3 playerPos = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        bool inAttackRange = Vector3.Distance(enemyPos, playerPos) <= goblinAI.GetAttackRange();
        bool outsideChaseRange = Vector3.Distance(enemyPos, playerPos) > goblinAI.GetViewRange();

        // If the player was killed, transition to idle state.
        if (player == null) {
            goblinAI.TransitionToState(goblinAI.GetIdleState());
        }
        // Else if the player is within attack range, transition to attack state.
        else if (player != null && inAttackRange) {
            goblinAI.TransitionToState(goblinAI.GetAttackState());
        }
        // Else if the player has exited the enemy's aggro range for chaseTime, transition to idle state.
        else if (outsideChaseRange) {
            chaseTime -= Time.deltaTime;
            if (chaseTime <= 0) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }
        }
        // Else if the player is in the enemy's aggro range, reset chaseTime.
        else {
            chaseTime = chaseDuration;
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();
        goblinAI.GetGoblinSounds().pitch = 1.0f;

        goblinAI.GetExclamationMark().SetActive(false);
    }
}
