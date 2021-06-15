using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSpike : PuzzleObjectBase ,IHasDescription
{
    public string strDescription { get; set; } = "<b>Spike Trap</b> \nImmediately kills any who step on it.";
    public string strActivateDescrip { get; set; } = "";
    public string strTriggerDescrip { get; set; } = "";
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Trigger()
    {
        gameObject.GetComponent<Tile>().FindContainedObjects();
        List<GameObject> contents = gameObject.GetComponent<Tile>().containedObjects;
        foreach(GameObject obj in contents)
        {
            obj.GetComponent<CharacterBase>().Kill();
            //Destroy(obj);

        }
        base.Trigger();
    }
}
