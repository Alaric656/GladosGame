using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasDescription
{
    //Give this script to any item that has a description that causes a popup box to appear and display it
    string strDescription { get;set;}

    string strActivateDescrip { get; set; }//How the item activates its wires
    string strTriggerDescrip { get; set; }//What a wire causes this object to do
}
