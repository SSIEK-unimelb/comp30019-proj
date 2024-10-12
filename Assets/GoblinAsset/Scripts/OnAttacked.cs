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
        print(collider.name);   
        if (collider.gameObject.tag == "Weapon") {
            if (goblinAI.IsKillable()) {
                //print("Should die");
                soundManager.PlaySoundEffect(hitSound, 0.3f);
                // Kill parent goblin.
                // check : the gameobject is a knife AND the knife is in stabbing animation
                StabbingStatus status = collider.gameObject.GetComponent<StabbingStatus>();
                if (status != null)
                {
                    // this is a knife
                    if (!status.isStabbing)
                    {
                        return;
                    }
                }
                goblinAI.Die();
                Destroy(gameObject);
            }
        }
    }
}
