using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solution
{

    public List<Task> taskList;
    public List<CharacterBase> requiredCharacters;
    public float totalCost;

    private List<Task> possibleTasks;

    public bool bPossibleSolution;

    public Solution()
    {
        totalCost = 0f;
        requiredCharacters = new List<CharacterBase>();
        bPossibleSolution = true;
    }

    public Solution(List<Task> tasks)
    {
        taskList = new List<Task>();
        requiredCharacters = new List<CharacterBase>();
        bPossibleSolution = true;
        /*foreach(Task t in tasks)
        {
            taskList.Add(t.Clone());
        }*/


        //---This section clones the list of tasks to create a discrete solution, it uses the required tasks pointer on the parent to maintain the relationships----
        Queue<Task> openTasks = new Queue<Task>(); //stores tasks that need their children cloned
        possibleTasks = new List<Task>(); //stores tasks that have no requirements
        openTasks.Enqueue(tasks[0].Clone()); //0th task is always the main goal
        while (openTasks.Count > 0)
        {
            Task currentTask = openTasks.Dequeue();
            List<Task> bufferRequiredTasks = new List<Task>(currentTask.requiredTasks);
            currentTask.requiredTasks = new List<Task>();
            if (bufferRequiredTasks.Count == 0)
            {
                possibleTasks.Add(currentTask);
            }
            foreach(Task t in bufferRequiredTasks)
            {
                Task newTask = t.Clone();
                newTask.parentTasks.Add(currentTask);
                currentTask.requiredTasks.Add(newTask);
                openTasks.Enqueue(newTask);
            }
            taskList.Add(currentTask);
        }
        totalCost = 0f;

        EvaluateSolution();
    }


    public void EvaluateSolution()
    {
        List<Task> nextLevelTasks = new List<Task>();
        List<Task> evaluatedTasks = new List<Task>();
        Task currentTask;

        while (possibleTasks.Count > 0)
        {

            bool bCanEvaluate = true;
            currentTask = FindShortestTask(possibleTasks);
            if(currentTask.character == null)
            {
                bPossibleSolution = false;
                return;//If there is a task with no character assigned, that means this solution isn't possible
            }
            if (requiredCharacters.Contains(currentTask.character))
            {
                Task requiredTask = null;
                int iCounter = evaluatedTasks.Count - 1;
                while(requiredTask == null && iCounter>-1)
                {
                    if(evaluatedTasks[iCounter].character == currentTask.character)
                    {
                        requiredTask = evaluatedTasks[iCounter];
                    }
                    iCounter--;
                }
                if (requiredTask.parentTasks.Contains(currentTask) == false) 
                {
                    requiredTask.parentTasks.Clear();
                    requiredTask.parentTasks.Add(currentTask);
                    nextLevelTasks.Add(currentTask);
                    possibleTasks.Remove(currentTask);
                    if (!currentTask.requiredTasks.Contains(requiredTask))
                    {
                        currentTask.requiredTasks.Add(requiredTask);
                    }
                    bCanEvaluate = false;
                    
                    foreach(Task t in evaluatedTasks)
                    {
                        if (t != requiredTask && t.parentTasks.Contains(currentTask))//Switching which tasks consider this a parent
                        {
                            t.parentTasks.Remove(currentTask);
                        }
                    }
                }
                currentTask.startingTile = requiredTask.path[requiredTask.path.Count - 1];
            }
            else
            {
                requiredCharacters.Add(currentTask.character);
            }
            if (bCanEvaluate)
            {
                //Debug.Log("Evaluating task with goal of " + currentTask.goal[0] +" " + currentTask.goal[0].transform.position);
                //if (requiredTask != null) { Debug.Log("It's required task has a goal of " + requiredTask.goal[0].transform.position); } //DEBUG CHECKING CODE
                if(currentTask.startingTile != currentTask.path[0])
                {
                    float oldDistanceCost = currentTask.totalCost - currentTask.costForPath;
                    //This rebuilds a path based on having a different starting location than previously expected, currently there is no exclusion list :S 
                    currentTask.path = EnemyController.convertToPathNodeList(currentTask.character.FindPath(CharacterBase.GetTargetTile(currentTask.goal[0]), currentTask.exclusionList, currentTask.startingTile.thisTile));
                    currentTask.costForPath = currentTask.path[currentTask.path.Count - 1].thisTile.fTotalCost;
                    currentTask.totalCost = currentTask.costForPath + oldDistanceCost;
                }
                possibleTasks.Remove(currentTask);
                evaluatedTasks.Add(currentTask);
                foreach (Task parent in currentTask.parentTasks)
                {
                    float newTotalCost = parent.costForPath + currentTask.totalCost;
                    if(newTotalCost > parent.costForPath)
                    {
                        parent.totalCost = newTotalCost;
                    }
                    if (!nextLevelTasks.Contains(parent) && !possibleTasks.Contains(parent) && !evaluatedTasks.Contains(parent))
                    {
                        if (!parent.requiredTasks.Except(evaluatedTasks).Any())//Ensures the parents required tasks have already been evaluated
                        {
                            nextLevelTasks.Add(parent);
                        }
                    }
                } 
            }
            if (possibleTasks.Count == 0)
            {
                possibleTasks.AddRange(nextLevelTasks);
                nextLevelTasks.Clear();
            }
        }
        totalCost = taskList[0].totalCost;
    }

    private Task FindShortestTask(List<Task> tasks)
    {
        Task shortest = tasks[0];
        foreach(Task t in tasks)
        {
            if(t.totalCost < shortest.totalCost)
            {
                shortest = t;
            }
        }
        return shortest;
    }
}
