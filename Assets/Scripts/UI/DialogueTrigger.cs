using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    //be sure to reimplement to be able to deal with changing progression
    public List<Dialogue> lines;

    public override void Interact()
    {
        //DialogueManager.Instance.StartDialogue(lines[0]);
    }

}
