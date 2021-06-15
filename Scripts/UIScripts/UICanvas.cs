using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICanvas : MonoBehaviour
{
    public  bool BlockedByUI;
    private EventTrigger eventTrigger;

    public void Start()//This stuff checks if the mouse is currently on a UI element or not
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            // Pointer Enter
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            //Pointer Exit
            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);
        }
    }

    public void EnterUI()
    {
        BlockedByUI = true;
    }
    public void ExitUI()
    {
        BlockedByUI = false;
    }
}
