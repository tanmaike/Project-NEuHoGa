using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearNotes : MonoBehaviour
{
    public GameObject NoteScreen;
    private bool IsActive = false;

    public void Activate()
    {
        IsActive = true;
        NoteScreen.SetActive(true);
    }
    public void Deactivate()
    {
        IsActive = false;
        NoteScreen.SetActive(false);
    }
}
