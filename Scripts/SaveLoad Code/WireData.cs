using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WireData 
{

    public PuzzleObject origin;
    public PuzzleObject destination;

    public bool bSelectable;
    public bool bVisible;
    public bool bUsableByAi;

    public  WireData()
    {

    }

    public  WireData(GameObject originPiece, GameObject destinationPiece, bool selectable=false, bool visible = true, bool usableByAi = true)
    {
        origin = new PuzzleObject(originPiece.name, new Vector2(originPiece.transform.position.x, originPiece.transform.position.y));
        destination = new PuzzleObject(destinationPiece.name, new Vector2(destinationPiece.transform.position.x, destinationPiece.transform.position.y));

        bSelectable = selectable;
        bVisible = visible;
        bUsableByAi = usableByAi;
    }

}
