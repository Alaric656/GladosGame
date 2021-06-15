using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : PuzzleObjectBase, IHasDescription
{
    public string strDescription { get; set; } = "<b>Explosive Barrel</b> \nIf ignited, this will cause an explosion";
    public string strActivateDescrip { get; set; } = "When this explodes...";
    public string strTriggerDescrip { get; set; } = "Explodes";

    void Start()
    {
        
    }


}
