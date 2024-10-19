// Adapted from COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private GoblinAI goblinAI;
    private int damageAmount;
    private string tagToDamage;
    private float attackCooldownDuration = 0.8f;
    private float currentAttackCooldownTime;

    void Start() {
        goblinAI = GetComponentInParent<GoblinAI>();
        damageAmount = goblinAI.GetDamageAmount();
        tagToDamage = goblinAI.GetTagToDamage();
        currentAttackCooldownTime = attackCooldownDuration;
    }

    void Update() {
        currentAttackCooldownTime -= Time.deltaTime;
    }

    // If collided with a damageable object, do damage.
    private void OnTriggerEnter(Collider col) {
        if (col.transform.root.gameObject.CompareTag(this.tagToDamage)) {
            if (goblinAI.IsAttacking()) {
                // If attack on cooldown, do nothing.
                if (currentAttackCooldownTime > 0) return;
                currentAttackCooldownTime = attackCooldownDuration;

                // Damage object with relevant tag. Note that this assumes the 
                // HealthManager component is attached to the respective object.
                var healthManager = col.transform.root.gameObject.GetComponentInParent<HealthManager>();
                healthManager.ApplyDamage(this.damageAmount);
            }
        }
    }
}
