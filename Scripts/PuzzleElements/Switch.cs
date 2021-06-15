using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : PuzzleObjectBase, IHasDescription
{

    public string strDescription { get; set; } = "<b>Switch</b> \nHumans will use these if they think it \nwill open doors for them.";
    public string strActivateDescrip { get; set; } = "When switch is hit...";
    public string strTriggerDescrip { get; set; } = "";

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
