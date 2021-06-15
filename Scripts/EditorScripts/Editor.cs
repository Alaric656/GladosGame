using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    private ToolkitController toolkitReference;
    private GameController gameControllerReference;
    private LevelController levelControllerReference;
    private UICanvas canvasReference;

    private bool bPlacingTileMode;
    private GameObject placementMarker;
    private float fPlacementTimer;
    private bool bPlacingOnGrid;

    private bool bWiringMode;
    private GameObject wireStartObject;
    private GameObject wireEndObject;

    public Color highlightColour;
    private GameObject currentlyHighlighted;

    public Color wireHighlightColour;

    //public GameObject TestTileType;
    public GameObject levelHolder; //This is a prefab to keep every element in a level as its child, so that a level is easy to compartmentalize
    private GameObject currentLevel; //This stores the current levelHolder being worked on.

    public GameObject wirePrefab;

    //Constants--------
    private const float fGridSize = 1f; //The size of the grid tiles are placed on
    private const float fPlacementDelay = 0.1f; //Seconds between being able to place new tiles
    //------------
    //Misc variables
    private RaycastHit2D rayHit;
    private int iCounter;
    //---------------
    public enum ObjectType
    {
        Tile,
        Wall,
        Doodad
    }

    public float GetGridSize()
    {
        return fGridSize;
    }

    void Start()
    {

    }

    public void Initialize()
    {
        toolkitReference = GameObject.Find("Toolkit").GetComponent<ToolkitController>();
        gameControllerReference = GameObject.Find("GameController").GetComponent<GameController>();
        levelControllerReference = GameObject.Find("LevelController").GetComponent<LevelController>();
        canvasReference = GameObject.Find("Canvas").GetComponent<UICanvas>();
        toolkitReference.Initialize();
        bPlacingTileMode = false;
        bWiringMode = false;
        bPlacingOnGrid = true;
        gameObject.SetActive(false);
    }

    public void StartEditor()
    {
        gameObject.SetActive(true);
        if (currentLevel == null) { CreateNewLevel(); }
        toolkitReference.ActivateToolkit();

    }

    public void CloseEditor()
    {
        toolkitReference.DeactivateToolkit();
        gameObject.SetActive(false);
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
    }

    public void CreateNewLevel()
    {
        currentLevel = Instantiate(levelHolder);
    }
    
    void Update()
    {
        //-------------------------------Placing Wires mode------------------------------------------
        if (bWiringMode)
        {
            if (Input.GetMouseButtonDown(0) && canvasReference.BlockedByUI == false)
            {
                //rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                rayHit = MainUIController.GetTopObject(Input.mousePosition);
                if (rayHit != false && rayHit.collider != null)
                {
                    if(rayHit.collider.gameObject.GetComponent<PuzzleObjectBase>() != null)
                    {
                        wireStartObject = rayHit.collider.gameObject;
                        wireStartObject.GetComponent<SpriteRenderer>().color = wireHighlightColour;
                    }
                }
            }else if (wireStartObject !=null && Input.GetMouseButtonUp(0))
            {
                //rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                rayHit = MainUIController.GetTopObject(Input.mousePosition);
                if (rayHit != false && rayHit.collider != null && canvasReference.BlockedByUI == false)
                {
                    if (rayHit.collider.gameObject.GetComponent<PuzzleObjectBase>() != null && rayHit.collider.gameObject !=wireStartObject)
                    {
                        wireEndObject = rayHit.collider.gameObject;
                        wireEndObject.GetComponent<SpriteRenderer>().color = wireHighlightColour;

                        GameObject newWire = Instantiate(wirePrefab);

                        newWire.GetComponent<Wire>().Initialize(wireStartObject, wireEndObject, false,true,true);
                        newWire.transform.parent = wireStartObject.transform;

                        wireStartObject.GetComponent<PuzzleObjectBase>().AddWire(newWire);
                        //TODO Offer different types of wiring based on what an item can actually do. 

                        wireStartObject.GetComponent<SpriteRenderer>().color = Color.white;
                        wireEndObject.GetComponent<SpriteRenderer>().color = Color.white;
                        //Debug.DrawLine(wireStartObject.transform.position, wireEndObject.transform.position, Color.black, 100, false);//draw the wire in debugging
                        wireStartObject = null;
                        wireEndObject = null;
                           
                    }
                }
                wireStartObject = null;
                wireEndObject = null;
            }
            if (Input.GetMouseButtonDown(1))
            {
                SetWiring(false);
            }
        }

        //------------Placing Tile Mode------------------------------------
        else if (bPlacingTileMode == true)
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2 newTilePosition = mousePosition;
            if (bPlacingOnGrid)
            {
                Vector2 gridPosition = new Vector2(GetClosestMultiple(mousePosition.x, fGridSize), GetClosestMultiple(mousePosition.y, fGridSize));

                if (placementMarker.gameObject.tag == "WallHorizontal" || placementMarker.gameObject.tag == "DoorHorizontal")
                {
                    gridPosition = new Vector2(GetClosestMultiple(mousePosition.x, fGridSize), GetClosestMultiple(mousePosition.y, fGridSize, fGridSize / 2));
                }
                if (placementMarker.gameObject.tag == "WallVertical" || placementMarker.gameObject.tag == "DoorVertical")
                {
                    gridPosition = new Vector2(GetClosestMultiple(mousePosition.x, fGridSize, fGridSize / 2), GetClosestMultiple(mousePosition.y, fGridSize));
                }

                placementMarker.transform.position = gridPosition;
                newTilePosition = gridPosition;
            }
            else
            {
                placementMarker.transform.position = mousePosition;
            }

            if (fPlacementTimer > 0)
            {
                fPlacementTimer -= Time.deltaTime;//This timer adds a delay to prevent new tiles from being placed every single frame if the button is held down.
            }

            if (Input.GetMouseButton(0) && fPlacementTimer<=0 && canvasReference.BlockedByUI == false)
            {
                //Check if there is already an item of the same kind in this spot--
                //rayHit = Physics2D.Raycast(gridPosition, Vector2.zero);
                rayHit = MainUIController.GetTopObject(Input.mousePosition);
                if (rayHit != false && rayHit.collider != null)
                {
                    if (placementMarker.gameObject.tag == "Tile" || placementMarker.gameObject.tag == "Wall" || placementMarker.gameObject.tag == "PuzzleTile")
                    { 
                        if (rayHit.collider.gameObject.tag == "Tile" || rayHit.collider.gameObject.tag == "Wall" || rayHit.collider.gameObject.tag == "PuzzleTile")
                        {
                            Destroy(rayHit.collider.gameObject);
                        }
                    }
                    else if(rayHit.collider.gameObject.tag == placementMarker.gameObject.tag)
                    {
                        Destroy(rayHit.collider.gameObject);
                    }
                }
                //---------
                GameObject newTile = Instantiate(placementMarker);
                newTile.transform.parent = currentLevel.transform;//Sets the new item as a child of the levelHolder
                newTile.transform.position = newTilePosition;
                newTile.name = placementMarker.name;
                newTile.layer = 0; //Set the new tiles to regular layer so they don't ignore raycast
                newTile.GetComponent<SpriteRenderer>().sortingOrder = 0;
                if (placementMarker.gameObject.tag != "Tile" && placementMarker.gameObject.tag != "PuzzleTile")
                {
                    switch (placementMarker.gameObject.tag)
                    {
                        case "Enemy":
                            newTile.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            break;
                        default://Puzzle objects all have different tags, so this is their section
                            newTile.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            break;
                    }
                }
                if (newTile.GetComponent<PuzzleObjectBase>() != null)
                {
                    for(iCounter =0;  iCounter<newTile.GetComponent<PuzzleObjectBase>().wireList.Count; iCounter++)
                    {//Updating position of wirelist 
                        //If it fails to update position, iCounter is reduced to accommodate the reduced length of wirelist
                        if (!newTile.GetComponent<PuzzleObjectBase>().wireList[iCounter].GetComponent<Wire>().UpdatePosition()) 
                        {
                            iCounter--;
                        }
                    }
                }
                fPlacementTimer = fPlacementDelay;
            }
            if (Input.GetMouseButtonDown(1))
            {
                StopPlacingTile();
            }
        }
        else//----------------Other things editor can do when not in any particular mode------------------------------------------------------
        {
            //--Highlight mouse-hovered elements--
            if (canvasReference.BlockedByUI == false)
            {
                //rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                rayHit = MainUIController.GetTopObject(Input.mousePosition);
                if (rayHit != false && rayHit.collider != null && rayHit.collider.gameObject.tag != "Wire")
                {
                    if (currentlyHighlighted == null)
                    {
                        currentlyHighlighted = rayHit.collider.gameObject;
                        currentlyHighlighted.GetComponent<SpriteRenderer>().color = highlightColour;
                    }
                    else if (currentlyHighlighted != rayHit.collider.gameObject)
                    {
                        currentlyHighlighted.GetComponent<SpriteRenderer>().color = Color.white;
                        currentlyHighlighted = rayHit.collider.gameObject;
                        currentlyHighlighted.GetComponent<SpriteRenderer>().color = highlightColour;
                    }
                }
                else if (currentlyHighlighted != null)
                {
                    currentlyHighlighted.GetComponent<SpriteRenderer>().color = Color.white;
                    currentlyHighlighted = null;
                }
                //------------------------
                if (Input.GetMouseButtonUp(0)) //Pick up and move existing tiles
                {
                    //rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    rayHit = MainUIController.GetTopObject(Input.mousePosition);
                    if (rayHit != false && rayHit.collider != null)
                    {
                        // if (rayHit.collider.gameObject.tag == "Tile")
                        //{
                        rayHit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white; //Changes hovered element back to normal colour
                        StartPlacingTile(rayHit.collider.gameObject);
                        Destroy(rayHit.collider.gameObject);
                        // }
                    }
                }
            }
            else if(currentlyHighlighted!=null)
            {
                currentlyHighlighted.GetComponent<SpriteRenderer>().color = Color.white;
                currentlyHighlighted = null;
            }
        }
    }

    public void StartPlacingTile(GameObject tileType)
    {
        if(bPlacingTileMode == false && bWiringMode == false)
        {
            if(tileType.gameObject.tag == "NonGridPuzzleObject")
            {
                bPlacingOnGrid = false;
            }
            else
            {
                bPlacingOnGrid = true; 
            }
            bPlacingTileMode = true;
            placementMarker = Instantiate(tileType);
            placementMarker.name = tileType.name;
            placementMarker.GetComponent<SpriteRenderer>().sortingOrder = 10;
            placementMarker.layer = 2; //Set to 'ignore raycast layer' so tiles underneath can be checked by mouse click
        }
    }

    public void StopPlacingTile()
    {
        if (bPlacingTileMode == true)
        {
            bPlacingTileMode = false;
            Destroy(placementMarker);
        }
    }

    public void ToggleWiring()
    {
        SetWiring(!bWiringMode);
    }

    public void SetWiring(bool wiringMode=true)
    {
        if (wiringMode)
        {
            bPlacingTileMode = false;
        }
        else
        {
            if (wireStartObject != null)
            {
                wireStartObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            if (wireEndObject != null)
            {
                wireEndObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            wireStartObject = null;
            wireEndObject = null;
        }
        bWiringMode = wiringMode;
    }

    public void TestLevel()
    {
        SaveLevel("RecentTest");
        gameControllerReference.TestLevel("RecentTest");
        
    }

    public void StopTestingLevel()
    {
        StartEditor();
        //LoadLevel("RecentTest");
        Level loadedLevel = LoadUtils.LoadLevel("recentTest");
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        currentLevel = LoadLevel(loadedLevel);
    }


    public void SaveEditorLevel()
    {
        SaveLevel("test");
        //TODO, choose name of saved level taken from text box
    }

    public void LoadEditorLevel()
    {
        //LoadLevel("test");
        Level loadedLevel = LoadUtils.LoadLevel("test");
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        currentLevel = LoadLevel(loadedLevel);
        //TODO, allow other levels to be loaded

    }

    public void SaveLevel(string levelName = "unnamed")
    {
        Level testLevel = new Level(levelName);
        foreach(Transform tile in currentLevel.transform)
        {
            //UnityEngine.Debug.Log(tile);
            if (tile.gameObject.tag == "Goal" || tile.gameObject.tag == "Door" || tile.gameObject.tag == "Switch" || tile.gameObject.tag == "PuzzleTile" || tile.gameObject.tag == "ExplosiveBarrel")
            {
                testLevel.AddPuzzleObject(tile.gameObject);
                foreach(GameObject wire in tile.GetComponent<PuzzleObjectBase>().wireList)
                {
                    testLevel.AddWire(wire);
                }
            }
            else if (tile.gameObject.tag == "Enemy")
            {
                testLevel.AddCharacter(tile.gameObject);
            }
            else if(tile.gameObject.tag == "Tile" || tile.gameObject.tag == "Wall")
            {
                testLevel.AddTile(tile.gameObject);
            }
        }

        //Gets the value from the inp field and converts it to a positive int
        testLevel.iPressurePlates = Mathf.Abs(int.Parse(toolkitReference.fldPressurePlates.text)); 
        testLevel.iRemoteExplosives = Mathf.Abs(int.Parse(toolkitReference.fldRemoteExplosives.text));

        SaveUtils.SaveLevel(testLevel);
        Debug.Log("Finished saving level: " + testLevel);
    }

    public GameObject LoadLevel(Level loadedLevel)//string levelName = "unnamed"
    {
        /*if(currentLevel != null)
        {
            Destroy(currentLevel);
        }*/
        GameObject currentLevel = Instantiate(levelHolder);
        //Level loadedLevel = LoadUtils.LoadLevel(levelName);

        List<TerrainTile> tileList = loadedLevel.GetTileList();
        foreach (TerrainTile tile in tileList)
        {
            GameObject newTile = Instantiate(Resources.Load<GameObject>("Tiles/" + tile.strTypeName));
            newTile.transform.position = tile.vPosition;
            newTile.transform.parent = currentLevel.transform;
            newTile.gameObject.name = tile.strTypeName;
            if (newTile.gameObject.tag != "Tile")//Doodads would presumable go through here
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
        List<GameObject> puzzleObjects = new List<GameObject>();//This keeps track of all puzzle objects loaded so the wires can search through them
        List<PuzzleObject> puzzleObjectList = loadedLevel.GetPuzzleObjectList();
        foreach (PuzzleObject tile in puzzleObjectList)
        {
            
            GameObject newTile = Instantiate(Resources.Load<GameObject>("PuzzleObjects/" + tile.strTypeName));
            newTile.transform.position = tile.vPosition;
            newTile.transform.parent = currentLevel.transform;
            newTile.gameObject.name = tile.strTypeName;
            puzzleObjects.Add(newTile);
            if (newTile.gameObject.tag != "Tile")//Tile based puzzle objects need this
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
        }

        List<Character> characterList = loadedLevel.GetCharacterList();
        foreach (Character tile in characterList)
        {
            GameObject newTile = Instantiate(Resources.Load<GameObject>("Characters/" + tile.strTypeName));
            newTile.transform.position = tile.vPosition;
            newTile.transform.parent = currentLevel.transform;
            newTile.gameObject.name = tile.strTypeName;

            newTile.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

        List<WireData> wireList = loadedLevel.GetWireList();//wires should be loaded last
        foreach (WireData wire in wireList)
        {
            GameObject newWire = Instantiate(Resources.Load<GameObject>("Abstracts/WirePrefab"));

            //Find origin and destination objects
            GameObject startObject = null;
            GameObject endObject = null;
            for(iCounter=0; iCounter < puzzleObjects.Count; iCounter++)
            {
                if ((Vector2)puzzleObjects[iCounter].transform.position == wire.origin.vPosition && puzzleObjects[iCounter].name == wire.origin.strTypeName)
                {
                    startObject = puzzleObjects[iCounter];
                }
            }
            for (iCounter = 0; iCounter < puzzleObjects.Count; iCounter++)
            {
                if ((Vector2)puzzleObjects[iCounter].transform.position == wire.destination.vPosition && puzzleObjects[iCounter].name == wire.destination.strTypeName)
                {
                    endObject = puzzleObjects[iCounter];
                }
            }

            if (startObject!=null && endObject != null)
            {
                if(startObject.GetComponent<PuzzleObjectBase>() !=null && endObject.GetComponent<PuzzleObjectBase>() != null)
                {
                    newWire.GetComponent<Wire>().Initialize( startObject, endObject , wire.bSelectable, wire.bVisible, wire.bUsableByAi);
                    newWire.gameObject.transform.parent = startObject.transform;
                    //Debug.DrawLine(startObject.transform.position,endObject.transform.position,Color.black,100,false);//draw the wire in debugging
                    startObject.GetComponent<PuzzleObjectBase>().AddWire(newWire);
                }
            }
        }

        toolkitReference.fldPressurePlates.text = loadedLevel.iPressurePlates.ToString();
        toolkitReference.fldRemoteExplosives.text = loadedLevel.iRemoteExplosives.ToString();

        Debug.Log("Finished loading level: " + loadedLevel);
        return currentLevel;
    }


    
    //Temporary Maths Tools------------------

    public static float GetClosestMultiple(float numberToRound, float multiple, float offset=0f)
    {
        if (multiple == 0)
        {
            return numberToRound;
        }
        //float remainder = numberToRound % multiple; <-- actually the remainder function and doesn't work in some cases  
        float remainder = numberToRound - (multiple * Mathf.Floor(numberToRound / multiple)); //<-- actual modulus formula
        if (remainder < multiple / 2 && remainder > -multiple / 2)//0.55, -0.55
        {

            return numberToRound - remainder + offset;
        }
        else
        {
            return numberToRound + multiple - remainder - offset;
        }
    }

    public static GameObject FindObjectAtPosition(Vector2 position)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(position, Vector2.zero);
        if (rayHit != false && rayHit.collider != null)
        {
            return rayHit.collider.gameObject;
        }
        return null;
    }

  
}
