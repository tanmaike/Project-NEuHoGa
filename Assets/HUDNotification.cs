using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDNotification : MonoBehaviour
{
    public static HUDNotification Instance;

    [Header("HUD Reference")]
    public TMP_Text messageText;
    [Header("Display Settings")]
    public float messageDuration = 2f;

    private Coroutine messageDisplay;

    private void Awake()
    {
        Instance = this;

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void displayMessage(string msg)
    {
        if (messageDisplay != null)
            StopCoroutine(messageDisplay);

        messageDisplay = StartCoroutine(displayMessageRoutine(msg));
    }

    private IEnumerator displayMessageRoutine(string msg)
    {
        messageText.text = msg;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }
}
