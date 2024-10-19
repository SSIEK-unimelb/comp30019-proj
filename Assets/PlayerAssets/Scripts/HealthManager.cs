// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    private HeartScript heartScript;
    private string heartsGameObjectName = "PlayerUI/Heart";
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;

    private bool isDead = false;
    [SerializeField] private UnityEvent onDeath;

    private void Start()
    {
        ResetHealthToStarting();
        heartScript = transform.Find(heartsGameObjectName).GetComponent<HeartScript>();
        heartScript.UpdateHearts(startingHealth);
    }

    public void ResetHealthToStarting()
    {
        currentHealth = this.startingHealth;
    }

    public void ApplyDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        heartScript.UpdateHearts(currentHealth);

        if (currentHealth <= 0) {
            this.onDeath.Invoke();
            isDead = true;
        }
    }
}
