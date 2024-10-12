using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] goblins;

    public void MakeSound() {
        foreach (GameObject goblin in goblins) {
            if (goblin != null) {
                GoblinAI goblinAI = goblin.GetComponent<GoblinAI>();
                if (goblinAI) {
                    Debug.Log("Calling goblin CanHearSound");
                    goblinAI.CanHearSound(transform.position);
                }
            }
        }
    }
}