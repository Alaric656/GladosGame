using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUIController : MonoBehaviour
{

    private LevelController levelControllerReference;
    public Editor editorReference;
    public UICanvas canvasReference;

    public GameObject PopupBox;
    public GameObject itemToolbar;
    public GameObject txtWireActivate;
    public GameObject txtWireTrigger;

    public GameObject PlanningUI;
    public GameObject PlayingUI;

    public GameObject PressurePlateButton;
    public GameObject RemoteExplosiveButton;

    //Puzzle pieces the player can place
    public GameObject pressurePlate;
    public GameObject remoteExplosive;

    private TrapName currentTrapName;
    private GameObject currentTrap;


    private GameObject currentlyHighlightedObject;

    private GameObject currentlySelectedObject;
    private GameObject tempWire;

    private UIState currentState;
    private GameController.GameState currentGameState;

    private RaycastHit2D rayHit;

    //UI Data
    public Color cannotPlaceColour;
    public Color highlightColour;

    Camera mainCam;

    private enum TrapName
    {
        None,
        PressurePlate,
        RemoteExplosive
    }

    private enum UIState
    {
        None,
        PlacingPiece,
        SelectingObject,
        Wiring
    }

    public void Initialize()
    {
        currentState = UIState.None;
        SwitchUI(GameController.GameState.Menu);
        levelControllerReference = GameObject.Find("LevelController").GetComponent<LevelController>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        PopupBox.SetActive(false);
        gameObject.SetActive(false);
        itemToolbar.SetActive(false);
        txtWireActivate.SetActive(false);
        txtWireTrigger.SetActive(false);
    }

    public void EnableUI()
    {
        gameObject.SetActive(true);
    }

    public void DisableUI()
    {
        gameObject.SetActive(false);
    }

    public void SwitchUI(GameController.GameState newState)
    {
        switch (newState) 
        {
            case GameController.GameState.Menu:
                PlayingUI.SetActive(false);
                PlanningUI.SetActive(false);
                break;
            case GameController.GameState.Editor:
                PlayingUI.SetActive(false);
                PlanningUI.SetActive(false);
                break;
            case GameController.GameState.GamePlanning:
                PlayingUI.SetActive(false);
                PlanningUI.SetActive(true);
                break;
            case GameController.GameState.GamePlaying:
                ClearPlayerUI();
                PlayingUI.SetActive(true);
                PlanningUI.SetActive(false);
                break;
        }
        currentState = UIState.None;
        currentGameState = newState;
    }

    public void ClearPlayerUI()
    {
        if (currentState == UIState.PlacingPiece)
        {
            if (currentlySelectedObject == null)
            {
                levelControllerReference.iNumberPressurePlates++;
                UpdateUIValues();
            }
            Destroy(currentTrap);
            currentTrap = null;
        }
        if (currentlyHighlightedObject != null)
        {
            UnHighlightObject(currentlyHighlightedObject);
        }
        if (currentlySelectedObject != null)
        {
            DeselectObject();
        }
        if (tempWire != null)
        {
            Destroy(tempWire);
            tempWire = null;
        }
        
        ClosePopup();
        CloseToolbar();
    }

    public void UpdateUIValues()
    {
        PressurePlateButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = levelControllerReference.iNumberPressurePlates.ToString();
        RemoteExplosiveButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = levelControllerReference.iNumberRemoteExplosives.ToString();
    }

    void Update()
    {
        switch (currentState)
        {
            case UIState.None:
                if (!canvasReference.BlockedByUI)
                {
                    rayHit = GetTopObject(Input.mousePosition);
                    if (rayHit != false && rayHit.collider != null)
                    {

                        if (rayHit.collider.gameObject.GetComponent<IHasDescription>() != null)
                        {
                            HighlightObject(rayHit.collider.gameObject);
                            ShowPopup(currentlyHighlightedObject.transform.position, rayHit.collider.gameObject.GetComponent<IHasDescription>().strDescription);
                        }
                        else if (currentlyHighlightedObject != null)
                        {
                            UnHighlightObject(currentlyHighlightedObject);
                        }
                    }
                    else if (currentlyHighlightedObject != null)
                    {
                        UnHighlightObject(currentlyHighlightedObject);
                    }
                }
                else
                {
                    UnHighlightObject(currentlyHighlightedObject);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (currentlyHighlightedObject != null)
                    {
                        if (currentlyHighlightedObject.GetComponent<PuzzleObjectBase>()!= null)
                        {
                            if (currentGameState == GameController.GameState.GamePlanning)
                            {
                                currentState = UIState.SelectingObject;
                                currentlySelectedObject = currentlyHighlightedObject;
                                ShowToolbar(currentlySelectedObject);
                                ClosePopup();
                            }
                        }
                        if (currentlyHighlightedObject.GetComponent<Wire>() != null)
                        {
                            if(currentGameState == GameController.GameState.GamePlanning && currentlyHighlightedObject.GetComponent<Wire>().bSelectable == true)
                            {
                                currentlySelectedObject = currentlyHighlightedObject.GetComponent<Wire>().origin;
                                ClosePopup();
                                currentlyHighlightedObject.GetComponent<Wire>().DestroyWire();
                                WireSelectedObject();
                            }
                        }
                        //UnHighlightObject(currentlyHighlightedObject);
                    }
                }
                break;


            case UIState.PlacingPiece:
                UpdatePlacingPiece();
                break;

            case UIState.SelectingObject:
                if(Input.GetMouseButtonDown(0))
                {
                    if (!canvasReference.BlockedByUI)
                    {
                        rayHit = GetTopObject(Input.mousePosition);
                        if (rayHit != false && rayHit.collider != null)
                        {

                            if (rayHit.collider.gameObject.GetComponent<IHasDescription>() != null && rayHit.collider.gameObject.GetComponent<PuzzleObjectBase>() !=null)
                            {
                                if(rayHit.collider.gameObject != currentlySelectedObject)//Change selected object
                                {
                                    UnHighlightObject(currentlySelectedObject);
                                    HighlightObject(rayHit.collider.gameObject);
                                    currentlySelectedObject = rayHit.collider.gameObject;
                                    currentlyHighlightedObject = rayHit.collider.gameObject;
                                }  
                            }
                            else
                            {
                                DeselectObject();
                            }
                        }
                        else
                        {
                            DeselectObject();
                        }
                    }
                }else if (Input.GetMouseButtonDown(1))
                {
                    DeselectObject();
                }

                if (currentlyHighlightedObject != null)//Constantly updated toolbar every frame to keep up with camera movement
                {
                    ShowToolbar(currentlySelectedObject);
                }
                break;
            case UIState.Wiring:
                bool bHaveObjectPosition = false;
                if (!canvasReference.BlockedByUI)
                {
                    rayHit = GetTopObject(Input.mousePosition);
                    if (rayHit != false && rayHit.collider != null)
                    {

                        if (rayHit.collider.gameObject.GetComponent<PuzzleObjectBase>() != null && rayHit.collider.gameObject != currentlySelectedObject)
                        {
                            if (rayHit.collider.gameObject.GetComponent<PuzzleObjectBase>().bCanBeActivated == true)
                            {
                                tempWire.GetComponent<Wire>().UpdatePosition(rayHit.collider.gameObject.transform.position);
                                bHaveObjectPosition = true;
                                if (Input.GetMouseButtonDown(0))
                                {
                                    Destroy(tempWire);
                                    GameObject newWire = Instantiate(editorReference.wirePrefab);
                                    newWire.GetComponent<Wire>().Initialize(currentlySelectedObject, rayHit.collider.gameObject);
                                    newWire.transform.parent = currentlySelectedObject.transform;
                                    currentlySelectedObject.GetComponent<PuzzleObjectBase>().AddWire(newWire);
                                    DeselectObject();
                                    UnHighlightObject(currentlyHighlightedObject);
                                }
                            }
                        }
                    }
                }
                if (!bHaveObjectPosition)
                {
                    tempWire.GetComponent<Wire>().UpdatePosition(mainCam.ScreenToWorldPoint(Input.mousePosition));
                }
                

                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(tempWire);
                    tempWire = null;
                    currentState = UIState.None;
                    UnHighlightObject(currentlySelectedObject);
                    currentlySelectedObject = null;
                    currentlyHighlightedObject = null;
                }
                break;
        }
        //Allow player to highlight and select objects on screen  
    }

    private void DeselectObject()
    {
        CloseToolbar();
        UnHighlightObject(currentlySelectedObject);
        currentlySelectedObject = null;
        currentState = UIState.None;
        currentlyHighlightedObject = null;
    }

    public void PlacePressurePlate()
    {
        if (currentState != UIState.PlacingPiece && levelControllerReference.iNumberPressurePlates > 0)
        {
            levelControllerReference.iNumberPressurePlates--;
            UpdateUIValues();
            currentTrapName = TrapName.PressurePlate;
            StartPlacingPuzzlePiece(pressurePlate);
        }
    }

    public void PlaceRemoteExplosive()
    {
        if (currentState != UIState.PlacingPiece && levelControllerReference.iNumberRemoteExplosives > 0)
        {
            levelControllerReference.iNumberRemoteExplosives--;
            UpdateUIValues();
            currentTrapName = TrapName.RemoteExplosive;
            StartPlacingPuzzlePiece(remoteExplosive);
        }
    }

    private void StartPlacingPuzzlePiece(GameObject piece)
    {
        currentState = UIState.PlacingPiece;
        currentTrap = Instantiate(piece);
        currentTrap.gameObject.name = piece.name;
        currentTrap.layer = 2;//Ignore raycast layer
        currentTrap.GetComponent<SpriteRenderer>().sortingOrder = 2; 
        foreach(GameObject w in currentTrap.GetComponent<PuzzleObjectBase>().wireList)
        {
            w.GetComponent<Wire>().SetCollision(false);
        }
    }

    public void WireSelectedObject()
    {
        if (currentGameState == GameController.GameState.GamePlanning)
        {
            CloseToolbar();
            ClosePopup();
            //UnHighlightObject(currentlyHighlightedObject);
            currentState = UIState.Wiring;
            tempWire = Instantiate(editorReference.wirePrefab);
            tempWire.GetComponent<Wire>().origin = currentlySelectedObject;
        }
    }

    public void DeleteSelectedObject()
    {
        if (currentGameState == GameController.GameState.GamePlanning)
        {
            CloseToolbar();
            ClosePopup();
            if (currentlySelectedObject.tag == "PressurePlate")
            {
                levelControllerReference.iNumberPressurePlates++;
            }
            if (currentlySelectedObject.tag == "RemoteExplosive")
            {
                levelControllerReference.iNumberRemoteExplosives++;
            }
            UpdateUIValues();
            Destroy(currentlySelectedObject);
            currentState = UIState.None;
        }
    }

    public void MoveSelectedObject()
    {
        if (currentGameState == GameController.GameState.GamePlanning)
        {
            CloseToolbar();
            ClosePopup();
            if (currentlySelectedObject.GetComponent<Explosive>() != null)
            {
                currentlySelectedObject.GetComponent<Explosive>().HideRadius();
            }
            if (currentlySelectedObject.GetComponent<PressurePlate>()) { currentTrapName = TrapName.PressurePlate; }
            if(currentlySelectedObject.GetComponent<RemoteExplosive>()) { currentTrapName = TrapName.RemoteExplosive; }
            StartPlacingPuzzlePiece(currentlySelectedObject);
            //currentlyHighlightedObject = null;
        }
    }

    private void UpdatePlacingPiece()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 gridPosition = new Vector2(Editor.GetClosestMultiple(mousePosition.x, editorReference.GetGridSize()), Editor.GetClosestMultiple(mousePosition.y, editorReference.GetGridSize())); ;
        GameObject objectHere = Editor.FindObjectAtPosition(gridPosition);
        bool bCanPlace = false;
        
        for(int x =0; x< currentTrap.GetComponent<PuzzleObjectBase>().wireList.Count; x++)
        {
            if (!currentTrap.GetComponent<PuzzleObjectBase>().wireList[x].GetComponent<Wire>().UpdatePosition())//Updates current wires, if any must be deleted (in the update function) it lowers index to allow for enumeration with a changed list
            {
                x--;
            }
        }
        switch (currentTrapName)
        {
            case TrapName.PressurePlate://-------------------------------------------------------
                if (objectHere != null)
                {
                    if(objectHere.tag == "Tile")
                    {
                        objectHere.GetComponent<Tile>().FindContainedObjects();
                        if (objectHere.GetComponent<Tile>().containedObjects.Count==0 || (objectHere.GetComponent<Tile>().containedObjects.Count == 1 && objectHere.GetComponent<Tile>().containedObjects.Contains(currentTrap)))
                        {
                            currentTrap.transform.position = gridPosition;
                            currentTrap.GetComponent<SpriteRenderer>().color = Color.white;
                            bCanPlace = true;
                        }
                        else
                        {
                            currentTrap.GetComponent<SpriteRenderer>().color = cannotPlaceColour;
                        }
                    }
                    else
                    {
                        currentTrap.GetComponent<SpriteRenderer>().color = cannotPlaceColour;
                        currentTrap.transform.position = mousePosition;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if(currentlySelectedObject == null)
                    {
                        levelControllerReference.iNumberPressurePlates++;
                        UpdateUIValues();
                    }
                    currentState = UIState.None;
                    Destroy(currentTrap);
                    currentTrap = null;
                }
                break;//------------------------------------------------------------
            case TrapName.RemoteExplosive:
                currentTrap.transform.position = mousePosition;
                if (objectHere != null)
                {
                    if (objectHere.tag == "Tile")
                    {
                        currentTrap.GetComponent<SpriteRenderer>().color = Color.white;
                        bCanPlace = true;
                    }
                    else
                    {
                        currentTrap.GetComponent<SpriteRenderer>().color = cannotPlaceColour;
                    }
                }
                else
                {
                    currentTrap.GetComponent<SpriteRenderer>().color = cannotPlaceColour;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    if (currentlySelectedObject == null)
                    {
                        levelControllerReference.iNumberRemoteExplosives++;
                        UpdateUIValues();
                    }
                    currentState = UIState.None;
                    Destroy(currentTrap);
                    currentTrap = null;
                }
                break;//-------------------------------------------------------------
            default:
                break;
        }

        if (bCanPlace)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentlySelectedObject != null)
                {
                    Destroy(currentlySelectedObject);
                    currentlySelectedObject = null;
                }
                currentState = UIState.None;
                currentTrap.transform.parent = levelControllerReference.currentLevelObject.transform;
                currentTrap.GetComponent<SpriteRenderer>().sortingOrder = 2;
                currentTrap.layer = 0;
                foreach (GameObject w in currentTrap.GetComponent<PuzzleObjectBase>().wireList)
                {
                    w.GetComponent<Wire>().SetCollision(true);
                    w.GetComponent<Wire>().UpdatePosition();
                }
                if (currentTrap.GetComponent<RemoteExplosive>() != null)
                {
                    currentTrap.GetComponent<RemoteExplosive>().InitializeExplosive();
                }
                currentTrap = null;

            }
        }
        else
        {
            currentTrap.transform.position = mousePosition;
        }

    }

    private void HighlightObject(GameObject obj)
    {
        if(currentlyHighlightedObject != null && currentlyHighlightedObject!= obj)
        {
            UnHighlightObject(currentlyHighlightedObject);
        }
        if(obj.GetComponent<SpriteRenderer>() != null)
        {
            obj.GetComponent<SpriteRenderer>().color = highlightColour;
        }else if(obj.GetComponent<LineRenderer>() != null)
        {
            if (obj.GetComponent<Wire>().bSelectable)
            {
                obj.GetComponent<LineRenderer>().startColor = new Color(1, 1, 1, 1f);
                obj.GetComponent<LineRenderer>().endColor = new Color(1, 1, 1, 1f);
            }
            else
            {
                obj.GetComponent<LineRenderer>().startColor = new Color(1, 0.5f, 0.5f, 1f);
                obj.GetComponent<LineRenderer>().endColor = new Color(1, 0.5f, 0.5f, 1f);
            }
        }
        if(obj.GetComponent<Explosive>() != null)
        {
            obj.GetComponent<Explosive>().ShowRadius();
        }

        currentlyHighlightedObject = obj;
        //ShowPopup(currentlyHighlightedObject.transform.position, obj.GetComponent<IHasDescription>().strDescription);

    }

    private void UnHighlightObject(GameObject obj = null)
    {
        if (obj != null)
        {
            if (obj.GetComponent<SpriteRenderer>() != null)
            {
                obj.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else if (obj.GetComponent<LineRenderer>() != null)
            {
                obj.GetComponent<LineRenderer>().startColor = new Color(0, 0, 0, 0.4f);
                obj.GetComponent<LineRenderer>().endColor = new Color(0, 0, 0, 0.4f);
            }
            if (obj.GetComponent<Explosive>() != null)
            {
                obj.GetComponent<Explosive>().HideRadius();
            }
        }
        currentlyHighlightedObject = null;
        ClosePopup();
    }

    private void ShowPopup(Vector2 position, string text)
    {
        Vector2 offsetHeight;
        Vector2 positionUsed = position;
        if (currentlyHighlightedObject.tag == "Wire") 
        {
            if (currentlyHighlightedObject.GetComponent<Wire>().UpdatePosition())//Returns true if the wire still has destination and origin intact 
            {
                offsetHeight = Vector2.zero;
                positionUsed = (currentlyHighlightedObject.GetComponent<LineRenderer>().GetPosition(0) + currentlyHighlightedObject.GetComponent<LineRenderer>().GetPosition(1)) / 2;
                //Show Wire text tags----
                txtWireActivate.GetComponent<TextMeshProUGUI>().text = currentlyHighlightedObject.GetComponent<Wire>().origin.GetComponent<IHasDescription>().strActivateDescrip;
                txtWireTrigger.GetComponent<TextMeshProUGUI>().text = currentlyHighlightedObject.GetComponent<Wire>().destination.GetComponent<IHasDescription>().strTriggerDescrip;
                Vector2 wireLocalPos;
                Vector2 wirePos = mainCam.WorldToScreenPoint(currentlyHighlightedObject.GetComponent<LineRenderer>().GetPosition(0));
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasReference.GetComponent<RectTransform>(), wirePos, null, out wireLocalPos);
                txtWireActivate.transform.localPosition = wireLocalPos;
                wirePos = mainCam.WorldToScreenPoint(currentlyHighlightedObject.GetComponent<LineRenderer>().GetPosition(1));
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasReference.GetComponent<RectTransform>(), wirePos, null, out wireLocalPos);
                txtWireTrigger.transform.localPosition = wireLocalPos;

                txtWireActivate.SetActive(true);
                txtWireTrigger.SetActive(true);
            }
        }
        else 
        { 
            offsetHeight = new Vector2(0, currentlyHighlightedObject.GetComponent<SpriteRenderer>().bounds.extents.y * 1.5f);

            //This was below the if statement
            Vector2 objectPos = mainCam.WorldToScreenPoint(positionUsed + offsetHeight);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasReference.GetComponent<RectTransform>(), objectPos, null, out localPoint);
            PopupBox.SetActive(true);
            PopupBox.GetComponent<PopupBoxHandler>().ShowText(text, localPoint);
        }
        
        
    }

    private void ClosePopup()
    {
        if (PopupBox.activeSelf) { PopupBox.SetActive(false); }
        CloseWireText();
    }

    private void CloseWireText()
    {
        txtWireActivate.SetActive(false);
        txtWireTrigger.SetActive(false);
    }

    private void ShowToolbar(GameObject obj)
    {
        Vector2 positionUsed = obj.transform.position - new Vector3(0, obj.GetComponent<SpriteRenderer>().bounds.extents.y, 0);
        Vector2 objectPosition = mainCam.WorldToScreenPoint(positionUsed);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasReference.GetComponent<RectTransform>(), objectPosition, null, out localPoint);
        itemToolbar.GetComponent<ItemToolbarHandler>().ShowToolbar(localPoint, obj);
    }

    private void CloseToolbar()
    {
        itemToolbar.SetActive(false);
    }

    //MISC functions
    public static RaycastHit2D GetTopObject(Vector3 position)
    {
        RaycastHit2D topObjectHit;
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
        topObjectHit = rayHit;
        List<RaycastHit2D> foundRayHits = new List<RaycastHit2D>();

        if (rayHit!= false && rayHit.collider != null)
        {
            rayHit.collider.gameObject.layer = 2; //ignore raycast layer 
            foundRayHits.Add(rayHit);
        }

        while (rayHit!=false)//find all objects at this point and select the one with the highest sorting order;
        {
            rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
            if(rayHit !=false && rayHit.collider != null)
            {
                rayHit.collider.gameObject.layer = 2;
                if(rayHit.collider.gameObject.GetComponent<Renderer>().sortingOrder > topObjectHit.collider.GetComponent<Renderer>().sortingOrder)
                {
                    topObjectHit = rayHit;
                }
                foundRayHits.Add(rayHit);
            }
        }
        foreach(RaycastHit2D ray in foundRayHits)
        {
            if (ray.collider.gameObject.tag == "Wall")
            {
                ray.collider.gameObject.layer = LayerMask.NameToLayer("Wall");
            }
            else
            {
                ray.collider.gameObject.layer = 0; //Sets them back to the default layer, if they were on a diff layer before - this needs to be changed.
            }
        }

        return topObjectHit;
    }
}
