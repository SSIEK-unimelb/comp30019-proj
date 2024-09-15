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
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        fpc = player.GetComponent<FirstPersonControl>();
        holdScript = player.GetComponentInChildren<HoldingScript>();
        animator = GetComponent<Animator>();
        itemSwitcher = player.GetComponentInChildren<ItemSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {
        // also make sure that the object this is attached to is the active prefab arm
        if (itemSwitcher.currentItem.Equals(transform.parent.gameObject) && Input.GetKeyDown(KeyCode.Mouse0) && !holdScript.isHolding())
        {
            WeaponType weaponType = transform.GetComponent<WeaponType>();

            if (weaponType == null) {
                Debug.LogError("WeaponType is missing from weapon");
            }

            if (itemSwitcher == null || itemSwitcher.currentItem == null) {
                Debug.LogError("ItemSwitcher or currentItem is null");
            }

            // do action - stabbing animation
            if (weaponType.currentWeapon == WeaponType.Weapon.Knife) {
                AnimatorClipInfo[] animatorinfo;
                animatorinfo = this.animator.GetCurrentAnimatorClipInfo(0);
                if ((animatorinfo.Length == 0) || animatorinfo[0].clip.name.Equals("Reset"))
                {
                    animator.Play("LeftClick");
                }
            }
            else if (weaponType.currentWeapon == WeaponType.Weapon.Crossbow) {
                // insert crossbow firing logic here
                FireArrow arrowShooter = transform.GetComponent<FireArrow>();
                arrowShooter.Fire();
            }
        }
    }
    
}
