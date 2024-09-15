using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
            collision.gameObject.layer == LayerMask.NameToLayer("HitboxLayer") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                Debug.Log("Arrow hit a target");
                // Arrow logic here
                Destroy(gameObject);

        }
    }
}
