// Code adapted from Omogonix.
// URL: https://www.youtube.com/watch?v=YFhr-pPAkkI.
// Description: The code was adapted to include a searching state, and the conditions for the enemy
//              to chase the player was changed.

// Code adapted from MKs Unity.
// URL: https://www.youtube.com/watch?v=ho7-pVNU62g&t=650s.
// Description: The code was adapted slightly to fit this project.

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GoblinAI : MonoBehaviour
{
    [Header("References")]
    private GameObject player;
    private LayerMask obstacleMask;
    private NavMeshAgent nav;
    private Transform _rayOrigin;
    [SerializeField] private GameObject backHitPoint;

    [Header("Patrol variables")]
    [SerializeField] private List<Transform> destinations;
    private Transform currentDest;
    private int patrolIndex = 0;
    private Vector3 soundPos;
    
    // AI States
    private enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Search,
        Attack,
        Dead
    }
    private AIState currentState;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float chaseSpeed = 6f;

    [Header("Detection")]
    [SerializeField] private float viewRadius = 15f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float hearRadius = 10f;
    [SerializeField] private float hearLoudness = 3f;   // This should equal the player's walkSpeed.
    [SerializeField] private float attackDistance = 2f;

    [Header("Timing")]
    [SerializeField] private float idleDuration = 5f;
    private float idleTime;

    [SerializeField] private float searchDuration = 10f;
    private float SearchTime;

    [SerializeField] private float minChaseTime = 10f;
    [SerializeField] private float maxChaseTime = 15f;
    private float chaseTime;

    [SerializeField] private float AttackInterval = 1f;
    private float attackTimer;
    [SerializeField] private float attackRotationRate = 20f;    // The speed at which the enemy rotates 
                                                                // to face the player when attacking
    private float rotationProgress;

    [Header("Suspicion Icons")]
    private GameObject questionMark;
    private GameObject exclamationMark;

    [Header("Animations and Sounds")]
    private Animator anim;
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip patrolAnimation;
    [SerializeField] private AnimationClip searchAnimation;
    [SerializeField] private AnimationClip chaseAnimation;
    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private AnimationClip dieAnimation;
    // [SerializeField] private AnimationClip killAnimation;

    private AudioSource audioSource;
    [SerializeField] private AudioClip idleSoundPath;
    [SerializeField] private AudioClip patrolSoundPath;
    [SerializeField] private AudioClip searchSoundPath;
    [SerializeField] private AudioClip chaseSoundPath;
    [SerializeField] private AudioClip attackSoundPath;
    [SerializeField] private AudioClip dieSoundPath;
    [SerializeField] private AudioClip killSoundPath;
    [SerializeField] private float pitch = 1.0f;
    [SerializeField] private float pitchVariance = 0.1f;

    [Header("Ragdoll and Holdable")]
    [SerializeField] private float deathAnimTime = 2.0f;
    [SerializeField] private string holdableTag = "HoldableItem";


    void Start()
    {
        // Animator
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");
        obstacleMask = LayerMask.GetMask("Obstacle");
        nav = GetComponent<NavMeshAgent>();
        _rayOrigin = transform.Find("Raycast_Position").transform;

        questionMark = transform.Find("Question_Mark").gameObject;
        exclamationMark = transform.Find("Exclamation_Mark").gameObject;

        audioSource = GetComponent<AudioSource>();

        // For fixed patrolling
        currentDest = destinations[patrolIndex];

        // For random patrolling
        // randNum = Random.Range(0, destinations.Count);
        // currentDest = destinations[randNum];

        SetStateToPatrol();
    }

    void Update()
    {
        if (currentState == AIState.Dead) { return; }

        Vector3 playerTarget = (player.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

        // If enemy sees player,
        if (PlayerInSight(playerTarget, distanceToTarget)) {
            // If player is in attack range,
            if (distanceToTarget <= attackDistance) {
                SetStateToAttack();
            } else {
                SetStateToChase();
            }
        }
        // If enemy does not see player,
        else {
            // But is within attack range.
            if (distanceToTarget <= attackDistance) {
                SetStateToAttack();
            }
            // If the enemy hears a sound, search for sound.
            else if (currentState != AIState.Chase && currentState != AIState.Attack && HearPlayer(distanceToTarget)) {
                SetStateToSearch();
            }
        }

        // Idle State.
        if (currentState == AIState.Idle) {
            Idle();
            // If the enemy has idle for this long,
            idleTime -= Time.deltaTime;
            if (idleTime < 0) {
                SetStateToPatrol();
            }
        }

        // Patrol State.
        if (currentState == AIState.Patrol) {
            Patrol();
            // If enemy has reached the patrol point.
            var destinationPos = new Vector3(nav.destination.x, 0, nav.destination.z);
            var currentPos = new Vector3(transform.position.x, 0, transform.position.z);
            if (Vector3.Distance(destinationPos, currentPos) <= nav.stoppingDistance) {
                SetStateToIdle();
                NextPatrolPoint();
            }
        }

        // Search State.
        if (currentState == AIState.Search) {
            Search();
            // If enemy has reached the position where they last heard a sound.
            var destinationPos = new Vector3(nav.destination.x, 0, nav.destination.z);
            var currentPos = new Vector3(transform.position.x, 0, transform.position.z);
            if (Vector3.Distance(destinationPos, currentPos) <= nav.stoppingDistance) {
                questionMark.SetActive(false);
                SetStateToIdle();
            }

            // If the enemy has spent too much time searching.
            SearchTime -= Time.deltaTime;
            if (SearchTime < 0) {
                questionMark.SetActive(false);
                SetStateToIdle();
            }
        }

        // Chase State.
        if (currentState == AIState.Chase) {
            Chase(distanceToTarget);
            // If the player has exited the enemy's line of sight for chaseTime.
            chaseTime -= Time.deltaTime;
            if (chaseTime < 0) {
                exclamationMark.SetActive(false);
                SetStateToIdle();
            }
        }

        // Attack State
        if (currentState == AIState.Attack) {
            // Stop moving and turn towards player.
            FocusOnPlayer(playerTarget);

            // Attack at a rate of attack interval.
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0) {
                Attack();
            }
        }
    }

    // Enemy Vision.
    private bool PlayerInSight(Vector3 playerTarget, float distanceToTarget) {
        // Debug.DrawRay(_rayOrigin.position, playerTarget * distanceToTarget, Color.red);

        // If player is in view range,
        if (distanceToTarget <= viewRadius) {
            // If player is in the current field of view,
            if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2) {
                // If there are no obstacles between player and enemy,
                if (Physics.Raycast(_rayOrigin.position, playerTarget, distanceToTarget, obstacleMask) == false) {
                    // The enemy can see the player.
                    // Debug.Log("I see you!");
                    return true;
                }
            }
        }
        return false;
    }

    // Enemy Hearing.
    private bool HearPlayer(float distanceToTarget) {
        // If the player is in hearing range,
        if (distanceToTarget <= hearRadius) {
            // If the player is being loud (i.e. not crouching while moving)
            if (player.GetComponent<FirstPersonControl>().GetCurrentSpeed() >= hearLoudness) {
                // The enemy can hear the player.
                // Debug.Log("I hear you!");
                return true;
            }
        }
        return false;
    }

    // Change to idle state.
    private void SetStateToIdle() {
        idleTime = idleDuration;
        currentState = AIState.Idle;

        anim.ResetTrigger(patrolAnimation.name);
        anim.ResetTrigger(searchAnimation.name);
        anim.ResetTrigger(chaseAnimation.name);
        anim.ResetTrigger(attackAnimation.name);
        anim.SetTrigger(idleAnimation.name);
    }

    // Change to patrol state.
    private void SetStateToPatrol() {
        currentState = AIState.Patrol;

        anim.ResetTrigger(idleAnimation.name);
        anim.ResetTrigger(searchAnimation.name);
        anim.ResetTrigger(chaseAnimation.name);
        anim.ResetTrigger(attackAnimation.name);
        anim.SetTrigger(patrolAnimation.name);
    }

    // Change to search state.
    private void SetStateToSearch() {
        SearchTime = searchDuration;

        soundPos = player.transform.position;
        currentState = AIState.Search;

        // Suspicion Marks.
        exclamationMark.SetActive(false);
        questionMark.SetActive(true);

        anim.ResetTrigger(idleAnimation.name);
        anim.ResetTrigger(patrolAnimation.name);
        anim.ResetTrigger(chaseAnimation.name);
        anim.ResetTrigger(attackAnimation.name);
        anim.SetTrigger(searchAnimation.name);
    }

    // Change to chase state.
    private void SetStateToChase() {
        chaseTime = Random.Range(minChaseTime, maxChaseTime);
        currentState = AIState.Chase;

        // Suspicion Marks.
        exclamationMark.SetActive(true);
        questionMark.SetActive(false);

        anim.ResetTrigger(idleAnimation.name);
        anim.ResetTrigger(patrolAnimation.name);
        anim.ResetTrigger(searchAnimation.name);
        anim.ResetTrigger(attackAnimation.name);
        anim.SetTrigger(chaseAnimation.name);
    }

    // Change to attack state.
    private void SetStateToAttack() {
        if (attackTimer < 0) {
            attackTimer = AttackInterval;
        }
        rotationProgress = 0;
        currentState = AIState.Attack;
    }

    // Change the current audio file
    private IEnumerator ChangeSoundTo(AudioClip clip) {
        // Wait until the current clip has finished playing.
        while (audioSource.isPlaying) {
            yield return null;
        }

        audioSource.clip = clip;

        // Randomise the pitch of the sound to make it sound more natural.
        audioSource.pitch = Random.Range(pitch - pitchVariance, pitch + pitchVariance);
        
        audioSource.Play();
    }

    // Look around before resuming patrolling.
    private void Idle() {
        StartCoroutine(ChangeSoundTo(idleSoundPath));
        nav.destination = transform.position;
        nav.speed = 0;
    }

    private void Patrol() {
        StartCoroutine(ChangeSoundTo(patrolSoundPath));
        
        nav.destination = currentDest.position;
        nav.speed = walkSpeed;
    }

    // Patrolling through fixed points.
    // If at end of patrol points, reset.
    private void NextPatrolPoint() {
        patrolIndex++;
        if (patrolIndex >= destinations.Count) {
            patrolIndex = 0;
        }
        currentDest = destinations[patrolIndex];
    }

    private void Search() {
        StartCoroutine(ChangeSoundTo(searchSoundPath));
        
        nav.destination = soundPos;
        nav.speed = walkSpeed;
    }

    private void Chase(float distanceToTarget) {
        StartCoroutine(ChangeSoundTo(chaseSoundPath));
        
        if (distanceToTarget <= attackDistance) {
            nav.destination = transform.position;
            nav.speed = 0;
        } else {
            nav.destination = player.transform.position;
            nav.speed = chaseSpeed;
        }
    }

    // If within attack range, stop moving and rotate towards the player.
    private void FocusOnPlayer(Vector3 playerTarget) {
        StartCoroutine(ChangeSoundTo(attackSoundPath));

        // Stop moving.
        nav.destination = transform.position;
        nav.speed = 0;
        
        // What rotation to face.
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(playerTarget.x, 0, playerTarget.z));
        
        // Increment the time count based on the rotation speed and deltaTime.
        rotationProgress += Time.deltaTime * attackRotationRate;

        // Ensure rotationProgress is clamped between 0 and 1.
        rotationProgress = Mathf.Clamp01(rotationProgress);

        // Smoothly rotate from current rotation to this rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationProgress);

        if (rotationProgress >= 1) {
            rotationProgress = 0;
        }
    }

    // Note: changeSoundTo(attackSoundPath) is in FocusOnPlayer.
    private void Attack() {
        // If the enemy's weapon has hit the player, reduce the player's HP.
        // This can be checked with a OnCollision/Trigger Enter, or sth else.

        anim.ResetTrigger(idleAnimation.name);
        anim.ResetTrigger(patrolAnimation.name);
        anim.ResetTrigger(searchAnimation.name);
        anim.ResetTrigger(chaseAnimation.name);
        anim.SetTrigger(attackAnimation.name);

        Debug.Log("Attack");
    }


    // Returns true if the enemy can be killed, false otherwise.
    public bool IsKillable() {
        if (currentState == AIState.Idle || currentState == AIState.Patrol || currentState == AIState.Search) {
            return true;
        }
        return false;
    }

    // This triggers the death animation, removes this script so enemy cannot move.
    public IEnumerator Die() {
        if (currentState != AIState.Dead) {
            questionMark.SetActive(false);
            exclamationMark.SetActive(false);
            StartCoroutine(ChangeSoundTo(dieSoundPath));

            // To stop movement.
            Destroy(gameObject.GetComponent<BoxCollider>());
            Destroy(nav);

            anim.ResetTrigger(idleAnimation.name);
            anim.ResetTrigger(patrolAnimation.name);
            anim.ResetTrigger(searchAnimation.name);
            anim.ResetTrigger(chaseAnimation.name);
            anim.ResetTrigger(attackAnimation.name);
            anim.SetTrigger(dieAnimation.name);

            currentState = AIState.Dead;

            // Add this to update instead.
            StartCoroutine(Die());
        }

        // Wait until the animation has finished playing.
        yield return new WaitForSeconds(deathAnimTime);

        Destroy(anim);

        // Set the body parts of the enemy to holdable.
        ChangeTagRecursively(transform, holdableTag);

        // Change layer to Dead for magic circle to work.

        // To start ragdoll, need a rigidbody.
        gameObject.AddComponent(typeof(Rigidbody));

        // Destroy this script.
        Destroy(this);
    }

    private void ChangeTagRecursively(Transform parent, string tag) {
        // Change the tag of the current object
        parent.gameObject.tag = tag;

        // Iterate through all children of the current object
        foreach (Transform child in parent) {
            // Recursively change the tag for each child
            ChangeTagRecursively(child, tag);
        }
    }

    // This should be called by the player, or a health manager.
    public void Kill() {
        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
        StartCoroutine(ChangeSoundTo(killSoundPath));

        // The player is set to inactive, so that the player cannot be moved.
        // This will remove the player from the scene but not destroy it.
        // Can do whichever.
        // player.SetActive(false);

        anim.ResetTrigger(idleAnimation.name);
        anim.ResetTrigger(patrolAnimation.name);
        anim.ResetTrigger(searchAnimation.name);
        anim.ResetTrigger(chaseAnimation.name);
        anim.ResetTrigger(attackAnimation.name);
        // anim.SetTrigger(killAnimation.name);

        // SceneManager.LoadScene(deathScene);
    }
}