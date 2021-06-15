using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleObjectBase : MonoBehaviour
{
    public List<GameObject> wireList;
    public bool bPassable;
    public bool bUnlockable;

    public bool bMovable;
    public bool bWirable;
    //public string description = "A generic puzzle object";
    public bool bCanBeActivated;

    public virtual void Activate()//TODO Add more activation types 
    {
        Debug.Log(gameObject.ToString() + " Has Been Activated");
        GameObject.Find("EventManager").GetComponent<EventManager>().LevelHasChanged();
    }

    public virtual void TriggerWires()
    {
        if (GameObject.Find("GameController").GetComponent<GameController>().currentState == GameController.GameState.GamePlaying)
        {
            foreach (GameObject wire in wireList)
            {
                wire.GetComponent<Wire>().destination.GetComponent<PuzzleObjectBase>().Activate();
            }
        }
    }

    public virtual void Trigger()//Trigger is activated by the Character class for some reason
    {
        Debug.Log(gameObject.ToString() + " Has Been Triggered");
        GameObject.Find("EventManager").GetComponent<EventManager>().LevelHasChanged();
    }

    public void AddWire(GameObject wire)
    {
        if (wireList == null)
        {
            wireList = new List<GameObject>();
        }

        wireList.Add(wire);
    }

    public void RemoveAllWires()
    {
        wireList.Clear();
    }
}
