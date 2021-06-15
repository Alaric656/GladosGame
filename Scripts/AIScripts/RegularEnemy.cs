using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularEnemy : CharacterBase
{

   

    public override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);        
        if (col.gameObject.tag == "Switch")
        {
            col.gameObject.GetComponent<Switch>().TriggerWires();
            //bActive = false;
        }
    }
}
