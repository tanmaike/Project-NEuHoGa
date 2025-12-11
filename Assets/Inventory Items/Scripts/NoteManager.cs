using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class NoteUIEntry
{
    public NoteClass NoteData; 
    public GameObject ButtonObject; 
    public TextMeshProUGUI ButtonTitleText; 
}

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;

    [Header("Note UI List")]
    public List<NoteUIEntry> AllNoteEntries = new List<NoteUIEntry>();

    [Header("Note Display")]
    public UnityEngine.UI.Image DisplayBackground;
    public TextMeshProUGUI DisplayBodyText;
    public UnityEngine.UI.Image DisplayImage;
    
    private HashSet<string> unlockedNoteIDs = new HashSet<string>();
    private Dictionary<string, NoteUIEntry> noteIDToUIEntry = new Dictionary<string, NoteUIEntry>();

    private void Awake()
    {
        Instance = this;

        foreach (NoteUIEntry entry in AllNoteEntries)
        {
            entry.ButtonTitleText.text = entry.NoteData.noteTitle;

            entry.ButtonObject.SetActive(false);
            
            noteIDToUIEntry.Add(entry.NoteData.noteID, entry);
        }

        ClearNoteDisplay();
    }

    public string GetNoteTitle(string noteID)
    {
        if (noteIDToUIEntry.TryGetValue(noteID, out NoteUIEntry entry)) return entry.NoteData.noteTitle;       

        return "null"; // error
    }

    public void ClearNoteDisplay()
    {
        DisplayBodyText.text = "";
        DisplayBackground.gameObject.SetActive(false);
        DisplayImage.gameObject.SetActive(false);
    }

    public void UnlockNote(string noteID)
    {
        if (noteIDToUIEntry.TryGetValue(noteID, out NoteUIEntry entry) && !unlockedNoteIDs.Contains(noteID))
        {
            entry.ButtonObject.SetActive(true);
            unlockedNoteIDs.Add(noteID);
        }
    }

    public void ShowNote(string noteID)
    {
        if (noteIDToUIEntry.TryGetValue(noteID, out NoteUIEntry entry))
        {
            DisplayBodyText.text = entry.NoteData.noteText;

            DisplayImage.sprite = entry.NoteData.noteImage;
            DisplayBackground.gameObject.SetActive(true);
            DisplayImage.gameObject.SetActive(true);
        }
    }
}
