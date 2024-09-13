using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundMaker : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private float soundRadius = 10f;

    void Start() {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void MakeSound() {
        audioSource.Play();
        // Notify nearby objects of the sound
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (var col in colliders) {
            GoblinAI goblinAI = col.GetComponentInParent<GoblinAI>();
            if (goblinAI) {
                goblinAI.OnSoundHeard(transform.position);
            }
        }
    }
}