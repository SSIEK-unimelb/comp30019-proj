using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip arrowHitFleshAudio;
    [SerializeField] private AudioClip arrowHitObstacleAudio;
    [SerializeField] float destroyTime = 2.5f;
    private void Start()
    {
        //soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
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
                    //soundManager.PlaySoundEffect(arrowHitFleshAudio, 0.1f);
                    GoblinAI goblinAI = collision.gameObject.GetComponentInParent<GoblinAI>();
                    if (goblinAI == null)
                    {
                        Debug.LogError("Can't find goblin reference");
                    }

                    else 
                    {
                        //if (goblinAI.IsKillable()) { }
                        goblinAI.Die();
                            // Transform backHitpoint = collision.gameObject.transform.Find("BackHitpoint")
                    }
                    
                } else {
                    //soundManager.PlaySoundEffect(arrowHitObstacleAudio, 0.1f);
                }
                Destroy(gameObject);

        }
    }
}
