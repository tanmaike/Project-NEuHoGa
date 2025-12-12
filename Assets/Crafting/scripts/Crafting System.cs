using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // For text

public class CraftingSystem : MonoBehaviour
{
    public Inventory inventory; // Assign the Player's inventory
    public Recipe currentRecipe; // The recipe we are currently viewing

    [Header("UI Elements")]
    public Button showRecipeButton;
    public Button craftButton;
    public TextMeshProUGUI recipeText; // Shows ingredient list
    public Image outputIcon;
    public TextMeshProUGUI outputQuantityText;

    void Start()
    {
        // Hook up the button clicks to our methods
        showRecipeButton.onClick.AddListener(ShowRecipe);
        craftButton.onClick.AddListener(CraftItem);

        ClearRecipeUI();
    }

    void ClearRecipeUI()
    {
        recipeText.text = "Select a recipe...";
        outputIcon.sprite = null;
        outputIcon.color = new Color(1, 1, 1, 0);
        outputQuantityText.text = "";
        craftButton.interactable = false;
    }

    public void ShowRecipe()
    {
        if (currentRecipe == null) return;

        // Build the ingredient list string
        string ingredientsList = "Requires:\n";
        foreach (var ingredient in currentRecipe.ingredients)
        {
            ingredientsList += $"- {ingredient.quantity}x {ingredient.item.itemName}\n";
        }
        recipeText.text = ingredientsList;

        // Show the output item
        outputIcon.sprite = currentRecipe.output.item.icon;
        outputIcon.color = new Color(1, 1, 1, 1);
        outputQuantityText.text = currentRecipe.output.quantity > 1 ? currentRecipe.output.quantity.ToString() : "";

        // Check if we can craft it
        //CheckCraftability();
    }

    // Call this whenever the inventory changes
    public void CheckCraftability()
    {
        if (currentRecipe != null)
        {
            craftButton.interactable = inventory.HasIngredients(currentRecipe.ingredients);
        }
    }

    public void CraftItem()
    {
        if (currentRecipe == null) return;

        // 1. Check if we have ingredients
        if (inventory.HasIngredients(currentRecipe.ingredients))
        {
            // 2. Remove ingredients from inventory
            inventory.RemoveItems(currentRecipe.ingredients);

            // 3. Add output item to inventory
            inventory.AddItem(currentRecipe.output.item, currentRecipe.output.quantity);
            
            // 4. Re-check if we can craft another one
            //CheckCraftability();
        }
    }

    // We need to re-check craftability whenever the inventory changes.
    // Let's listen to the inventory's event.
    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnSlotChanged += OnInventoryChanged;
        }
    }
    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnSlotChanged -= OnInventoryChanged;
        }
    }

    // This is called by the event
    private void OnInventoryChanged(int slotIndex)
    {
        //CheckCraftability();
    }
}