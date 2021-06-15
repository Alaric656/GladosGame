using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Task 
{
    public List<GameObject> goal;
    public CharacterBase character;
    public PathNode startingTile;
    public List<PathNode> path;

    public int NumberOfRequiredTasks; // keeps track of how many tasks must be completed that allow for this one's completion
    public List<Task> requiredTasks;
    public List<Task> parentTasks;

    public float costForPath;
    public float totalCost;

    private List<Task> previousSolution;
    private int iUniqueBranchIndex; //Keeps track of which branches have been reset to prevent infinite loop

    public List<PuzzleObjectBase> exclusionList;

    public Task(bool possible = true)
    {
        iUniqueBranchIndex = 0;
        previousSolution = new List<Task>();
        parentTasks = new List<Task>();
        requiredTasks = new List<Task>();
        NumberOfRequiredTasks = 0;
        totalCost = 0;
        costForPath = 0;
        path = new List<PathNode>();
        exclusionList = null;

        if (!possible)
        {
            costForPath = 100000;
        }
    }

    public Task(List<PathNode> givenPath, CharacterBase givenChar = null, List<GameObject> givenGoal = null, int obstacles = 0, float pathCost = 0, List<PuzzleObjectBase> exclusions = null)
    {
        path = givenPath;
        character = givenChar;
        startingTile = givenPath[0];
        goal = givenGoal;
        NumberOfRequiredTasks = obstacles;
        costForPath = pathCost;

        iUniqueBranchIndex = 0;
        previousSolution = new List<Task>();
        requiredTasks = new List<Task>();
        parentTasks = new List<Task>();
        exclusionList = exclusions;

        if (character != null)
        {
            costForPath = costForPath / character.GetMoveSpeed();
        }
        totalCost = costForPath;
    }

    public List<Task> GetNextSolution()
    {
        requiredTasks.Clear();
        List<Task> solution = new List<Task>();
        solution.Add(this);

        List<Task> tempSolution = new List<Task>();
        List<Task> returnedSolution;
        //bool solutionImpossible = false;

        foreach (PathNode p in path)
        {
            if (p.containedObstacle != null)
            {
                returnedSolution = p.GetNextSolution();
                if (returnedSolution != null)
                {
                    tempSolution.AddRange(returnedSolution);
                    requiredTasks.Add(returnedSolution[0]);//The first task in the list will always be the one directly below this
                }
                else
                {
                    
                }
                //tempSolution.AddRange(p.GetNextSolution());
            }
        }
        if((tempSolution.Count> 0 && previousSolution.SequenceEqual(tempSolution)))
        {
            bool bBranchHasChanged = false;
            int iCounter = 0;//iUniqueBranchIndex;
            while (!bBranchHasChanged && iCounter < path.Count) 
            { 
                if (path[iCounter].NextBranch() == true)
                {
                    iUniqueBranchIndex = iCounter;
                    bBranchHasChanged = true;
                }
                iCounter++;
            }
            if (bBranchHasChanged)
            {
                requiredTasks.Clear();
                tempSolution.Clear();
                for (iCounter = 0; iCounter < path.Count;iCounter++)
                {
                    if (iCounter < iUniqueBranchIndex)
                    {
                        path[iCounter].ResetSolution();
                    }
                }
                foreach (PathNode p in path)
                {
                    if (p.containedObstacle != null)
                    {
                        //tempSolution.AddRange(p.GetNextSolution());
                        returnedSolution = p.GetNextSolution();
                        if (returnedSolution != null)
                        {
                            tempSolution.AddRange(returnedSolution);
                            requiredTasks.Add(returnedSolution[0]);//The first task in the list will always be the one directly below this
                        }
                    }
                }
            }

        }
        solution.AddRange(tempSolution);
        previousSolution = tempSolution;
        return solution;
    }


    public void ResetSolution()
    {
        iUniqueBranchIndex = 0;
        previousSolution = null; 
        foreach(PathNode p in path)
        {
            p.ResetSolution();
        }
    }

    public Task Clone()//Used to create shallow copies of tasks
    {
        Task newTask = (Task)this.MemberwiseClone();
        newTask.parentTasks = new List<Task>();
        return newTask;
    }
}
