using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteExplosive : PuzzleObjectBase, IHasDescription
{
    public string strDescription { get; set; } = "<b>Remote Explosive</b> \nIf ignited or triggered by a wire\nthis will cause an explosion";
    public string strActivateDescrip { get; set; } = "When this explodes...";
    public string strTriggerDescrip { get; set; } = "Explodes";

    private float fExplosionRadius = 1;

    void Start()
    {
        
    }

    public override void Activate()
    {
        base.Activate();
    }

    public void InitializeExplosive()
    {
        gameObject.GetComponent<Explosive>().SetExplosionRadius(fExplosionRadius);
    }

}
