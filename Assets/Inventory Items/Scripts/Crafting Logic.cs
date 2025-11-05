using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingLogic : MonoBehaviour
{
    public GameObject Arrow;
    public GameObject CraftButton;
    public RecipeActivator RecipeActivator;
    public GameObject Requirements;
    public GameObject Results;
    public void Display()
    {
        Arrow.SetActive(true);
        CraftButton.SetActive(true);
        Requirements.SetActive(true);
        Results.SetActive(true);
    }
    public void Reset()
    {
        Arrow.SetActive(false);
        CraftButton.SetActive(false);
        RecipeActivator.clear();
        Requirements.SetActive(false);
        Results.SetActive(false);
    }
}
