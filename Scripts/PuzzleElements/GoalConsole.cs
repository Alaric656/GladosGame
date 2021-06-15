using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalConsole : PuzzleObjectBase ,IHasDescription
{
    public string strDescription { get; set; } = "<b>Command Console</b> \nA user could shut me out \nof the system here.";
    public string strActivateDescrip { get; set; } = "";
    public string strTriggerDescrip { get; set; } = "";

}
