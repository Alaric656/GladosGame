using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public bool bActive;
    public bool bAlive;


    public Tile currentTile;


    public Stack<Task> taskList;
    public Task currentTask;
    public List<Tile> currentPath;
    
    //Character stats---- these must be changed in properties in Unity
    protected float fMoveSpeed = 1; //Tiles they can move per second 
    protected float fTimeToOpenDoor = 0; //Time for character to open a closed door
    //---------------

    protected Tile previouslyMoved;

    public virtual void Start()
    {
        bAlive = true;
    }

    public virtual void Update()
    {
        if (bActive)
        {
            if (currentTask != null)
            {
                Move();
                if (currentPath.Count == 0)
                {
                    currentTask = null;
                }
            }
            else
            {
                if (taskList != null && taskList.Count > 0)
                {
                    currentTask = taskList.Pop();
                    currentPath = EnemyController.convertToTileList(currentTask.path);
                    for (int i = 0; i < currentPath.Count - 1; i++)
                    {
                        Debug.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position, Color.green, currentTask.costForPath);
                    }
                }
            }
            
        }
    }

    public void ResetTaskStack()
    {
        if (taskList != null) { taskList.Clear(); }
        else { taskList = new Stack<Task>();  }
        currentTask = null;
        currentPath = null;
    }

    public void Kill()
    {
        bAlive = false;
        Destroy(gameObject);
        Destroy(this);
    }

    public void Move()
    {
        if (currentPath != null && currentPath.Count>0)//I think it won't check the 2nd condition if first is failed?
        {
            Tile targetTile = currentPath[0];
            if(targetTile == previouslyMoved)
            {
                if (currentPath.Count > 1)
                {
                    targetTile = currentPath[1];
                    currentPath.RemoveAt(0);
                }
                else
                {
                    //currentPath.Clear();
                    
                }
                
            }
            Vector2 direction = targetTile.transform.position - gameObject.transform.position;
            direction.Normalize();

            gameObject.transform.position += (Vector3)direction * fMoveSpeed * Time.deltaTime;

            if (OnTile(targetTile.gameObject, 0.1f)) //Is the character within the middle 10% of the tile's centre
            {
                previouslyMoved = targetTile;
                currentPath.Remove(targetTile);//Only needed if the path isn't constantly updated
            }
        }
        else
        {
            Debug.Log("No current path");
        }
    }


    public bool OnTile(GameObject targetTile, float areaOfTile = 0.1f)
    {
        bool onTile = false;

        Bounds tileBounds = targetTile.GetComponent<SpriteRenderer>().bounds;
        if (gameObject.transform.position.x >= targetTile.transform.position.x - tileBounds.size.x * areaOfTile && gameObject.transform.position.x <= targetTile.transform.position.x + tileBounds.size.x * areaOfTile)
        {
            if (gameObject.transform.position.y >= targetTile.transform.position.y - tileBounds.size.y * areaOfTile && gameObject.transform.position.y <= targetTile.transform.position.y + tileBounds.size.y * areaOfTile)
            {
                onTile = true;
            }
        }

        return onTile;
    }


    //PATHFINDING--------------------------------

    public void FindCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
    }

    public static Tile GetTargetTile(GameObject targetObject)
    {
        Tile target = null;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(targetObject.transform.position,targetObject.GetComponent<SpriteRenderer>().bounds.extents,0f);
        foreach (Collider2D col in colliders)
        {
            if(col.gameObject.tag == "Tile" || col.gameObject.tag == "PuzzleTile")
            {
                target = col.gameObject.GetComponent<Tile>();
            }
        }

        return target;
    }

    protected Tile FindLowestTotalCost(List<Tile> list)
    {
        Tile lowestCostTile = list[0];

        foreach (Tile tile in list)
        {
            if(tile.fTotalCost < lowestCostTile.fTotalCost)
            {
                lowestCostTile = tile;
            }
        }

        return lowestCostTile;
    }

    public List<Tile> FindPath(Tile targetTile, List<PuzzleObjectBase> exclusionList = null, Tile startingTile = null)
    {
        List<GameObject> allTiles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));
        allTiles.AddRange(GameObject.FindGameObjectsWithTag("PuzzleTile"));
        foreach (GameObject aT in allTiles)
        {
            aT.GetComponent<Tile>().FindAdjacency(exclusionList);//Have every tile find it's neighbours
        }
        if (startingTile != null) 
        {
            currentTile = startingTile;
        }
        else
        {
            FindCurrentTile();//Find starting tile
        }
        List<Tile> path = new List<Tile>();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currentTile);

        currentTile.fHueristic = Vector2.Distance(currentTile.transform.position, targetTile.transform.position);
        currentTile.fDistance = 0f;
        currentTile.fTotalCost = currentTile.fHueristic + currentTile.fDistance; //For reference

        while (openList.Count > 0)//Go through and evaluate each tile on open list
        {
            Tile tile = FindLowestTotalCost(openList);
            openList.Remove(tile);
            closedList.Add(tile);

            if(tile == targetTile)
            {
                //Found end, time to build path.
                Stack<Tile> bufferStack = new Stack<Tile>();
                bufferStack.Push(tile);
                Tile parentTile = tile.parent;
                while (parentTile != null)
                {
                    bufferStack.Push(parentTile);
                    parentTile = parentTile.parent;
                }
                while (bufferStack.Count > 0)//Since the path is built backwards - using a buffer stack reverses the order so the List reads the path forwards from 0 to list.Count
                {
                    path.Add(bufferStack.Pop());
                }

                return path;
            }
            
            foreach(Tile adjacent in tile.adjacentTiles)
            {
                if (closedList.Contains(adjacent))
                {
                    //do nothing, tile has already been processed
                }else if (openList.Contains(adjacent))
                {
                    float comparedDistance = tile.fDistance + Vector2.Distance(tile.transform.position, adjacent.transform.position) + AddedObstacles(adjacent);
                    if(comparedDistance < adjacent.fDistance)
                    {
                        adjacent.parent = tile;
                        adjacent.fDistance = comparedDistance;
                        adjacent.fTotalCost = adjacent.fHueristic + adjacent.fDistance;
                    }
                }
                else
                {
                    adjacent.parent = tile;

                    adjacent.fHueristic = Vector2.Distance(adjacent.transform.position, targetTile.transform.position);//Distance to goal
                    adjacent.fDistance = tile.fDistance + Vector2.Distance(tile.transform.position, adjacent.transform.position) + AddedObstacles(adjacent);//Total distance for this path
                    adjacent.fTotalCost = adjacent.fHueristic + adjacent.fDistance;

                    openList.Add(adjacent);
                }
            }

        }

        //Couldn't find path
        return path;

    }

    protected float AddedObstacles(Tile tile)
    {
        float increasedDistance = 0;
        foreach(GameObject obstacle in tile.containedObjects)
        {
            if(obstacle.gameObject.tag == "Door")
            {
                increasedDistance += fTimeToOpenDoor * fMoveSpeed;
            }
        }

        return increasedDistance;
    }


    //----------------------------

    public virtual void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Wall" || col.gameObject.tag == "Door" || (col.gameObject.GetComponent<PuzzleObjectBase>()!=null && col.gameObject.GetComponent<PuzzleObjectBase>().bPassable == false))
        {
            Vector2 directionOfImpact = col.gameObject.transform.position - gameObject.transform.position;
            Vector2 maxDistanceToTouch = (Vector2)(gameObject.GetComponent<Collider2D>().bounds.extents); //+col.gameObject.GetComponent<Collider2D>().bounds.extents
            float distance = col.collider.Distance(gameObject.GetComponent<Collider2D>()).distance;
            distance = Mathf.Abs(distance);
            directionOfImpact.Normalize();
            //gameObject.transform.position += (Vector3)(-directionOfImpact*fMoveSpeed*3f * Time.deltaTime);

            //gameObject.transform.position += (Vector3)((maxDistanceToTouch * -directionOfImpact) + (-directionOfImpact*(fMoveSpeed * Time.deltaTime)));

            gameObject.transform.position += (Vector3)((distance * -directionOfImpact) + (-directionOfImpact * (fMoveSpeed * 2* Time.deltaTime)));


        }
    }

    public void OnCollisionStay2D (Collision2D col)
    {
        if (col.gameObject.tag == "Tile" || col.gameObject.tag == "PuzzleTile" || col.gameObject.tag  == "PressurePlate")
        {
            if (col.gameObject.GetComponent<PuzzleObjectBase>() != null && OnTile(col.gameObject))
            {
                col.gameObject.GetComponent<PuzzleObjectBase>().Trigger();
            }
        }
    }

    public float GetMoveSpeed()
    {
        return fMoveSpeed;
    }
}
