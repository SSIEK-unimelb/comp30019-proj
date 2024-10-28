using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


// Shoutout to Comp-3 Interactive's series on first person controllers

public class FirstPersonControl : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    private SoundManager soundManager;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip landAudio;

    public bool CanMove { get; private set; } = true;
    [SerializeField] private int InteractibleLayer = 6;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey) && !holder.isHolding();

    private bool shouldJump => characterController.isGrounded && Input.GetKeyDown(jumpKey) && !holder.isHolding();
    private bool shouldCrouch => !isInCrouchAnimation && (canMidairCrouch? true : characterController.isGrounded)  && Input.GetKeyDown(crouchKey);
    //private bool shouldCrouch => !isInCrouchAnimation && (canMidairCrouch ? true : characterController.isGrounded) && Input.GetKey(crouchKey);


    private SoundMaker soundMaker;
    private float soundMakerInterval = 0.2f;
    private float currentSoundMakerTime = 0.2f;
    private bool playerMadeSound = false;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float crouchSpeed = 2.0f;
    private float currentSpeed;

    [Header("Movement Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool canMidairCrouch = false;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Looking")]
    [SerializeField] private float lookSpeedX = 2.0f;
    [SerializeField] private float lookSpeedY = 2.0f;
    [SerializeField] private float aboveLookClamp = 90.0f;
    [SerializeField] private float belowLookClamp = 90.0f;

    [Header("Jump")]
    [SerializeField] private float jumpStrength = 8.0f;
    [SerializeField] private float gravity = 9.81f;
    private bool wasInAir = false;
    private bool hasLanded => characterController.isGrounded && wasInAir;
    // Prevent false isGrounded during crouch transition.
    [SerializeField] private float crouchTransitionTimeOffset = 0.2f;
    private float timeSinceCrouchTransition;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchTransitionTime = 0.25f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standCenter = new Vector3(0, 0, 0);
    private bool isCrouched;
    private bool isInCrouchAnimation;

    [Header("Headbob")]
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float walkBobIntensity = 0.05f;
    [SerializeField] private float crouchBobSpeed = 6f;
    [SerializeField] private float crouchBobIntensity = 0.025f;
    [SerializeField] private float sprintBobSpeed = 12f;
    [SerializeField] private float sprintBobIntensity = 0.1f;
    private float defaultYPos = 0;
    private float bobTimer;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactRayIntersect = default;
    [SerializeField] private float interactDistance = default;
    [SerializeField] private LayerMask interactLayerMask = default;
    private Interactible currentInteractible;

    [SerializeField] private GameObject interactTextMesh;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDir;
    private Vector2 inputDir;
    private float rotationX = 0;

    private HoldingScript holder;

    private GameObject cameraHierarchy;
    private bool isCameraActive;

    private ItemSwitcher itemSwitcher;

    // private bool crouchToggled = false;

    private void Awake()
    {
        soundMaker = GetComponent<SoundMaker>();
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        holder = GetComponentInChildren<HoldingScript>();
        cameraHierarchy = transform.GetChild(1).gameObject;
        isCameraActive = true;

        timeSinceCrouchTransition = crouchTransitionTime + crouchTransitionTimeOffset;

        defaultYPos = playerCamera.transform.localPosition.y;
        itemSwitcher = GetComponentInChildren<ItemSwitcher>();

        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }
    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        
        SetInteractText(false);
        if (CanMove)
        {
            if (isCameraActive)
            {
                ValidateMovement();
                RegisterMouseMovement();
                if (canJump) RegisterJump();
                if (canCrouch) RegisterCrouch();
                if (canUseHeadBob) DoHeadBob();
                if (canInteract)
                {
                    CheckInteraction();
                    RegisterInteractInput();
                }
            }
            else { 
                // Reject all movement except gravity
                moveDir = new Vector3(0f, moveDir.y, 0f);
            }

            ApplyMovement();
            MakeSound();

        }
    }

    private void ValidateMovement()
    {
        if (characterController.isGrounded)
        {
            currentSpeed = isCrouched ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;
        }
        inputDir = new Vector2(currentSpeed * Input.GetAxis("Vertical"), currentSpeed * Input.GetAxis("Horizontal"));
        float moveDirY = moveDir.y;
        moveDir = (transform.TransformDirection(Vector3.forward) * inputDir.x) + (transform.TransformDirection(Vector3.right) * inputDir.y);
        moveDir.y = moveDirY;
    }

    private void RegisterMouseMovement() 
    {
        // look vertically
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -aboveLookClamp, belowLookClamp);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // horizontal looking moves the character
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);

    }
     
    private void ApplyMovement()
    {
        if (!characterController.isGrounded) { 
            moveDir.y -= gravity * Time.deltaTime;
            // timeSinceStandUp prevents false isGrounded during crouch transition.
            timeSinceCrouchTransition -= Time.deltaTime;
            if (timeSinceCrouchTransition <= 0) {
                wasInAir = true;
            }
        } else {
            timeSinceCrouchTransition = crouchTransitionTime + crouchTransitionTimeOffset;
        }
        characterController.Move(moveDir * Time.deltaTime);
    }


    private void RegisterJump() 
    {
        if (shouldJump) {
            moveDir.y = jumpStrength;
            soundManager.PlaySoundEffect(jumpAudio, 1.0f);
        }
    }

    private void RegisterCrouch() 
    {
        /**
        if (shouldCrouch) {
            crouchToggled = true;
        }
        if (shouldCrouch || isCrouched && !isInCrouchAnimation && (!Input.GetKey(crouchKey) || crouchToggled))
        {
            StartCoroutine(CrouchStandTransition());
        }
        **/
        if (shouldCrouch || (isCrouched && isSprinting))
        {
            StartCoroutine(CrouchStandTransition());
        }
    }

    private IEnumerator CrouchStandTransition() 
    {
        if (isCrouched && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }
        isInCrouchAnimation = true;

        float elapsedTime = 0;
        float targetHeight = isCrouched ? standHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouched ? standCenter : crouchCenter;
        Vector3 currentCenter = characterController.center;

        isCrouched = !isCrouched;

        while (elapsedTime < crouchTransitionTime) 
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, elapsedTime/crouchTransitionTime);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, elapsedTime/crouchTransitionTime);
            elapsedTime += Time.deltaTime;  
            yield return null;
        }
        
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isInCrouchAnimation = false;
        // crouchToggled = false;
    }

    private void CheckInteraction() 
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactRayIntersect), out RaycastHit hit, interactDistance, ~layerMask)) 
        { 
            if(hit.collider.gameObject.layer == InteractibleLayer && 
                (currentInteractible == null || hit.collider.gameObject.GetInstanceID() != currentInteractible.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractible);
                if (currentInteractible != null) { 
                    currentInteractible.OnFocus();
                }
            }
        }
        else if (currentInteractible != null) 
        {
            currentInteractible.OnLoseFocus();
            currentInteractible = null;
        }   
    }

    private void RegisterInteractInput() 
    {
        if (Input.GetKeyDown(interactKey) && currentInteractible != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactRayIntersect),
            out RaycastHit hit, interactDistance, interactLayerMask))
        {
            currentInteractible.OnInteract();
        }
    }

    public void MakeSound() {
        // If the player has landed after jumping or falling.
        if (hasLanded) {
            wasInAir = false;
            playerMadeSound = true;
            soundManager.PlaySoundEffect(landAudio, 1.0f);
        }

        // Do not check this every frame.
        currentSoundMakerTime -= Time.deltaTime;
        if (currentSoundMakerTime > 0) return;
        currentSoundMakerTime = soundMakerInterval;

        // If the player is moving at walk speed or faster.
        if (inputDir != Vector2.zero && currentSpeed >= walkSpeed) {
            soundMaker.MakeSound();
        }

        // If the player has landed after jumping or falling.
        if (playerMadeSound) {
            playerMadeSound = false;
            soundMaker.MakeSound();
        }
    }

    public void toggleCamera() {

        isCameraActive = !isCameraActive;

        if (!isCameraActive)
        {
            Transform ap;
            foreach (Transform child in playerCamera.transform)
            {
                if (itemSwitcher.currentItem.Equals(child.gameObject))
                {
                    // found arm prefab
                    ap = child;
                    print(playerCamera.transform.parent);
                    ap.parent = playerCamera.transform.parent;

                    break;
                }
            }
            
        }
        cameraHierarchy.SetActive(isCameraActive);
    }

    public void DoHeadBob() 
    {
        if (characterController.isGrounded) 
        {
            bobTimer += Time.deltaTime * (isCrouched ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            if (Mathf.Abs(new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude) > 0.1f 
                || (Mathf.Abs(defaultYPos - playerCamera.transform.localPosition.y) > 0.005f))
            {
                playerCamera.transform.localPosition = new Vector3(
                        playerCamera.transform.localPosition.x,
                        defaultYPos + Mathf.Sin(bobTimer) * (isCrouched ? crouchBobIntensity : isSprinting ? sprintBobIntensity : walkBobIntensity),
                        playerCamera.transform.localPosition.z);
            }
            else if (defaultYPos != playerCamera.transform.localPosition.y)
            {
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos, playerCamera.transform.localPosition.z);
            }
        }
    }

    public void SetInteractText(bool val) {
        interactTextMesh.SetActive(val);
    }
}
