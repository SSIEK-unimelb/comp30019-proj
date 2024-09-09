using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class OnAttacked : MonoBehaviour
{
    private GoblinAI goblinAI;
    public void Awake()
    {
        goblinAI = GetComponentInParent<GoblinAI>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon") 
        {
            print("Should die");
            // Kill parent goblin.
            StartCoroutine(goblinAI.Die());
            Destroy(gameObject);
        }
    }
}
