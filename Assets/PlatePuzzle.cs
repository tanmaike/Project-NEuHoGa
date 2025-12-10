using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatePuzzle : MonoBehaviour
{
    [Header("Item Frames")]
    public ItemFrame[] frames = new ItemFrame[4];

    [Header("Order of Plates")]
    public Item[] correctSequence = new Item[4];

    [Header("Puzzle Outcome")]
    public bool puzzleSolved = false;
    public InteractableItem spawnedItem;

    private void Start()
    {
        foreach (var frame in frames) frame.OnItemChanged += OnFrameUpdated;
    }

    private void OnFrameUpdated(ItemFrame changedFrame)
    {
        CheckPuzzle();
    }

    private void CheckPuzzle()
    {
        if (puzzleSolved) return;

        for (int i = 0; i < frames.Length; i++)
            if (frames[i].storedItem != correctSequence[i]) return;

        puzzleSolved = true;
        PuzzleSolved();
    }

    private void PuzzleSolved()
    {
        HUDNotification.Instance.displayMessage("The sink drains.");
        if (spawnedItem != null) spawnedItem.gameObject.SetActive(true);
    }
}
