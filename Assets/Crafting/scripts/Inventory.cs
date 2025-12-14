using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Used for .FirstOrDefault()

public class Inventory : MonoBehaviour
{
    // This event will fire when an inventory slot changes, so the UI can update
    public event System.Action<int> OnSlotChanged;

    public List<InventorySlot> slots = new List<InventorySlot>();
    public int capacity = 20; // e.g., 5x4 grid

    [Header("Dropping Items")]
    public float dropPoint = 1.5f;
    public float dropHeight = -3f;

    private Transform playerTransform;

    private void Awake()
    {
        // Initialize the inventory with empty slots
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform.Find("dropOrigin");
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

    // remove a specific item
    public bool RemoveItem(Item item)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].item == item && slots[i].quantity > 0)
            {
                RemoveFromSlot(i, 1);
                return true;
            }
        }
        return false;
    }

    public void RemoveFromSlot(int slotIndex, int quantityToRemove)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        var slot = slots[slotIndex];
        
        // Check if we have an item to remove
        if (slot.item != null && slot.quantity >= quantityToRemove)
        {
            TryUnequipIfEquipped(slot.item);
            
            // Subtract the amount
            slot.quantity -= quantityToRemove;
            
            Debug.Log($"Removed {quantityToRemove} from slot {slotIndex}. Remaining: {slot.quantity}");

            // CRITICAL STEP: If quantity is 0, destroy the item in the slot
            if (slot.quantity <= 0)
            {
                slots[slotIndex] = new InventorySlot(null, 0);
                Debug.Log($"Slot {slotIndex} is now empty.");
            }

            // Tell the UI to refresh
            OnSlotChanged?.Invoke(slotIndex);
        }
    }

    // Drop an item from a specific slot
    public void DropItem(int slotIndex, int quantityToDrop = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        
        var slot = slots[slotIndex];
        if (slot.item != null && slot.quantity >= quantityToDrop)
        {
            TryUnequipIfEquipped(slot.item);
            Vector3 dropPoint = GetPlayerDropPosition();
            Debug.Log($"Dropping item at player position: {dropPoint}");
            
            // Spawn the world item
            SpawnInteractableItem(slot.item, quantityToDrop, dropPoint);
            
            // Remove from inventory
            RemoveFromSlot(slotIndex, quantityToDrop);
        }
    }

    public void DropAllFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        
        var slot = slots[slotIndex];
        if (slot.item != null)
        {
            TryUnequipIfEquipped(slot.item);
            Vector3 dropPoint = GetPlayerDropPosition();
            SpawnInteractableItem(slot.item, slot.quantity, dropPoint);
            slots[slotIndex] = new InventorySlot(null, 0);
            OnSlotChanged?.Invoke(slotIndex);
        }
    }

    private Vector3 GetPlayerDropPosition()
{
        if (playerTransform != null) return playerTransform.position + playerTransform.forward * dropPoint;
        return transform.position + Vector3.forward * dropPoint;
    }

    private void SpawnInteractableItem(Item item, int quantity, Vector3 position)
    {
        InteractableItem existingInteractableItem = FindExistingInteractableItem(item);
        if (existingInteractableItem != null)
        {
            existingInteractableItem.quantity = quantity;
            existingInteractableItem.DropItem(position);
            return;
        }
        
        GameObject interactableItemGO = new GameObject($"InteractableItem_{item.itemName}");
        
        InteractableItem interactableItem = interactableItemGO.AddComponent<InteractableItem>();
        interactableItem.item = item;
        interactableItem.quantity = quantity;
        
        position.y += -0.2f;
        interactableItem.DropItem(position);
    }

    // Helper method to find existing disabled world items of the same type
    private InteractableItem FindExistingInteractableItem(Item item)
    {
        InteractableItem[] allInteractableItems = FindObjectsOfType<InteractableItem>(true); // include inactive
        foreach (InteractableItem interactableItem in allInteractableItems)
        {
            if (interactableItem.item == item && !interactableItem.gameObject.activeInHierarchy)
            {
                return interactableItem;
            }
        }
        return null;
    }

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
                            slots[i] = new InventorySlot(null, 0);
                        }
                        OnSlotChanged?.Invoke(i);
                        break;
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

        OnSlotChanged?.Invoke(indexA);
        OnSlotChanged?.Invoke(indexB);
    }

    private void TryUnequipIfEquipped(Item item)
    {
        PlayerEquipment equip = FindObjectOfType<PlayerEquipment>();
        if (equip != null && equip.equippedItem == item)
        {
            equip.Unequip();
        }
    }
}