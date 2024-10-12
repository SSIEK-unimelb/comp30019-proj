
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Sourced from with permission from Code Monkey : https://www.youtube.com/watch?v=2IhzPTS4av4

public class HoldingScript : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip throwSound;

    public GameObject player;
    public Transform holdPos;
    
    [SerializeField ]public float throwForce = 1000f; 
    [SerializeField] public float pickUpRange = 5f; 
    private GameObject heldObj; 
    private Rigidbody heldObjRb; 
    private bool canDrop = true; 
    private int LayerNumber;
    [SerializeField] private float pickupForce = 150.0f;

    [SerializeField] private string pickupTag = "HoldableItem";
    [SerializeField] private Vector3 interactRayIntersect = default;
    private Camera playerCamera;
    private Transform objectParent;
    private ItemSwitcher itemSwitcher;

    [SerializeField] int layerToIgnore = 12;
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("Holdable"); 
        playerCamera = GetComponent<Camera>();
        itemSwitcher = GetComponent<ItemSwitcher>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) 
        {
            if (heldObj == null) //if currently not holding anything
            {
                //perform raycast to check if player is looking at object within pickuprange
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.ViewportPointToRay(interactRayIntersect), out hit, pickUpRange, ~layerToIgnore))
                {
                    print("HIT");
                    print(hit.collider.gameObject.name);
                    print(hit.transform.gameObject.tag == pickupTag);
                    //make sure pickup tag is attached
                    if (hit.transform.gameObject.tag == pickupTag)
                    {
                        //pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                        itemSwitcher.SwitchToHoldArms();
                        itemSwitcher.canSwitch = false;
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                    itemSwitcher.canSwitch = true;
                    itemSwitcher.SwitchItem(0);
                }
            }
        }
        if (heldObj != null) //if player is holding object
        {
            // itemSwitcher.SwitchToHoldArms();
            MoveObject(); //keep object position at holdPos
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true) 
            {
                StopClipping();
                ThrowObject();
                itemSwitcher.canSwitch = true;
                itemSwitcher.SwitchItem(0);
            }

        }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        // If the object cannot be held, do not hold
        HoldStatus holdStatus = pickUpObj.GetComponentInParent<HoldStatus>();
        if (holdStatus != null && !holdStatus.CanBeHeld) {
            return;
        }
        GameObject pickedObj = pickUpObj;
        ParentReference parentRef = pickUpObj.GetComponentInParent<ParentReference>();
        if (parentRef != null)
        {
            pickedObj = parentRef.getTransform().gameObject;
            print("Got parent from parent ref!");
            print(pickedObj.name);
        }
        if (pickedObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            soundManager.PlaySoundEffect(pickUpSound, 1.0f);

            heldObj = pickedObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            if (heldObj.GetComponentInParent<HoldStatus>()) heldObj.GetComponentInParent<HoldStatus>().IsHeld = true;
            heldObjRb = heldObj.GetComponent<Rigidbody>(); //assign Rigidbody
            //heldObjRb.isKinematic = true;

            heldObjRb.useGravity = false;
            heldObjRb.drag = 10;
            heldObjRb.constraints = RigidbodyConstraints.FreezeRotation;

            objectParent = heldObj.transform.parent;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            //heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void DropObject()
    {
        soundManager.PlaySoundEffect(dropSound, 1.0f);

        //re-enable collision with player
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0; //object assigned back to default layer
        //heldObjRb.isKinematic = false;

        heldObjRb.useGravity = true;
        heldObjRb.drag = 1;
        heldObjRb.constraints = RigidbodyConstraints.None;

        heldObj.transform.parent = objectParent; //unparent object
        if (heldObj.GetComponentInParent<HoldStatus>()) heldObj.GetComponentInParent<HoldStatus>().IsHeld = false;
        heldObj = null; //undefine game object
    }
    void MoveObject()
    {
        if (Vector3.Distance(heldObj.transform.position, holdPos.position) > 0.1f)
        {
            Vector3 moveDir = (holdPos.position - heldObj.transform.position);
            heldObjRb.AddForce(moveDir * (pickupForce), ForceMode.Impulse);
            // check if the force added made the object overshoot, if it did then reposition

        }
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        //Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        //heldObj.layer = 0;
        //heldObjRb.isKinematic = false;
        //heldObj.transform.parent = objectParent;
        // find the root parent with rigid body
        //var parent = objectParent.transform;
        //print(parent.gameObject.name);
        //print(objectParent.gameObject.name);    
        //Rigidbody parentObjRb = null;
        //foreach (Transform child in parent) {
        //  if (child.GetComponent<Rigidbody>() != null && child.Equals(heldObj)) {
        //    parentObjRb = child.GetComponent<Rigidbody>();
        //  print(parentObjRb.name);
        //}
        //}
        soundManager.PlaySoundEffect(throwSound, 1.0f);

        heldObjRb.useGravity = true;
        heldObjRb.drag = 1;
        heldObjRb.constraints = RigidbodyConstraints.None;
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = objectParent;
        //print(parentObjRb.name);\
        //var mainObjectRb = heldObj.GetComponent <Rigidbody>();
        //if (parentRb != null)
        {
            //parentRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        }
        //else {
            heldObjRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        //}
        if (heldObj.GetComponentInParent<HoldStatus>()) heldObj.GetComponentInParent<HoldStatus>().IsHeld = false;
        heldObj = null;
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the cam
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to cam position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    public bool isHolding() { 
        return heldObj != null;
    }
}