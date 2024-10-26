using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GoblinAI : MonoBehaviour
{
    private GameObject player;
    private Transform rayOrigin;
    private string rayOriginName = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/" +
                                    "mixamorig:Neck/mixamorig:Head/Raycast_Position";
    RaycastHit hit;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private AudioSource goblinSounds;
    private SoundManager soundManager;
    private ObjectSoundCollision objectSoundCollision;

    [SerializeField] private float updateTimeStep = 0.2f;

    [Header("Idle")]
    [SerializeField] private float idleDuration = 5f;
    [SerializeField] private AnimationClip idleAnimation;
    
    [Header("Patrol")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private List<Transform> patrolPoints;
    private Transform currentPatrolPoint;
    private int patrolIndex = 0;
    [SerializeField] private AnimationClip walkAnimation;
    [SerializeField] private AudioClip walkAudio;

    [Header("Search")]
    [SerializeField] private float hearRadius = 10f;
    [SerializeField] private float searchDuration = 10f;
    private bool canHearSound = false;
    private Vector3 soundHeardPos;

    [Header("Chase")]
    [SerializeField] private float viewRange = 15f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float waitDurationToChase = 1f;
    [SerializeField] private float playerLocationCheckInterval = 0.1f;
    private float currentPlayerLocationCheckTime = 0;
    [SerializeField] private float chaseDurationInRange = 5f;
    [SerializeField] private float chaseDurationOutsideRange = 2f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private AnimationClip chaseAnimation;
    [SerializeField] private AudioClip chaseAudio;
    [SerializeField] private AudioClip discoverAudio;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float outsideAttackRangeDuration = 0.5f;
    [SerializeField] private float attackCooldown = 1f;
    private float currentAttackCooldown;
    private bool isAttacking = false;
    [SerializeField] private AnimationClip attackAnimation;
    private float attackAnimationLength;
    private float currentAttackAnimationLength;
    [SerializeField] private AnimationClip idleAttackAnimation;
    [SerializeField] private AudioClip attackAudio;

    [Header("Damage")]
    private string backHitpointName = "mixamorig:Hips/mixamorig:Spine/BackHitpoint";
    [SerializeField] private string damageTriggerName = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/" +
                                                        "mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/" +
                                                        "DamageTrigger";
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private string tagToDamage = "Player";
    
    [Header("Suspicion Icons")]
    private GameObject questionMark;
    private GameObject exclamationMark;
    private bool firstTimeSeen = true;

    // These variables are used to turn the enemy so that it faces the direction it is moving in,
    // or so that it faces the player when in attack range.
    [Header("Other")]
    [SerializeField] private float rotationTime = 0.3f;
    private float yVelocity;    // This variable does not do anything, just to plug something in the method.
    private string headColliderName = "ColliderToPreventPlayerJumpingOnGoblin";

    [Header("States")]
    private BaseEnemyState currentState;
    private IdleState idleState;
    private PatrolState patrolState;
    private SearchState searchState;
    private ChaseState chaseState;
    private AttackState attackState;

    public float GetUpdateTimeStep() { return updateTimeStep; }
    public float GetIdleDuration() { return idleDuration; }
    public float GetWalkSpeed() { return walkSpeed; }
    public Transform GetCurrentPatrolPoint() { return currentPatrolPoint; }
    public float GetSearchDuration() { return searchDuration; }
    public bool GetCanHearSound() { return canHearSound; }
    public void SetCanHearSound(bool canHearSound) { this.canHearSound = canHearSound; }
    public float GetViewRange() { return viewRange; }
    public float GetWaitDurationToChase() { return waitDurationToChase; }
    public float GetChaseDurationInRange() { return chaseDurationInRange; }
    public float GetChaseDurationOutsideRange() { return chaseDurationOutsideRange; }
    public float GetChaseSpeed() { return chaseSpeed; }
    public float GetOutsideAttackRangeDuration() { return outsideAttackRangeDuration; }
    public float GetAttackCooldown() { return attackCooldown; }
    public void SetCurrentAttackCooldown(float currentAttackCooldown) { this.currentAttackCooldown = currentAttackCooldown; }
    public bool IsAttacking() { return isAttacking; }
    public float GetAttackRange() { return attackRange; }
    public int GetDamageAmount() { return damageAmount; }
    public string GetTagToDamage() { return tagToDamage; }

    public AudioSource GetGoblinSounds() { return goblinSounds; }
    public AudioClip GetWalkAudio() { return walkAudio; }
    public AudioClip GetChaseAudio() { return chaseAudio; }
    public AudioClip GetDiscoverAudio() { return discoverAudio; }
    public SoundManager GetSoundManager() { return soundManager; }

    public GameObject GetQuestionMark() { return questionMark; }
    public GameObject GetExclamationMark() { return exclamationMark; }
    public bool GetFirstTimeSeen() { return firstTimeSeen; }
    public void SetFirstTimeSeenToFalse() { firstTimeSeen = false; }

    public IdleState GetIdleState() { return idleState; }
    public PatrolState GetPatrolState() { return patrolState; }
    public SearchState GetSearchState() { return searchState; }
    public ChaseState GetChaseState() { return chaseState; }
    public AttackState GetAttackState() { return attackState; }

    void Awake() {
        player = GameObject.FindWithTag("Player");
        rayOrigin = transform.Find(rayOriginName).transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        animator = GetComponent<Animator>();
        goblinSounds = GetComponent<AudioSource>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        objectSoundCollision = GetComponent<ObjectSoundCollision>();

        questionMark = transform.Find("Question_Mark").gameObject;
        exclamationMark = transform.Find("Exclamation_Mark").gameObject;

        idleState = new IdleState(this, navMeshAgent, player);
        patrolState = new PatrolState(this, navMeshAgent, player);
        searchState = new SearchState(this, navMeshAgent, player);
        chaseState = new ChaseState(this, navMeshAgent, player);
        attackState = new AttackState(this, navMeshAgent, player);

        
        currentState = idleState;
        currentState.EnterState();

        currentPatrolPoint = patrolPoints[patrolIndex];

        // Get the length of the attack animation
        // Get the RuntimeAnimatorController from the Animator
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        // Iterate through all AnimationClips in the controller
        foreach (AnimationClip clip in controller.animationClips) {
            if (clip.name == attackAnimation.name) {
                attackAnimationLength = clip.length;
            }
        }
    }

    void Start()
    {
        GetComponent<HoldStatus>().CanBeHeld = false;
    }
    void Update() {
        currentState.UpdateState();
    }

    // Exit the current state, and enter the new state.
    public void TransitionToState(BaseEnemyState newState) {
        currentState.ExitState();
        currentState = newState;
        newState.EnterState();
    }

    public void Idle() {
        animator.SetTrigger(idleAnimation.name);
    }

    public void Patrol() {
        animator.SetTrigger(walkAnimation.name);
        RotateTowardsMovementDirection();
    }

    public void Search() {
        animator.SetTrigger(walkAnimation.name);

        if (navMeshAgent.speed == 0) {
            RotateTowardsPlayer();
        } else {
            RotateTowardsMovementDirection();
        }

        if (CanSeePlayer()) {
            navMeshAgent.destination = player.transform.position;
        } else {
            navMeshAgent.destination = soundHeardPos;
        }
    }

    public void Chase() {
        animator.SetTrigger(chaseAnimation.name);
        RotateTowardsMovementDirection();
        
        // If the player is dead, do nothing.
        if (player == null) return;

        // No need to check player location every frame.
        currentPlayerLocationCheckTime -= Time.deltaTime;
        if (currentPlayerLocationCheckTime > 0) return;
        currentPlayerLocationCheckTime = playerLocationCheckInterval;
        navMeshAgent.destination = player.transform.position;
    }

    public void Attack() {
        if (isAttacking) {
            currentAttackAnimationLength -= Time.deltaTime;
            if (currentAttackAnimationLength <= 0) {
                isAttacking = false;
            }
        } else {
            animator.SetTrigger(idleAttackAnimation.name);
            RotateTowardsPlayer();

            // Calculate the current cooldown time. If cooldown is over, attack.
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown <= 0) {
                currentAttackCooldown = attackCooldown;

                Debug.Log("Attack!");
                animator.ResetTrigger(idleAttackAnimation.name);
                animator.SetTrigger(attackAnimation.name);
                goblinSounds.PlayOneShot(attackAudio);

                isAttacking = true;
                currentAttackAnimationLength = attackAnimationLength;
            }
        }
    }

    // Returns true if the enemy can be killed, false otherwise.
    public bool IsKillable() {
        return currentState is IdleState || currentState is PatrolState || currentState is SearchState;
    }

    // This triggers the death animation, removes this script so enemy cannot move.
    public void Die() {
        if (currentState == null) {
            return;
        }
        currentState.ExitState();
        currentState = null;

        // To stop movement.
        Destroy(gameObject.GetComponent<BoxCollider>());
        Destroy(navMeshAgent);

        // To stop the player from being damaged by the enemy.
        Destroy(transform.Find(damageTriggerName).gameObject);
        Destroy(transform.Find(headColliderName).gameObject);
        Destroy(transform.Find(backHitpointName).gameObject);

        // Destroy the audiosources
        Destroy(goblinSounds);

        // Can now be held.
        GetComponent<HoldStatus>().CanBeHeld = true;

        // To start ragdoll, need a rigidbody.
        gameObject.AddComponent(typeof(Rigidbody));
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        ResetAnimationTriggers();
        Destroy(animator);

        // Destroy this script.
        Destroy(this);
    }

    // Set the next patrol point. If the last patrol point was reached, reset.
    public void NextPatrolPoint() {
        patrolIndex++;
        if (patrolIndex >= patrolPoints.Count) patrolIndex = 0;
        currentPatrolPoint = patrolPoints[patrolIndex];
    }

    // Enemy Vision.
    public bool CanSeePlayer() {
        if (player == null) return false;

        // Direction from the enemy to the player.
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;

        // y is set to 0 to check for horizontal distance only - Quick fix for crouching.
        Vector2 enemyPos = new Vector2(transform.position.x,transform.position.z);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        float distanceToPlayer = Vector2.Distance(enemyPos, playerPos) + 0.1f;

        // Debug.DrawRay(_rayOrigin.position, playerTarget * distanceToTarget, Color.red);
        // If player is in view range,
        if (distanceToPlayer <= viewRange) {
            // If player is in the current field of view,
            if (Vector3.Angle(transform.forward, playerDirection) <= viewAngle / 2) {
                // Cast a ray towards the player
                if (Physics.Raycast(rayOrigin.position, playerDirection, out hit, distanceToPlayer)) {
                    // Check if the ray hit the player
                    if (hit.transform.root.CompareTag("Player")) {
                        // The enemy can see the player
                        // Debug.Log("I see you!");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Enemy Hearing for player/objects.
    public void CanHearSound(Vector3 soundOrigin) {
        // y is set to 0 to check for horizontal distance only - Quick fix for crouching.
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 soundPos = new Vector2(soundOrigin.x, soundOrigin.z);
        float distance = Vector2.Distance(enemyPos, soundPos);

        canHearSound = false;
        // If the player/object is in hearing range,
        if (distance <= hearRadius) {
            Debug.Log(gameObject.name + " heard a sound from: " + soundOrigin);
            soundHeardPos = soundOrigin;
            canHearSound = true;
        }
    }

    // Rotate smoothly towards movement direction / towards player when in attack range.
    private void RotateTowardsMovementDirection() {
        if (navMeshAgent.velocity != Vector3.zero) {
            float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                                                        Quaternion.LookRotation(navMeshAgent.velocity).eulerAngles.y, 
                                                        ref yVelocity, rotationTime);
            navMeshAgent.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
        }
    }

    // Rotate smoothly towards player when in attack range
    private void RotateTowardsPlayer() {
        if (player == null) return;
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;
        Vector3 playerDirectionWithoutY = new Vector3(playerDirection.x, 0, playerDirection.z);
        float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                                                    Quaternion.LookRotation(playerDirectionWithoutY).eulerAngles.y, 
                                                    ref yVelocity, rotationTime);
        navMeshAgent.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
    }

    public void ResetAnimationTriggers() {
        animator.ResetTrigger(idleAnimation.name);
        animator.ResetTrigger(walkAnimation.name);
        animator.ResetTrigger(chaseAnimation.name);
        animator.ResetTrigger(attackAnimation.name);
        animator.ResetTrigger(idleAttackAnimation.name);
    }
}