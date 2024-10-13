using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    [SerializeField] float destroyTime = 2.5f;
    private void Start()
    {
        //soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider collider) {
        Debug.Log("Arrow hit gameobject: " + collider.gameObject + " in OnTriggerEnter");
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Debug.Log("Goblin was killed by an arrow");
            GoblinAI goblinAI = collider.gameObject.GetComponentInParent<GoblinAI>();
            if (goblinAI == null) Debug.LogError("Can't find goblin reference");
            else goblinAI.Die();
            Destroy(gameObject);
        }
    }
}
