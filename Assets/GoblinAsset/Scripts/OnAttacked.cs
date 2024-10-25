using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class OnAttacked : MonoBehaviour
{
    private GoblinAI goblinAI;

    private SoundManager soundManager;
    [SerializeField] private AudioClip hitSound;

    public void Start() {
        goblinAI = GetComponentInParent<GoblinAI>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    private void OnTriggerEnter(Collider collider) {
        //Debug.Log("The goblin has collided with: " + collider.gameObject.name);
        if (collider.gameObject.CompareTag("Weapon")) {
            if (goblinAI.IsKillable()) {
                //Debug.Log("The goblin has collided with a weapon!");

                // check : the gameobject is a knife AND the knife is in stabbing animation
                StabbingStatus status = collider.gameObject.GetComponent<StabbingStatus>();
                if (status != null) {
                    //Debug.Log("This is a knife, is it now stabbing or not?");
                    if (status.isStabbing) {
                        soundManager.PlaySoundEffect(hitSound, 0.3f);
                        if (goblinAI != null) goblinAI.Die();
                        //Debug.Log("The knife is in stabbing animation!");
                    } else {
                        //Debug.Log("The knife is NOT in stabbing animation!");
                    }
                }
            }
        }
    }
}
