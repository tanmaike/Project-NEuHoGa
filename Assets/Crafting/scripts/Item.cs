using UnityEngine;

// This attribute lets you right-click in the Project window and create a new Item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject 
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public int maxStack = 64;
}