using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControl : MonoBehaviour
{
    private int[] result, correctCombination;
    private bool padlockSolved;
    public InteractableItem spawnedItem;

    void Start()
    {
        result = new int[] { 0, 0, 0 };
        correctCombination = new int[] { 7, 4, 6 };
        RotatePadlock.Rotated += CheckResults;
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "wheel1":
                result[0] = number;
                break;
            case "wheel2":
                result[1] = number;
                break;
            case "wheel3":
                result[2] = number;
                break;
        }
        if (!padlockSolved && result[0] == correctCombination[0] && result[1] == correctCombination[1] && result[2] == correctCombination[2]) {
            padlockSolved = true;
            HUDNotification.Instance.displayMessage("The padlock unlocks.");
            if (spawnedItem != null) spawnedItem.gameObject.SetActive(true);
        }
    }
    
    private void OnDestroy()
    {
        RotatePadlock.Rotated -= CheckResults;
    }
}
