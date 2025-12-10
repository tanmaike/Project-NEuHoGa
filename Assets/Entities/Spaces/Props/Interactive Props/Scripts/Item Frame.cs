using UnityEngine;
using System.Collections.Generic;

public class ItemFrame : MonoBehaviour, IInteractable
{
    public Item storedItem;
    
    [Header("Allowed Items")]
    public List<Item> allowedItems = new List<Item>();

    private SpriteRenderer spriteRenderer;

    public event System.Action<ItemFrame> OnItemChanged;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            GameObject child = new GameObject("Sprite");
            child.transform.SetParent(transform);
            
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            spriteRenderer = child.AddComponent<SpriteRenderer>();
        }

        UpdateSprite();
    }

    public void Interact(Vector3 interactorPos, Item equippedItem)
    {
        PlayerEquipment eq = FindObjectOfType<PlayerEquipment>();
        Inventory inv = FindObjectOfType<Inventory>();

        if (storedItem == null)
        {
            if (equippedItem == null) return;

            if (!allowedItems.Contains(equippedItem))
            {
                Debug.Log($"{equippedItem.itemName} is not permitted");
                return;
            }

            inv.RemoveItem(equippedItem);
            HUDNotification.Instance.displayMessage("Placed " + equippedItem.itemName + ".");

            if (eq.equippedItem == equippedItem) eq.Unequip();

            storedItem = equippedItem;
            UpdateSprite();
        }

        else
        {
            if (!inv.AddItem(storedItem, 1))
            {
                Debug.Log("inventory full");
                return;
            }

            storedItem = null;
            UpdateSprite();
        }

        OnItemChanged?.Invoke(this);
    }

    public void UpdateSprite()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = (storedItem != null) ? storedItem.icon : null;
    }
}