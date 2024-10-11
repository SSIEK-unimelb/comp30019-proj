using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] goblins;

    public void MakeSound() {
        foreach (GameObject goblin in goblins) {
            if (goblin != null) {
                GoblinAI goblinAI = goblin.GetComponent<GoblinAI>();
                if (goblinAI) {
                    goblinAI.CanHearSound(transform.position);
                }
            }
        }
    }
}