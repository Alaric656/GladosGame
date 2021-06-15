using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour, IHasDescription
{
    public string strDescription { get; set; } = "<b>Wire</b>";
    public string strActivateDescrip { get; set; } = "";
    public string strTriggerDescrip { get; set; } = "";

    public GameObject origin;
    public GameObject destination;

    public bool bSelectable;
    public bool bVisibleToAi;
    public bool bVisible;

    /*public GameObject activationText; //These are references to the text that shows what a wire will do 
    public GameObject triggerText;*/

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Initialize(GameObject start, GameObject end, bool selectable=true, bool visible = true, bool AIusable = false)
    {
        origin = start;
        destination = end;
        bSelectable = selectable;
        bVisibleToAi = AIusable;
        bVisible = visible;

        //strDescription += "\n" + start.GetComponent<IHasDescription>().strActivateDescrip + end.GetComponent<IHasDescription>().strTriggerDescrip;

        if (!bSelectable)
        {
            strDescription += "\n<color=red>Cannot be changed</color>";
        }

        if (bVisible)
        {
            UpdatePosition();
        }
    }

    public bool UpdatePosition() //returns true if wire successfully updates
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
        if (origin != null && destination != null)
        {
            transform.localPosition = Vector2.zero;
            line.SetPositions(new Vector3[] { origin.transform.position, destination.transform.position });
            line.startWidth = 0.04f;
            line.endWidth = 0.04f;
            line.startColor = new Color(0, 0, 0, 0.4f);
            line.endColor = new Color(0, 0, 0, 0.4f);

            EdgeCollider2D edgeCollider;
            if (gameObject.GetComponent<EdgeCollider2D>() == null) { edgeCollider = gameObject.AddComponent<EdgeCollider2D>(); }
            else { edgeCollider = gameObject.GetComponent<EdgeCollider2D>(); }
            edgeCollider.SetPoints(new List<Vector2>() { gameObject.transform.InverseTransformPoint(origin.transform.position), gameObject.transform.InverseTransformPoint(destination.transform.position) });
            edgeCollider.edgeRadius = 0.1f; 
        }
        else
        {
            //If destination is missing, remove this wire
            DestroyWire();
            return false;
        }
        return true;
    }

    public void UpdatePosition(Vector2 tempDestination)//Optional entry point for the function, for use when a wire is being placed and doesn't have a destination yet. 
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));

        line.SetPositions(new Vector3[] { origin.transform.position, tempDestination });
        line.startWidth = 0.04f;
        line.endWidth = 0.04f;
        line.startColor = new Color(0, 0, 0, 0.4f);
        line.endColor = new Color(0, 0, 0, 0.4f);
    }

    public void DestroyWire()
    {
        origin.GetComponent<PuzzleObjectBase>().wireList.Remove(gameObject);
        Destroy(gameObject);
        Destroy(this);
    }
    
    public void SetCollision(bool active)
    {
        EdgeCollider2D col = GetComponent<EdgeCollider2D>();
        if (col == null)
        {
            return;
        }
        col.enabled = active;
    }
}
