using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    int currentHealth;
    public int actualHealth;
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    // Call this at the start to set up the health bar
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        currentHealth = health;
        actualHealth = health;

        fill.color = gradient.Evaluate(1f);
    }

    // Call this function to DEAL DAMAGE
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        actualHealth = currentHealth;

        currentHealth = Mathf.Clamp(currentHealth, 0, (int)slider.maxValue);

        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);

        if (currentHealth <= 0)
        {
            die();
        }
    }

    // Call this function to HEAL
    public void Heal(int healAmount)
    {
        // Add the health
        currentHealth += healAmount;
        actualHealth += healAmount;

        // Clamp the value so it never goes above the max health
        currentHealth = Mathf.Clamp(currentHealth, 0, (int)slider.maxValue);

        if (actualHealth <= 20 && actualHealth > 10)
        {
            slider.value = 25;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        } else if(actualHealth <= 10 && actualHealth > 0)
        {
            slider.value = 22;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
        else
        {
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
    public void Update()
    {
        if (currentHealth <= 0)
        {
            die();
        }
    }
    public void die()
    {
        SceneManager.LoadScene("EndScene");
    }
}