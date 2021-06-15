using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

    public Tile thisTile;
    //public List<List<PathNode>> possibleUnlockPaths;
    public List<Task> possibleUnlockPaths;
    public int solutionIndex;

    public GameObject containedObstacle;

    private List<Task> previousSolution;

    public PathNode(Tile tile)
    {
        solutionIndex = 0;
        thisTile = tile;
        containedObstacle = null;
        //possibleUnlockPaths = new List<List<PathNode>>();
        possibleUnlockPaths = new List<Task>();
        foreach (GameObject obj in thisTile.containedObjects)
        {
            if (obj.GetComponent<PuzzleObjectBase>() != null)
            {
                if (obj.GetComponent<PuzzleObjectBase>().bUnlockable == true)
                {
                    containedObstacle = obj;
                }
            }
        }
    }

    public void ResetSolution()
    {
        solutionIndex = 0;
        foreach (Task t in possibleUnlockPaths)
        {
            t.ResetSolution();
        }
    }

    /*public List<Task> GetCurrentSolution()
    {
        List<Task> returnedTask = null;
        if (possibleUnlockPaths.Count > 0)
        {
            returnedTask = possibleUnlockPaths[solutionIndex].GetNextSolution();
        }
        return returnedTask;
    }*/

    public bool NextBranch()
    {
        if (solutionIndex < possibleUnlockPaths.Count - 1)
        {
            solutionIndex++;
            return true;
        }
        return false;
    }

    public List<Task> GetNextSolution()
    {
        List<Task> returnedTask = null;
        if(possibleUnlockPaths.Count > 0)
        {
            returnedTask = possibleUnlockPaths[solutionIndex].GetNextSolution();
        }
        /*if(returnedTask == previousSolution)
        {
            if (solutionIndex < possibleUnlockPaths.Count - 1)
            {
                solutionIndex++;
                returnedTask = possibleUnlockPaths[solutionIndex].GetNextSolution();
            }
        }

        previousSolution = returnedTask;*/
        return returnedTask;    
    }
}
