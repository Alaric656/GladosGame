using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleObject 
{
    public string strTypeName;
    public Vector2 vPosition;

    public PuzzleObject(string name, Vector2 position)
    {
        strTypeName = name;
        vPosition = position;
    }


}
