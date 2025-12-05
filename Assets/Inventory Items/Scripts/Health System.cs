using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    int currentHealth;
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    // Call this at the start to set up the health bar
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        currentHealth = health;

        fill.color = gradient.Evaluate(1f);
    }

    // Call this function to DEAL DAMAGE
    public void TakeDamage(int damageAmount)
    {
        // Subtract the damage
        currentHealth -= damageAmount;

        // Clamp the value so it never goes below 0
        currentHealth = Mathf.Clamp(currentHealth, 0, (int)slider.maxValue);

        // Update the slider UI
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // Call this function to HEAL
    public void Heal(int healAmount)
    {
        // Add the health
        currentHealth += healAmount;

        // Clamp the value so it never goes above the max health
        currentHealth = Mathf.Clamp(currentHealth, 0, (int)slider.maxValue);

        // Update the slider UI
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}