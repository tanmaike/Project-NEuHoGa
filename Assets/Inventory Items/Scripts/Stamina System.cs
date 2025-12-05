using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Needed for the Coroutine

public class StaminaSystem : MonoBehaviour
{
    // --- UI/Data References ---
    private float currentStamina; // Use float for smooth regeneration
    private float maxStamina;
    
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private bool isDraining = false;

    [Header("Stamina Mechanics")]
    public float regenRate = 5f;       // Stamina recovered per second
    public float regenDelay = 2f;      // Time to wait before regeneration starts
    public float drainRateMultiplier = 1f; // How quickly stamina is drained when active

    private Coroutine regenCoroutine;

    // --- INITIALIZATION ---
    void Start()
    {
        // This is a simple safety check, you might call SetMaxStamina from another script later
        if (slider != null && slider.maxValue == 0)
        {
            SetMaxStamina(100f); // Default start value
        }
    }

    public void SetMaxStamina(float stamina)
    {
        maxStamina = stamina;
        slider.maxValue = maxStamina;
        slider.value = maxStamina;
        currentStamina = maxStamina;

        // Set initial color (usually full stamina is 1f)
        fill.color = gradient.Evaluate(1f);
    }

    // --- STAMINA ACTIONS ---

    /// <summary>
    /// Drains stamina by a fixed cost, used for actions like a single jump.
    /// </summary>
    public bool DrainStaminaCost(float cost)
    {
        if (currentStamina >= cost)
        {
            currentStamina -= cost;
            UpdateStaminaUI();
            StartRegeneration(); // Starts the delay timer for regeneration
            return true;
        }
        return false;
    }

    public void DrainStaminaOverTime(float amountPerSecond)
    {
        // <--- MODIFIED: Set Draining state and ensure regeneration is stopped --->
        isDraining = true; 
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        // <--- END MODIFIED --->

        currentStamina -= amountPerSecond * Time.deltaTime * drainRateMultiplier;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }
    public void StopStaminaDrain()
    {
        // <--- MODIFIED: Only start regeneration if we were just draining. --->
        if (isDraining)
        {
            isDraining = false; // Reset the flag
            StartRegeneration();
        }
    }

    private void StartRegeneration()
    {
        // <--- MODIFIED: Prevent starting if we are already regenerating or draining --->
        if (regenCoroutine != null || isDraining)
        {
             return; 
        }
        // <--- END MODIFIED --->
        
        // Stop any active regeneration coroutine first (this handles race conditions)
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
        // Start the new coroutine
        regenCoroutine = StartCoroutine(RegenStamina());
    }

    private IEnumerator RegenStamina()
    {
        // Wait for the delay period before starting regen
        yield return new WaitForSeconds(regenDelay);

        // Regenerate stamina until full
        while (currentStamina < maxStamina)
        {
            // Add stamina based on regen rate and time since last frame
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateStaminaUI();
            yield return null; // Wait until the next frame
        }

        // Coroutine finishes when stamina is full
        regenCoroutine = null;
    }

    // --- UI UPDATER ---

    private void UpdateStaminaUI()
    {
        slider.value = currentStamina;
        // Update the fill color based on the current percentage
        slider.normalizedValue = currentStamina / maxStamina;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    /// <summary>
    /// A quick way for other scripts to check if the player can perform an action.
    /// </summary>
    public bool HasStamina(float cost)
    {
        return currentStamina >= cost;
    }
}