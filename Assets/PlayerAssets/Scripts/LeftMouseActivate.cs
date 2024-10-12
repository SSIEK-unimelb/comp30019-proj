using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMouseActivate : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    private FirstPersonControl fpc;
    private HoldingScript holdScript;
    private Animator animator;
    private ItemSwitcher itemSwitcher;
    private SoundManager soundManager;
    [SerializeField] private AudioClip stabSound;
    [SerializeField] private AudioClip activateEyeSound;
    [SerializeField] private AudioClip deactivateEyeSound;


    private EyeScript eyeController;
    private bool isThrowingEye;
    private bool isActivated = false;
    private EyeScript EyeScript;

    [SerializeField] Transform prefabParent;
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        fpc = player.GetComponent<FirstPersonControl>();
        holdScript = player.GetComponentInChildren<HoldingScript>();
        animator = GetComponent<Animator>();
        itemSwitcher = player.GetComponentInChildren<ItemSwitcher>();
        eyeController = player.GetComponentInChildren<EyeScript>();
        isThrowingEye = false;

        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isThrowingEye && Input.GetKeyDown(KeyCode.Mouse0)) {
            // Switch cam
            fpc.toggleCamera();
            EyeScript.ActivateEye();

            if (!isActivated) {
                soundManager.PlaySoundEffect(activateEyeSound, 0.2f);
                isActivated = true;
            } else {
                soundManager.PlaySoundEffect(deactivateEyeSound, 0.2f);
                isActivated = false;
            }

            // Reinstantiate eye arm prefab if done throwing
            if (!EyeScript.isEyeActive())
            {
                itemSwitcher.SwitchItem(0);
                itemSwitcher.SwitchItem(3);
            }
            return;
        }
        // also make sure that the object this is attached to is the active prefab arm
        if (transform.parent != null && itemSwitcher.currentItem.Equals(prefabParent.gameObject) && Input.GetKeyDown(KeyCode.Mouse0) && !holdScript.isHolding())
        {
            WeaponType weaponType = transform.GetComponent<WeaponType>();

            if (weaponType == null) {
                Debug.LogError("WeaponType is missing from weapon");
            }

            if (itemSwitcher == null || itemSwitcher.currentItem == null) {
                Debug.LogError("ItemSwitcher or currentItem is null");
            }

            // do action - stabbing animation
            if (weaponType.currentWeapon == WeaponType.Weapon.Knife)
            {
                AnimatorClipInfo[] animatorinfo;
                animatorinfo = this.animator.GetCurrentAnimatorClipInfo(0);
                Debug.Log(animatorinfo[0].clip.name);
                if (animatorinfo[0].clip.name.Equals("IdleKnife"))
                {
                    
                    animator.SetTrigger("doStabAnim");
                    //animator.Play("LeftClick");

                    soundManager.PlaySoundEffect(stabSound, 0.2f);
                }
            }
            else if (weaponType.currentWeapon == WeaponType.Weapon.Crossbow)
            {
                // insert crossbow firing logic here
                FireArrow arrowShooter = transform.GetComponent<FireArrow>();
                arrowShooter.Fire();
            }
            else if (weaponType.currentWeapon == WeaponType.Weapon.Eye) 
            {
                if (!isThrowingEye)
                {
                    // Eye throw logic here
                    Rigidbody body = transform.GetComponent<Rigidbody>();
                    body.isKinematic = false;
                    body.freezeRotation = true;
                    body.AddForce(transform.forward * 20, ForceMode.Impulse);

                    transform.parent = null;
                    prefabParent = transform.parent;
                    isThrowingEye = true;
                    EyeScript = transform.GetComponent<EyeScript>();
                }
            }

        }
    }
    
}
