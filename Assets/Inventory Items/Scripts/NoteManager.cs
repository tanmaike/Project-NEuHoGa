using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;

    [Header("Notes")]
    public GameObject plateNoteButton;
    public GameObject couchNoteButton;

    [Header("Note Texts")]
    public GameObject plateNoteText;
    public GameObject couchNoteText;

    private void Awake()
    {
        Instance = this;

        plateNoteButton.SetActive(false);
        couchNoteButton.SetActive(false);

        plateNoteText.SetActive(false);
        couchNoteText.SetActive(false);
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
        }
    }

    public void ShowNote(string noteID)
    {
        plateNoteText.SetActive(false);
        couchNoteText.SetActive(false);

        switch (noteID)
        {
            case "PlateNote":
                plateNoteText.SetActive(true);
                break;

            case "CouchNote":
                couchNoteText.SetActive(true);
                break;
        }
    }

    public void ClearAllNotes()
    {
        plateNoteText.SetActive(false);
        couchNoteText.SetActive(false);
    }
}
