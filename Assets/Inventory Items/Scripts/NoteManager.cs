using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;

    [Header("Notes")]
    public GameObject plateNoteButton;
    public GameObject couchNoteButton;
    public GameObject firstKitchenNoteButton;
    public GameObject bathroomNoteButton;
    public GameObject bedroomNoteButton;

    [Header("Note Texts")]
    public GameObject plateNoteText;
    public GameObject couchNoteText;
    public GameObject firstKitchenNoteText;
    public GameObject bathroomNoteText;
    public GameObject bedroomNoteText;

    private void Awake()
    {
        Instance = this;

        plateNoteButton.SetActive(false);
        couchNoteButton.SetActive(false);
        firstKitchenNoteButton.SetActive(false);
        bathroomNoteButton.SetActive(false);
        bedroomNoteButton.SetActive(false);

        plateNoteText.SetActive(false);
        couchNoteText.SetActive(false);
        firstKitchenNoteText.SetActive(false);
        bathroomNoteText.SetActive(false);
        bedroomNoteText.SetActive(false);
    }

    public void UnlockNote(string noteID)
    {
        switch (noteID)
        {
            case "001":
                plateNoteButton.SetActive(true);
                break;
            case "002":
                couchNoteButton.SetActive(true);
                break;
            case "003":
                firstKitchenNoteButton.SetActive(true);
                break;
            case "004":
                bathroomNoteButton.SetActive(true);
                break;
            case "005":
                bedroomNoteButton.SetActive(true);
                break;
        }
    }

    public void ShowNote(string noteID)
    {
        plateNoteText.SetActive(false);
        couchNoteText.SetActive(false);
        firstKitchenNoteText.SetActive(false);
        bathroomNoteText.SetActive(false);
        bedroomNoteText.SetActive(false);

        switch (noteID)
        {
            case "001":
                plateNoteText.SetActive(true);
                break;
            case "002":
                couchNoteText.SetActive(true);
                break;
            case "003":
                firstKitchenNoteText.SetActive(true);
                break;
            case "004":
                bathroomNoteText.SetActive(true);
                break;
            case "005":
                bedroomNoteText.SetActive(true);
                break;
        }
    }

    public void ClearAllNotes()
    {
        plateNoteText.SetActive(false);
        couchNoteText.SetActive(false);
        firstKitchenNoteText.SetActive(false);
        bathroomNoteText.SetActive(false);
        bedroomNoteText.SetActive(false);
    }
}
