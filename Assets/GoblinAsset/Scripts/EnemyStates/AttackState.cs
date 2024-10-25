using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseEnemyState
{
    private float currentUpdateTime;
    private float outsideAttackRangeTime;
    private float outsideAttackRangeDuration = 0.5f;
    
    public AttackState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) : 
                        base(goblinAI, navMeshAgent, player) { }

    public override void EnterState() {
        Debug.Log("Entering Attack State");

        Vector3 playerDirection = (player.transform.position - goblinAI.transform.position).normalized;
        navMeshAgent.destination = goblinAI.transform.position + playerDirection;
        navMeshAgent.speed = 0;

        goblinAI.SetCurrentAttackCooldown(0);
        outsideAttackRangeDuration = goblinAI.GetOutsideAttackRangeDuration();
        outsideAttackRangeTime = outsideAttackRangeDuration;

        goblinAI.GetExclamationMark().SetActive(true);

        if (goblinAI.GetFirstTimeSeen()) {
            goblinAI.GetSoundManager().PlaySoundEffect(goblinAI.GetDiscoverAudio(), 1.0f);
            goblinAI.SetFirstTimeSeenToFalse();
        }
    }

    public override void UpdateState() {
        goblinAI.Attack();

        currentUpdateTime -= Time.deltaTime;
        if (currentUpdateTime > 0) {
            return;
        }
        currentUpdateTime = updateTimeStep;

        // If player was killed, transition to idle state.
        if (player == null) {
            goblinAI.TransitionToState(goblinAI.GetIdleState());
        }

        // y is set to 0 to check for horizontal distance only.
        Vector2 enemyPos = new Vector2(goblinAI.transform.position.x, goblinAI.transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        bool inAttackRange = Vector2.Distance(enemyPos, playerPos) <= goblinAI.GetAttackRange();

        // If the enemy is currently attacking, wait for the attack to finish first before checking anything.
        if (goblinAI.IsAttacking()) return;
        // Else if the player has exited attack range for some time, transition to chase state.
        else if (!inAttackRange) {
            outsideAttackRangeTime -= updateTimeStep;
            if (outsideAttackRangeTime <= 0) {
                goblinAI.TransitionToState(goblinAI.GetChaseState());
            }
        } else {
            // Reset outside attack range time.
            outsideAttackRangeTime = outsideAttackRangeDuration;
        }
    }

    public override void ExitState() {
        goblinAI.ResetAnimationTriggers();
        goblinAI.GetGoblinSounds().Stop();

        goblinAI.GetExclamationMark().SetActive(false);
    }
}
