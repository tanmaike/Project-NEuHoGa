using System.Collections.Generic;
using UnityEngine;

public class Inventory_UI : MonoBehaviour
{
    public Inventory inventory; // Assign the main Inventory object
    public GameObject slotPrefab; // Prefab for a single UI slot
    public Transform slotParent;  // The panel with the Grid Layout Group

    private List<InventorySlot_UI> uiSlots = new List<InventorySlot_UI>();

    void Start()
    {
        // Subscribe to the inventory's event
        inventory.OnSlotChanged += UpdateSlotUI;
        InitializeInventoryUI();
    }

    void InitializeInventoryUI()
    {
        for (int i = 0; i < inventory.capacity; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotParent);
            InventorySlot_UI newSlot = slotGO.GetComponent<InventorySlot_UI>();
            newSlot.inventory = inventory;
            newSlot.slotIndex = i;
            newSlot.UpdateSlot(inventory.slots[i]); // Update with initial data
            uiSlots.Add(newSlot);
        }
    }

    // This method is called by the OnSlotChanged event from the Inventory
    void UpdateSlotUI(int index)
    {
        if (index < uiSlots.Count)
        {
            uiSlots[index].UpdateSlot(inventory.slots[index]);
        }
    }
}