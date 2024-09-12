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
    private Transform rayOrigin;
    [SerializeField] private GameObject backHitPoint;

    [Header("Patrol variables")]
    [SerializeField] private List<Transform> patrolPoints;
    private Transform currentPatrolPoint;
    private int patrolIndex = 0;
    private Vector3 lastHeardPos;
    
    // AI States
    private enum State { Idle, Patrol, Chase, Search, Attack, Dead }
    private State currentState;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float rotationTime = 0.3f;
    private float yVelocity;

    [Header("Detection")]
    [SerializeField] private float viewRadius = 15f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float hearRadius = 10f;
    [SerializeField] private float hearLoudness = 3f;   // Match to player's walkSpeed.
    [SerializeField] private float attackDistance = 2f;

    [Header("Timing")]
    [SerializeField] private float idleDuration = 5f;
    private float idleTime;
    [SerializeField] private float searchDuration = 10f;
    private float SearchTime;
    [SerializeField] private float waitTimeToChase = 1f;
    private float currentWaitTimeToChase;
    [SerializeField] private float minChaseTime = 10f;
    [SerializeField] private float maxChaseTime = 15f;
    private float chaseTime;

    [Header("Suspicion Icons")]
    private GameObject questionMark;
    private GameObject exclamationMark;

    [Header("Animations and Sounds")]
    private Animator animator;
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip patrolAnimation;
    [SerializeField] private AnimationClip searchAnimation;
    [SerializeField] private AnimationClip chaseAnimation;
    [SerializeField] private AnimationClip attackAnimation;
    // [SerializeField] private AnimationClip dieAnimation;
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


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        obstacleMask = LayerMask.GetMask("Obstacle");
        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
        rayOrigin = transform.Find("Raycast_Position").transform;

        questionMark = transform.Find("Question_Mark").gameObject;
        exclamationMark = transform.Find("Exclamation_Mark").gameObject;
        audioSource = GetComponent<AudioSource>();

        // For fixed patrolling
        currentPatrolPoint = patrolPoints[patrolIndex];

        // For random patrolling
        // randNum = Random.Range(0, destinations.Count);
        // currentDest = destinations[randNum];

        SetState(State.Patrol);
        currentWaitTimeToChase = waitTimeToChase;
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        Vector3 playerDirection = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // If enemy sees player,
        if (CanSeePlayer(playerDirection, distanceToPlayer)) {
            // Identifying player takes some time
            currentWaitTimeToChase -= Time.deltaTime;
            if (currentWaitTimeToChase > 0) {
                SetState(State.Search);
            } else {
                // If player is in attack range,
                if (distanceToPlayer <= attackDistance) {
                    SetState(State.Attack);
                } else {
                    SetState(State.Chase);
                }
            }
        }
        // If enemy does not see player, but hears a sound, search for sound.
        else if (currentState != State.Chase && currentState != State.Attack && CanHearPlayer(distanceToPlayer)) {
            SetState(State.Search);
        }

        // State Handling
        switch (currentState) {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Patrol:
                HandlePatrolState();
                break;
            case State.Search:
                HandleSearchState();
                break;
            case State.Chase:
                HandleChaseState(playerDirection);
                break;
            case State.Attack:
                HandleAttackState(playerDirection);
                break;
        }
    }

    // Enemy Vision.
    private bool CanSeePlayer(Vector3 playerDirection, float distanceToPlayer) {
        // Debug.DrawRay(_rayOrigin.position, playerTarget * distanceToTarget, Color.red);
        // If player is in view range,
        if (distanceToPlayer <= viewRadius) {
            // If player is in the current field of view,
            if (Vector3.Angle(transform.forward, playerDirection) < viewAngle / 2) {
                // If there are no obstacles between player and enemy,
                if (Physics.Raycast(rayOrigin.position, playerDirection, distanceToPlayer, obstacleMask) == false) {
                    // The enemy can see the player.
                    // Debug.Log("I see you!");
                    return true;
                }
            }
        }
        return false;
    }

    // Enemy Hearing.
    private bool CanHearPlayer(float distanceToPlayer) {
        // If the player is in hearing range,
        if (distanceToPlayer <= hearRadius) {
            // If the player is being loud (i.e. not crouching while moving)
            if (player.GetComponent<FirstPersonControl>().GetCurrentSpeed() >= hearLoudness) {
                // The enemy can hear the player.
                // Debug.Log("I hear you!");
                return true;
            }
        }
        return false;
    }

    private void SetState(State newState) {
        currentState = newState;

        animator.ResetTrigger(idleAnimation.name);
        animator.ResetTrigger(patrolAnimation.name);
        animator.ResetTrigger(searchAnimation.name);
        animator.ResetTrigger(chaseAnimation.name);
        animator.ResetTrigger(attackAnimation.name);

        switch (newState) {
            case State.Idle:
                idleTime = idleDuration;
                animator.SetTrigger(idleAnimation.name);
                // Set the currentWaitTimeToChase back to initial time
                currentWaitTimeToChase = waitTimeToChase;
                break;

            case State.Patrol:
                animator.SetTrigger(patrolAnimation.name);
                break;

            case State.Search:
                SearchTime = searchDuration;
                lastHeardPos = player.transform.position;
                exclamationMark.SetActive(false);
                questionMark.SetActive(true);
                animator.SetTrigger(searchAnimation.name);
                break;

            case State.Chase:
                chaseTime = Random.Range(minChaseTime, maxChaseTime);
                exclamationMark.SetActive(true);
                questionMark.SetActive(false);
                animator.SetTrigger(chaseAnimation.name);
                break;

            case State.Attack:
                animator.SetTrigger(attackAnimation.name);
                break;
        }
    }

    // Look around before resuming patrolling.
    private void HandleIdleState() {
        StartCoroutine(ChangeSoundTo(idleSoundPath));
        nav.destination = transform.position;
        nav.speed = 0;

        // If the enemy has idled for this long,
        idleTime -= Time.deltaTime;
        if (idleTime < 0) SetState(State.Patrol);
    }

    private void HandlePatrolState() {
        StartCoroutine(ChangeSoundTo(patrolSoundPath));
        
        nav.destination = currentPatrolPoint.position;
        nav.speed = walkSpeed;

        // Rotate smoothly towards movement direction
        if (nav.velocity != Vector3.zero) {
            float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, Quaternion.LookRotation(nav.velocity).eulerAngles.y, ref yVelocity, rotationTime);
            nav.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
        }
        
        // y is set to 0 so that the patrol point can be at any height.
        var destinationPos = new Vector3(nav.destination.x, 0, nav.destination.z);
        var currentPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(destinationPos, currentPos) <= nav.stoppingDistance) {
            SetState(State.Idle);
            NextPatrolPoint();
        }
    }

    // Patrolling through fixed points.
    // If at end of patrol points, reset.
    private void NextPatrolPoint() {
        patrolIndex++;
        if (patrolIndex >= patrolPoints.Count) {
            patrolIndex = 0;
        }
        currentPatrolPoint = patrolPoints[patrolIndex];
    }

    private void HandleSearchState() {
        StartCoroutine(ChangeSoundTo(searchSoundPath));
        
        nav.destination = lastHeardPos;
        nav.speed = walkSpeed;

        // Rotate smoothly towards movement direction
        if (nav.velocity != Vector3.zero) {
            float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, Quaternion.LookRotation(nav.velocity).eulerAngles.y, ref yVelocity, rotationTime);
            nav.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
        }

        // y is set to 0 so that the sound position can be at any height.
        var destinationPos = new Vector3(nav.destination.x, 0, nav.destination.z);
        var currentPos = new Vector3(transform.position.x, 0, transform.position.z);

        // If enemy has reached the position where they last heard a sound.
        if (Vector3.Distance(destinationPos, currentPos) <= nav.stoppingDistance) {
            questionMark.SetActive(false);
            SetState(State.Idle);
        }

        // If the enemy has spent too much time searching.
        SearchTime -= Time.deltaTime;
        if (SearchTime < 0) {
            questionMark.SetActive(false);
            SetState(State.Idle);
        }
    }

    private void HandleChaseState(Vector3 playerDirection) {
        StartCoroutine(ChangeSoundTo(chaseSoundPath));

        nav.destination = player.transform.position;
        nav.speed = chaseSpeed;

        // If moving,
        if (nav.velocity != Vector3.zero) {
            // Rotate smoothly towards movement direction
            float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, Quaternion.LookRotation(nav.velocity).eulerAngles.y, ref yVelocity, rotationTime);
            nav.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
        } else {
            // Rotate towards player
            Vector3 playerDirectionWithoutY = new Vector3(playerDirection.x, 0, playerDirection.z);
            float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, Quaternion.LookRotation(playerDirectionWithoutY).eulerAngles.y, ref yVelocity, rotationTime);
            nav.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
        }

        // If the player has exited the enemy's line of sight for chaseTime.
        chaseTime -= Time.deltaTime;
        if (chaseTime < 0) {
            exclamationMark.SetActive(false);
            SetState(State.Idle);
        }
    }

    // If within attack range, stop moving.
    private void HandleAttackState(Vector3 playerDirection) {
        StartCoroutine(ChangeSoundTo(attackSoundPath));
        nav.destination = player.transform.position;
        nav.speed = 0;
        
        // Rotate towards player
        Vector3 playerDirectionWithoutY = new Vector3(playerDirection.x, 0, playerDirection.z);
        float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, Quaternion.LookRotation(playerDirectionWithoutY).eulerAngles.y, ref yVelocity, rotationTime);
        nav.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
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

    // Returns true if the enemy can be killed, false otherwise.
    public bool IsKillable() {
        return currentState == State.Idle || currentState == State.Patrol || currentState == State.Search;
    }

    // This triggers the death animation, removes this script so enemy cannot move.
    public void Die() {
        currentState = State.Dead;

        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
        StartCoroutine(ChangeSoundTo(dieSoundPath));

        // To stop movement.
        Destroy(gameObject.GetComponent<BoxCollider>());
        Destroy(nav);

        // To start ragdoll, need a rigidbody.
        gameObject.AddComponent(typeof(Rigidbody));

        animator.ResetTrigger(idleAnimation.name);
        animator.ResetTrigger(patrolAnimation.name);
        animator.ResetTrigger(searchAnimation.name);
        animator.ResetTrigger(chaseAnimation.name);
        animator.ResetTrigger(attackAnimation.name);
        Destroy(animator);

        // Destroy this script.
        Destroy(this);
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

        animator.ResetTrigger(idleAnimation.name);
        animator.ResetTrigger(patrolAnimation.name);
        animator.ResetTrigger(searchAnimation.name);
        animator.ResetTrigger(chaseAnimation.name);
        animator.ResetTrigger(attackAnimation.name);
        // anim.SetTrigger(killAnimation.name);

        // SceneManager.LoadScene(deathScene);
    }
}