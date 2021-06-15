using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{

    public bool bActive;
    //Public Data
    public List<CharacterBase> characterList;
    public List<GoalConsole> terminalList;
    public List<Wire> wireList;

    public List<Solution> chosenSolutions;
    //MainDataForPathfinding

    //private List<List<PathNode>> paths; //list of possible paths (which have their own list of possible solutions)
    private List<Task> solutionTree;
    List<Solution> solutions;
    //public List<PuzzleObjectBase> obstaclesThatCanBeBypassed;

    void Start()
    {
        bActive = false;
        //obstaclesThatCanBeBypassed.Add(Door);
    }

    void Update()
    {
        /*if (Input.GetKeyDown("a"))
        {
            bActive = true;
            ReevaluateAI();
        }*/

    }

    public void Initialize(GameObject level)//Load Data for current level
    {
        terminalList = new List<GoalConsole>();
        characterList = new List<CharacterBase>();
        wireList = new List<Wire>();

        List<CharacterBase> tempCharacterList = level.transform.GetComponentsInChildren<CharacterBase>().ToList();
        foreach(CharacterBase enemy in tempCharacterList)
        {
            if(enemy.bAlive == true)
            {
                characterList.Add(enemy);
            }
        }
        terminalList = level.transform.GetComponentsInChildren<GoalConsole>().ToList();
        
        List<Wire> tempWireList = level.transform.GetComponentsInChildren<Wire>().ToList();
        
        foreach(Wire w in tempWireList)
        {
            if (w.bVisibleToAi) { wireList.Add(w); }//Finds all wires then removes ones AI cannot 'see'
        }
    }

    public void ActivateAI(bool active=true)
    {
        bActive = active;
        if (active)
        {
            ReevaluateAI();
        }
    }
    

    public void ReevaluateAI()
    {
        Initialize(GameObject.Find("LevelHolder(Clone)"));
        List<Tile> goalTiles = new List<Tile>();
        foreach (GoalConsole terminal in terminalList)
        {
            goalTiles.Add(CharacterBase.GetTargetTile(terminal.gameObject));
        }
        solutionTree = BuildSolutionTree(characterList, goalTiles);
        solutions = new List<Solution>();
        FindAllSolutions();
        AssignSolutions();
    }


    public void AssignSolutions()
    {
        foreach(Solution sol in chosenSolutions)
        {
            foreach(CharacterBase chara in sol.requiredCharacters)
            {
                chara.ResetTaskStack();
                chara.bActive = true;
            }
            Queue<Task> tasksToBeGiven = new Queue<Task>();
            tasksToBeGiven.Enqueue(sol.taskList[0]);
            while (tasksToBeGiven.Count > 0)
            {
                Task t = tasksToBeGiven.Dequeue();
                t.character.taskList.Push(t);
                foreach(Task child in t.requiredTasks)
                {
                    if (child.parentTasks.Contains(t))
                    {
                        tasksToBeGiven.Enqueue(child);
                    }
                }
            }
        }
    }


    //public List<List<PathNode>> BuildSolutionTree(List<CharacterBase> characters, List<Tile> goals, List<PuzzleObjectBase> passedExclusionList = null)
    public List<Task> BuildSolutionTree(List<CharacterBase> characters, List<Tile> goals, List<PuzzleObjectBase> passedExclusionList = null)
    {
        List<PuzzleObjectBase> recursionExclusionList = new List<PuzzleObjectBase>();
        //List<List<PathNode>> pathList = new List<List<PathNode>>();
        List<Task> taskList = new List<Task>();

        foreach(CharacterBase origin in characters)
        {
            foreach(Tile destination in goals)
            {
                List<PuzzleObjectBase> exclusionList = new List<PuzzleObjectBase>();//objects excluded each search to generate alternate paths
                if (passedExclusionList != null)
                {
                    recursionExclusionList = passedExclusionList;
                    exclusionList.AddRange(passedExclusionList);
                }
                else
                {
                    recursionExclusionList = new List<PuzzleObjectBase>();
                }
                
                bool bDoneSearching = false; //To see if we're done looking for alternate routes
                while (!bDoneSearching)
                {
                    List<PathNode> currentPath = convertToPathNodeList(origin.FindPath(destination, exclusionList));
                    List<PuzzleObjectBase> taskExclusionList = new List<PuzzleObjectBase>(exclusionList); //This keeps track of which objects this specific task route avoids so rebuilding the path during solutions avoids the same objects
                    if(currentPath.Count > 0) //I.E there IS a path
                    {
                        float pathCost = currentPath[currentPath.Count - 1].thisTile.fTotalCost;
                        int iObstaclesFound = 0; //Tells the task how many obstacles this particular path needs to unlock
                        bool bFoundObstacle = false;
                        bool bCheckedList = false;
                        int iCounter = 0;
                        while (!bCheckedList) 
                        {
                            if (currentPath[iCounter].containedObstacle != null)
                            {
                                iObstaclesFound++;
                                bFoundObstacle = true;
                                //bCheckedList = true;
                                exclusionList.Add(currentPath[iCounter].containedObstacle.GetComponent<PuzzleObjectBase>());

                                List<Wire> potentialUnlocks = FindWiresForObject(currentPath[iCounter].containedObstacle.GetComponent<PuzzleObjectBase>());
                                List<Tile> potentialUnlockTiles = new List<Tile>();
                                foreach (Wire w in potentialUnlocks)
                                {
                                    potentialUnlockTiles.Add(CharacterBase.GetTargetTile(w.origin));
                                }
                                if (potentialUnlockTiles.Count == 0)
                                {
                                    currentPath[iCounter].possibleUnlockPaths = new List<Task>();
                                    currentPath[iCounter].possibleUnlockPaths.Add(new Task(false));//If there is no unlockable paths, add an impossible task to this branch
                                }
                                else
                                {
                                    recursionExclusionList.Add(currentPath[iCounter].containedObstacle.GetComponent<PuzzleObjectBase>());
                                    currentPath[iCounter].possibleUnlockPaths = BuildSolutionTree(characterList, potentialUnlockTiles, recursionExclusionList);
                                    recursionExclusionList.Remove(currentPath[iCounter].containedObstacle.GetComponent<PuzzleObjectBase>()); //That exclusion has been sent down the recursion and is removed so other solutions don't keep the exclusion needlessly
                                }
                            }
                            if (iCounter >= currentPath.Count-1) 
                            { 
                                bCheckedList = true; 
                            }
                            iCounter++;
                        }
                        if (!bFoundObstacle)
                        {
                            bDoneSearching = true;
                        }

                        //alternateRoutes.Add(currentPath);
                        Task newTask = new Task(currentPath, origin, destination.containedObjects,iObstaclesFound, pathCost, taskExclusionList);
                        taskList.Add(newTask);
                        //pathList.Add(currentPath);
                        
                    }
                    else
                    {
                        bDoneSearching = true;
                    }  
                        
                }
            }
        }

        //return pathList;
        return taskList;
    }


    private void FindAllSolutions()
    {
        
        int iSolutionIndex = 0;

        List<Task> previousSolution = new List<Task>();
        List<Task> currentSolution = solutionTree[iSolutionIndex].GetNextSolution();
        solutions.Add(new Solution(currentSolution));
        while(previousSolution.SequenceEqual(currentSolution) == false && iSolutionIndex <solutionTree.Count)
        {
            previousSolution = currentSolution;

            currentSolution = solutionTree[iSolutionIndex].GetNextSolution();
            if(currentSolution.SequenceEqual(previousSolution))
            {
                iSolutionIndex++;
                if (iSolutionIndex < solutionTree.Count)
                {
                    currentSolution = solutionTree[iSolutionIndex].GetNextSolution();
                    solutions.Add(new Solution(currentSolution));
                } 
            }
            else
            {
                solutions.Add(new Solution(currentSolution));
            }
        }

        for(int iCounter=0; iCounter<solutions.Count; iCounter++)
        {
            if (solutions[iCounter].bPossibleSolution == false)
            {
                solutions.Remove(solutions[iCounter]);
                iCounter--;
            }
        }

        //----------------------Choose solutions to use--------------
        chosenSolutions = new List<Solution>();
        List<CharacterBase> unassignedCharacters = new List<CharacterBase>(characterList);

        while (unassignedCharacters.Count > 0)
        {
            Solution bestSolution = null;
            for(int iCounter =0; iCounter< solutions.Count; iCounter++)
            {
                //.Except returns any items in requiredChars that are NOT in unassignedCharacters  .Any() asks if there are any, if this is false then we have chars we need
                if (!solutions[iCounter].requiredCharacters.Except(unassignedCharacters).Any())
                {
                    if (bestSolution == null)
                    {
                        bestSolution = solutions[iCounter];
                    }
                    else
                    {
                        if(solutions[iCounter].totalCost < bestSolution.totalCost)
                        {
                            bestSolution = solutions[iCounter];
                        }
                    }
                }
            }
            if(bestSolution == null)
            {
                Debug.Log("Unable to find solutions for " + unassignedCharacters.Count + " characters!");
                unassignedCharacters.Clear();
            }
            else
            {
                chosenSolutions.Add(bestSolution);
                foreach(CharacterBase chara in bestSolution.requiredCharacters)
                {
                    unassignedCharacters.Remove(chara);
                }
            }
        }

           /* //For Debugging purposes------- Shows every solution found
        foreach(Solution sol in solutions)
        {
            string output = "solution: ";
            foreach(Task t in sol.taskList)
            {
                output += t.character + "goes to " + t.goal[0].ToString() + " at " + t.goal[0].transform.position + "  with pathlength: " + t.path.Count + ", " ;
            }
            output += " Total cost = " + sol.totalCost;
            Debug.Log(output);
        }*/
        
        
    }


    public static  List<PathNode> convertToPathNodeList(List<Tile> TilePath)
    {
        List<PathNode> output = new List<PathNode>();

        foreach(Tile tile in TilePath)
        {
            output.Add(new PathNode(tile));
        }

        return output;

    }
    
    public static List<Tile> convertToTileList(List<PathNode> PathNodeList)
    {
        List<Tile> output = new List<Tile>();
        foreach(PathNode tile in PathNodeList)
        {
            output.Add(tile.thisTile);
        }
        return output;
    }

    private List<Wire> FindWiresForObject(PuzzleObjectBase obj)
    {
        List<Wire> wiresFound = new List<Wire>();
        foreach(Wire w in wireList)
        {
            if(w.destination == obj.gameObject)
            {
                wiresFound.Add(w);
            }
        }

        return wiresFound;
    }
}
