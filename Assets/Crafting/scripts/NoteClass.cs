using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Note")]
public class NoteClass : ScriptableObject
{
    public string noteID;
    public string noteTitle;
    public Sprite noteImage;
    [TextArea(5,15)]
    public string noteText;
}
