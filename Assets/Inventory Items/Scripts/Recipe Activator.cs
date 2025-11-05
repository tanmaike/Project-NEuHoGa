using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeActivator : MonoBehaviour
{
    public GameObject ThisItemsRequirements;
    public GameObject ThisItemsResults;
    private GameObject CurrentRequirements;
    private GameObject CurrentResults;
    public CraftingLogic CraftingLogic;
    public void Awake()
    {
        if (ThisItemsRequirements != null && ThisItemsResults != null)
        {
            ThisItemsRequirements.SetActive(false);
            ThisItemsResults.SetActive(false);
        }
    }
    public void clear()
    {
        if (CurrentRequirements != null && CurrentResults != null)
        {
            CurrentRequirements.SetActive(false);
            CurrentResults.SetActive(false);
        }
    }
    public void Show()
    {
        clear();
        CraftingLogic.Display();
        ThisItemsRequirements.SetActive(true);
        ThisItemsResults.SetActive(true);
        CurrentRequirements = ThisItemsRequirements;
        CurrentResults = ThisItemsResults;
    }
}
