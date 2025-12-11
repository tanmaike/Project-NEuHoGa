using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthSystem healthScript = FindObjectOfType<HealthSystem>();

            if (healthScript != null)
            {
                healthScript.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("Player hit, but no HealthSystem found in the scene!");
            }
        }
    }
}