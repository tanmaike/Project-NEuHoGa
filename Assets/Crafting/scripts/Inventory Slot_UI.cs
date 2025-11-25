using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for drag/drop interfaces
using TMPro; // Use TextMeshPro for quantity text

public class InventorySlot_UI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler 
{
    [Header("UI Elements")]
    public Image icon;
    public TextMeshProUGUI quantityText;

    [Header("Slot Data")]
    public Inventory inventory; // Reference to the main inventory
    public int slotIndex;       // This slot's index in the inventory list
    
    // Static reference to the item being dragged
    private static InventorySlot_UI currentlyDragging;
    private static GameObject dragIcon;

    // Handle right-click to drop items
    public void OnPointerClick(PointerEventData eventData)
    {
        var slot = inventory.slots[slotIndex];

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slot.item != null)
                inventory.DropItem(slotIndex, 1);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (slot.item != null)
            {
                PlayerEquipment equipment = FindObjectOfType<PlayerEquipment>();
                if (equipment != null) {
                    equipment.Equip(slot.item);
                }
            }
        }
    }

    // This updates the slot's visual (icon and quantity)
    public void UpdateSlot(InventorySlot slot)
    {
        if (slot.item != null)
        {
            icon.sprite = slot.item.icon;
            icon.color = new Color(1, 1, 1, 1); // Make visible
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
        else
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0); // Make invisible
            quantityText.text = "";
        }
    }

    // --- Drag and Drop Logic ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].item == null) return; // Don't drag empty slots

        currentlyDragging = this;

        // Create a temporary drag icon that follows the mouse
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root); // Set parent to Canvas root
        dragIcon.transform.SetAsLastSibling(); // Ensure it renders on top
        var img = dragIcon.AddComponent<Image>();
        img.sprite = icon.sprite;
        img.raycastTarget = false; // Prevents it from blocking drop events
        dragIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

        icon.color = new Color(1, 1, 1, 0.5f); // Make original semi-transparent
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentlyDragging == null) return;

        // If dropped outside UI, drop the item
        if (eventData.pointerEnter == null || 
            (eventData.pointerEnter.GetComponent<InventorySlot_UI>() == null && 
             eventData.pointerEnter.GetComponentInParent<InventorySlot_UI>() == null))
        {
            // Drop the entire stack
            inventory.DropAllFromSlot(currentlyDragging.slotIndex);
        }
        
        currentlyDragging = null;
        Destroy(dragIcon);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentlyDragging != null)
        {
            // We dropped on this slot. Tell the inventory to swap the items.
            inventory.SwapSlots(currentlyDragging.slotIndex, this.slotIndex);
            
            // Reset visibility of the slot we dragged from
            currentlyDragging.UpdateSlot(inventory.slots[currentlyDragging.slotIndex]);
        }
    }
}