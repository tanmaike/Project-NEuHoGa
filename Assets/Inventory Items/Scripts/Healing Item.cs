using UnityEngine;

// This adds a new option in your Right-Click -> Create menu specifically for healing items
[CreateAssetMenu(fileName = "New Healing Item", menuName = "Inventory/Healing Item")]
public class HealingItem : Item 
{
    [Header("Healing Settings")]
    public int healAmount = 20;
    
    // You can add more sounds or effects specific to potions here later
}