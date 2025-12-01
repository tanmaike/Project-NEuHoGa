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

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;
        currentHealth = health;

		fill.color = gradient.Evaluate(1f);
	}

    public void SetHealth(int amount)
	{
        currentHealth = currentHealth - amount;
		slider.value = currentHealth;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}
