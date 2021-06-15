using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : PuzzleObjectBase, IHasDescription
{
    public string strDescription { get; set; } = "<b>Pressure Plate</b> \nActivates connected wires when sufficient pressure is applied, \nsuch as when stepped on by a heavy, biological intruder.";
    public string strActivateDescrip { get; set; } = "When pressure plate is stepped on...";
    public string strTriggerDescrip { get; set; } = "";

    public Sprite spriteReady;
    public Sprite spriteDown;
    public bool bReady;//Allows the pressure plate to only go off once 
    void Start()
    {
        bReady = true; 
    }

    void Update()
    {
        
    }

    public override void Trigger()
    {
        if (bReady)
        {
            GetComponent<SpriteRenderer>().sprite = spriteDown;
            bReady = false;
            TriggerWires();
            base.Trigger();
        }
    }
}
