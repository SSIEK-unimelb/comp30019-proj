using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseEnemyState
{
    private float currentUpdateTime;
    private float chaseDurationInRange;
    private float chaseTimeInRange;
    private float chaseDurationOutsideRange;
    private float chaseTimeOutsideRange;
    private bool firstTime = true;

    public ChaseState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) : 
                    base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Chase State");

        navMeshAgent.speed = goblinAI.GetChaseSpeed();

        chaseDurationInRange = goblinAI.GetChaseDurationInRange();
        chaseTimeInRange = chaseDurationInRange;

        chaseDurationOutsideRange = goblinAI.GetChaseDurationOutsideRange();
        chaseTimeOutsideRange = chaseDurationOutsideRange;

        goblinAI.GetExclamationMark().SetActive(true);

        goblinAI.GetGoblinSounds().clip = goblinAI.GetChaseAudio();
        goblinAI.GetGoblinSounds().pitch = 3.0f;
        goblinAI.GetGoblinSounds().Play();

        if (firstTime) {
            goblinAI.GetSoundManager().PlaySoundEffect(goblinAI.GetDiscoverAudio(), 1.0f);
            firstTime = false;
        }

        goblinAI.GetSoundManager().OnEnterChase();
    }

    public override void UpdateState() {
        goblinAI.Chase();

        currentUpdateTime -= Time.deltaTime;
        if (currentUpdateTime > 0) {
            return;
        }
        currentUpdateTime = updateTimeStep;

        // y is set to 0 to check for horizontal distance only.
        Vector2 enemyPos = new Vector2(goblinAI.transform.position.x, goblinAI.transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        float distance = Vector2.Distance(enemyPos, playerPos);
        bool inAttackRange = distance <= goblinAI.GetAttackRange();
        bool outsideChaseRange = distance > goblinAI.GetViewRange();

        // If the player was killed, transition to idle state.
        if (player == null) {
            goblinAI.TransitionToState(goblinAI.GetIdleState());
            return;
        }

        // If the player is within attack range, transition to attack state.
        if (inAttackRange) {
            goblinAI.TransitionToState(goblinAI.GetAttackState());
        }
        // Else if the player has exited the enemy's aggro range for chaseTime, transition to idle state.
        else if (outsideChaseRange) {
            chaseTimeInRange = chaseDurationInRange;

            chaseTimeOutsideRange -= updateTimeStep;
            if (chaseTimeOutsideRange <= 0) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }
        }
        // Else if the player is in the enemy's aggro range.
        else {
            chaseTimeInRange -= updateTimeStep;
            if (chaseTimeInRange <= 0) {
                goblinAI.TransitionToState(goblinAI.GetIdleState());
            }
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();
        goblinAI.GetGoblinSounds().pitch = 1.0f;

        goblinAI.GetExclamationMark().SetActive(false);

        goblinAI.GetSoundManager().OnExitChase();
    }
}
