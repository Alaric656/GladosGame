using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Door : PuzzleObjectBase, IHasDescription
{
    public string strDescription { get; set; } = "<b>Door</b> \nCan be locked to delay intruders.";
    public string strActivateDescrip { get; set; } = "When door opens...";
    public string strTriggerDescrip { get; set; } = "Lock/unlock door";
    public override void Activate()
    { 
        Open();
        base.Activate();
    }

    public void Open()
    {
        gameObject.SetActive(false);
    }

}
