using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotatePadlock : MonoBehaviour, IInteractable
{
    public static event Action<string, int> Rotated = delegate { };
    private bool coroutineAllowed;
    public int numberShown;

    void Start()
    {
        coroutineAllowed = true;
        numberShown = 0;
    }

    public void Interact(Vector3 interactorPosition, Item item)
    {
        if (coroutineAllowed) StartCoroutine("RotateWheel");
    }

    private IEnumerator RotateWheel()
    {
        coroutineAllowed = false;

        for (int i = 0; i <= 11; i++)
        {
            transform.Rotate(3f, 0f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        coroutineAllowed = true;

        numberShown += 1;
        HUDNotification.Instance.displayMessage(numberShown + "...");

        if (numberShown > 9) numberShown = 0;

        Rotated(name, numberShown);
    }
}
