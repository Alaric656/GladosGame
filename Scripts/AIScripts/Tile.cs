using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public List<GameObject> containedObjects;
    public List<Tile> adjacentTiles;

    public float fDistance;
    public float fHueristic;
    public float fTotalCost;

    public Tile parent;
    public bool bWalkable;

    void Start()
    {
        adjacentTiles = new List<Tile>();
        containedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        
    }

    public void FindAdjacency(List<PuzzleObjectBase> exclusionList = null)
    {
        ResetValues();

        if(exclusionList == null)
        {
            exclusionList = new List<PuzzleObjectBase>();
        }
        Vector2 areaOfCheck = new Vector2(gameObject.GetComponent<Collider2D>().bounds.extents.x, gameObject.GetComponent<Collider2D>().bounds.extents.y);
        //areaOfCheck = areaOfCheck * 0.9f;//check 90% of the area of this tile

        CheckTile(areaOfCheck, new Vector2(0, 1), exclusionList);//Check four directions
        CheckTile(areaOfCheck, new Vector2(1, 0), exclusionList);
        CheckTile(areaOfCheck, new Vector2(0, -1), exclusionList);
        CheckTile(areaOfCheck, new Vector2(-1, 0), exclusionList);

        FindContainedObjects();
    }

    public void CheckTile(Vector2 area, Vector2 direction, List<PuzzleObjectBase> exclusionList)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)gameObject.transform.position + (area * direction), area, 0f);

        foreach(Collider2D col in colliders)
        {
            if (col.gameObject.tag == "Tile" || col.gameObject.tag == "PuzzleTile")
            {
                if (col.gameObject.GetComponent<Tile>().bWalkable) //Is the tile considered walkable
                {
                    if (!col.gameObject.GetComponent<Tile>().ContainsException(exclusionList))//This checks if this tile is being excluded from this path calculation in order to find alternate routes
                    {
                        //Can object actually reach the tile
                        gameObject.layer = 2; //ignore raycast layer
                        col.gameObject.layer = 2;

                        bool bCanReach = false;
                        bool bDoneSearching = false;
                        List<GameObject> obstacles = new List<GameObject>();
                        while (bDoneSearching == false)
                        {
                            RaycastHit2D rayhit = Physics2D.Raycast(gameObject.transform.position, col.gameObject.transform.position - gameObject.transform.position, 1);
                            if (rayhit == false)
                            {
                                bDoneSearching = true;
                                bCanReach = true;

                            }
                            else if (rayhit.collider.gameObject.tag == "Doodad" || rayhit.collider.gameObject.tag == "Wall" || rayhit.collider.gameObject.tag == "ExplosiveBarrel")  //Things that block movement
                            {
                                bCanReach = false;
                                bDoneSearching = true;
                            }
                            else
                            {
                                rayhit.collider.gameObject.layer = 2;
                                obstacles.Add(rayhit.collider.gameObject);
                            }
                        }

                        if (bCanReach)
                        {
                            adjacentTiles.Add(col.gameObject.GetComponent<Tile>());
                            Debug.DrawLine(gameObject.transform.position, col.gameObject.transform.position, Color.red, 100);
                        }

                        foreach (GameObject obj in obstacles)
                        {
                            obj.layer = 0;
                        }

                        gameObject.layer = 0; //back to default
                        col.gameObject.layer = 0;
                    }

                }
            }
        }
    }

    public void FindContainedObjects()
    {
        Vector2 areaOfCheck = new Vector2(gameObject.GetComponent<Collider2D>().bounds.size.x, gameObject.GetComponent<Collider2D>().bounds.size.y);
        areaOfCheck = areaOfCheck * 0.8f;//check 90% of the area of this tile
        Collider2D[] colliders = Physics2D.OverlapBoxAll(gameObject.transform.position,areaOfCheck,0f);
        foreach(Collider2D tile in colliders)
        {
            if (tile.gameObject.tag != "Tile" && tile.gameObject.tag != "PuzzleTile" && tile.gameObject.tag != "Wire")
            {
                containedObjects.Add(tile.gameObject);
            }
        }
    }

    public bool ContainsException(List<PuzzleObjectBase> exceptionList)//Does this tile contain a puzzleobject being excluded from another tiles neighbour search
    {
        bool bHasException = false;

        List<PuzzleObjectBase> containedPuzzleObjects = new List<PuzzleObjectBase>();
        foreach(GameObject obj in containedObjects)
        {
            if(obj.GetComponent<PuzzleObjectBase>() != null)
            {
                containedPuzzleObjects.Add(obj.GetComponent<PuzzleObjectBase>());
            }
        }

        foreach(PuzzleObjectBase contents in containedPuzzleObjects)
        {
            if (exceptionList.Contains(contents))
            {
                bHasException = true;
            }
        }

        return bHasException;
    }

    public void ResetValues()
    {
        fDistance = 0;
        fHueristic = 0;
        fTotalCost = 0;
        parent = null;

        adjacentTiles.Clear();
        containedObjects.Clear();
    }
}
