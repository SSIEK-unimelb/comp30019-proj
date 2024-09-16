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
        //Debug.Log("Collision");

        if (collision.gameObject.tag == "Weapon") 
        {
            if (goblinAI.IsKillable()) {
                //print("Should die");
                // Kill parent goblin.
                goblinAI.Die();
                Destroy(gameObject);
            }
        }
    }
}
