using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    [SerializeField] private float soundRadius = 10f;

    public void MakeSound() {
        // Notify nearby objects of the sound
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (var col in colliders) {
            GoblinAI goblinAI = col.GetComponentInParent<GoblinAI>();
            if (goblinAI) {
                goblinAI.CanHearSound(transform.position);
            }
        }
    }
}