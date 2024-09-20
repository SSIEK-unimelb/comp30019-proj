// Adapted from COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private int damageAmount;
    private string tagToDamage;
    private float attackInterval;
    private float currentAttackIntervalTime;

    void Start() {
        damageAmount = GetComponentInParent<GoblinAI>().GetDamageAmount();
        tagToDamage = GetComponentInParent<GoblinAI>().GetTagToDamage();
        attackInterval = GetComponentInParent<GoblinAI>().GetAttackInterval();
        currentAttackIntervalTime = attackInterval;
    }

    void Update() {
        currentAttackIntervalTime -= Time.deltaTime;
    }

    // If collided with a damageable object, do damage.
    private void OnTriggerEnter(Collider col) {
        if (col.transform.root.gameObject.CompareTag(this.tagToDamage)) {
            // If attack on cooldown, do nothing.
            if (currentAttackIntervalTime > 0) return;

            currentAttackIntervalTime = attackInterval;
            // Damage object with relevant tag. Note that this assumes the 
            // HealthManager component is attached to the respective object.
            var healthManager = col.transform.root.gameObject.GetComponentInParent<HealthManager>();
            healthManager.ApplyDamage(this.damageAmount);
        }
    }
}
