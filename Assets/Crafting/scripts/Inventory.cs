using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Used for .FirstOrDefault()

public class Inventory : MonoBehaviour
{
    // This event will fire when an inventory slot changes, so the UI can update
    public event System.Action<int> OnSlotChanged;

    public List<InventorySlot> slots = new List<InventorySlot>();
    public int capacity = 20; // e.g., 5x4 grid

    private void Awake()
    {
        // Initialize the inventory with empty slots
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }

    // Add an item to the inventory
    public bool AddItem(Item item, int quantity)
    {
        // 1. Try to stack with existing items
        for(int i = 0; i < capacity; i++)
        {
            if (slots[i].item == item && slots[i].quantity + quantity <= item.maxStack)
            {
                slots[i].AddQuantity(quantity);
                OnSlotChanged?.Invoke(i); // Notify UI that this slot changed
                return true;
            }
        }

        // 2. Try to find an empty slot
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].item == null)
            {
                slots[i] = new InventorySlot(item, quantity);
                OnSlotChanged?.Invoke(i); // Notify UI that this slot changed
                return true;
            }
        }
        
        return false; // Inventory is full
    }

    // Remove items from the inventory
    public void RemoveItems(List<Ingredient> itemsToRemove)
    {
        foreach (var itemToRemove in itemsToRemove)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (slots[i].item == itemToRemove.item)
                {
                    if (slots[i].quantity >= itemToRemove.quantity)
                    {
                        slots[i].quantity -= itemToRemove.quantity;
                        if (slots[i].quantity <= 0)
                        {
                            slots[i] = new InventorySlot(null, 0); // Clear slot
                        }
                        OnSlotChanged?.Invoke(i); // Notify UI
                        break; // Move to the next itemToRemove
                    }
                }
            }
        }
    }

    // Check if inventory has all required ingredients
    public bool HasIngredients(List<Ingredient> ingredients)
    {
        return ingredients.All(ingredient => 
            slots.Where(slot => slot.item == ingredient.item)
                 .Sum(slot => slot.quantity) >= ingredient.quantity
        );
    }

    // Swap items between two slots (for drag and drop)
    public void SwapSlots(int indexA, int indexB)
    {
        InventorySlot temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;

        OnSlotChanged?.Invoke(indexA); // Notify UI
        OnSlotChanged?.Invoke(indexB); // Notify UI
    }
}