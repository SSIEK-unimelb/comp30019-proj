using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    [SerializeField] float destroyTime = 2.5f;
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
            collision.gameObject.layer == LayerMask.NameToLayer("HitboxLayer") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                Debug.Log("Arrow hit a target");
                // Arrow logic here
                if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    GoblinAI goblinAI = collision.gameObject.GetComponentInParent<GoblinAI>();
                    if (goblinAI == null)
                    {
                        Debug.LogError("Can't find goblin reference");
                    }
                    /*
                    if (goblinAI.IsKillable())
                    {
                        goblinAI.Die();
                        // Transform backHitpoint = collision.gameObject.transform.Find("BackHitpoint")
                    }
                    */
                }
                Destroy(gameObject);

        }
    }
}
