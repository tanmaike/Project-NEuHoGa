using System.Collections.Generic;
using UnityEngine;

// A helper class for ingredients, since each needs an item AND a quantity
[System.Serializable]
public class Ingredient
{
    public Item item;
    public int quantity;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    public List<Ingredient> ingredients;
    public InventorySlot output;
}