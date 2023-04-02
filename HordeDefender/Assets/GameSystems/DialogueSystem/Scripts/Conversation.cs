using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Conversation
{
    public long ID;
    public List<Dialogue> dialogues;

    public void AddDialogue(Dialogue dialogue)
    {
        dialogues.Add(dialogue);
    }
}
