using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TerrainTile
{

    public string strTypeName;
    public Vector2 vPosition; 

    public TerrainTile(string name, Vector2 position)
    {
        strTypeName = name;
        vPosition = position;
    }


}
