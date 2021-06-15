using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
public class ToolkitController : MonoBehaviour
{

    private Editor editorReference;

    //UIPrefabs
    public GameObject btnTilePrefab;
    public GameObject tglSectionPrefab;
    //---

    //The Actual data
    public GameObject[] groundTiles;
    public GameObject[] wallTiles;
    public GameObject[] puzzlePieces;
    public GameObject[] characters;

    public InputField fldPressurePlates;  //The inputfield that stores how many pressure plates this level gives the player. etc..
    public InputField fldRemoteExplosives;

    //-------
    //Button holders
    private GameObject tglTerrainSection;
    private bool bTerrainSectionEnabled;
    private GameObject[] buttons_groundTiles;

    private GameObject tglWallSection;
    private bool bWallSectionEnabled;
    private GameObject[] buttons_wallTiles;

    private GameObject tglPuzzleSection;
    private bool bPuzzleSectionEnabled;
    private GameObject[] buttons_puzzlePieces;

    private GameObject tglCharacterSection;
    private bool bCharacterSectionEnabled;
    private GameObject[] buttons_characters;
    //--------


    private bool bToolkitMoving;
    private Vector2 toolkitMoveOffset;
    private const float bufferPercentage = 0.2f; //% of the toolkit that must remain on screen to prevent losing it off-screen || 0.2 = 20%

    private GameObject scrollTiles;
    private GameObject mainPanel;
    private GameObject playerItems;

    private int iCounter;

    void Start()
    {
        //testBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>testbtnfunction("wahoo"));

        //gameObject.SetActive(false);
    }

    public void Initialize()
    {
        bTerrainSectionEnabled = false;
        editorReference = GameObject.Find("EditorController").GetComponent<Editor>();
        //setup references
        scrollTiles = transform.Find("Scroll_Tiles").gameObject;
        mainPanel = transform.Find("Main_Panel").gameObject;
        playerItems = transform.Find("Player_Items").gameObject;
        fldPressurePlates = GameObject.Find("fld_PressurePlate").GetComponent<InputField>();
        fldRemoteExplosives = GameObject.Find("fld_RemoteExplosive").GetComponent<InputField>();
        ChangeMenu(0);

        CreateButtons();
        gameObject.SetActive(false);
    }

    public void ActivateToolkit()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateToolkit()
    {
        gameObject.SetActive(false);
    }

    private void CreateButtons()
    {
        buttons_groundTiles = new GameObject[groundTiles.Length];
        tglTerrainSection = Instantiate(tglSectionPrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
        tglTerrainSection.transform.Find("Label").GetComponent<UnityEngine.UI.Text>().text = "Terrain Tiles";
        tglTerrainSection.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((bool value) =>  ToggleSection(0, value));

        for (iCounter = 0; iCounter < groundTiles.Length; iCounter++)
        {
            if (groundTiles[iCounter] != null)
            {
                buttons_groundTiles[iCounter] = Instantiate(btnTilePrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
                //buttons_groundTiles[iCounter].GetComponent<UnityEngine.UI.Button>().image.sprite = groundTiles[iCounter].gameObject.GetComponent<SpriteRenderer>().sprite;
                buttons_groundTiles[iCounter].GetComponent<Toolkit_objectButton>().SetObject(groundTiles[iCounter]);
                buttons_groundTiles[iCounter].SetActive(false);
            }
            
        }

        buttons_wallTiles = new GameObject[wallTiles.Length];
        tglWallSection = Instantiate(tglSectionPrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
        tglWallSection.transform.Find("Label").GetComponent<UnityEngine.UI.Text>().text = "Wall Tiles";
        tglWallSection.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((bool value) => ToggleSection(1, value));

        for (iCounter = 0; iCounter < wallTiles.Length; iCounter++)
        {
            if (wallTiles[iCounter] != null)
            {
                buttons_wallTiles[iCounter] = Instantiate(btnTilePrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
                buttons_wallTiles[iCounter].GetComponent<Toolkit_objectButton>().SetObject(wallTiles[iCounter]);
                buttons_wallTiles[iCounter].SetActive(false);
            }

        }

        buttons_puzzlePieces = new GameObject[puzzlePieces.Length];
        tglPuzzleSection = Instantiate(tglSectionPrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
        tglPuzzleSection.transform.Find("Label").GetComponent<UnityEngine.UI.Text>().text = "Puzzle Pieces";
        tglPuzzleSection.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((bool value) => ToggleSection(2, value));

        for (iCounter = 0; iCounter < puzzlePieces.Length; iCounter++)
        {
            if (puzzlePieces[iCounter] != null)
            {
                buttons_puzzlePieces[iCounter] = Instantiate(btnTilePrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
                buttons_puzzlePieces[iCounter].GetComponent<Toolkit_objectButton>().SetObject(puzzlePieces[iCounter]);
                buttons_puzzlePieces[iCounter].SetActive(false);
            }

        }

        buttons_characters = new GameObject[characters.Length];
        tglCharacterSection = Instantiate(tglSectionPrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
        tglCharacterSection.transform.Find("Label").GetComponent<UnityEngine.UI.Text>().text = "Characters";
        tglCharacterSection.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((bool value) => ToggleSection(3, value));

        for (iCounter = 0; iCounter < characters.Length; iCounter++)
        {
            if (puzzlePieces[iCounter] != null)
            {
                buttons_characters[iCounter] = Instantiate(btnTilePrefab, scrollTiles.transform.Find("Viewport").transform.Find("Content"));
                buttons_characters[iCounter].GetComponent<Toolkit_objectButton>().SetObject(characters[iCounter]);
                buttons_characters[iCounter].SetActive(false);
            }

        }


        ArrangeToolkitButtons();
    }

    public void SelectTileToBuild(GameObject newTile)
    {
        editorReference.StartPlacingTile(newTile);
    }

    void Update()
    {
        if (bToolkitMoving)
        {
            MouseMoveToolkit();
        }
    }


    public void ToggleSection(int sectionID, bool isOn)
    {
        switch (sectionID)
        {
            case 0:
                bTerrainSectionEnabled = isOn;
                for(iCounter = 0; iCounter < buttons_groundTiles.Length; iCounter++)
                {
                    buttons_groundTiles[iCounter].SetActive(isOn);
                    
                }
                break;
            case 1:
                bWallSectionEnabled = isOn;
                for (iCounter = 0; iCounter < buttons_wallTiles.Length; iCounter++)
                {
                    buttons_wallTiles[iCounter].SetActive(isOn);
                }
                break;
            case 2:
                bPuzzleSectionEnabled = isOn;
                for (iCounter = 0; iCounter < buttons_puzzlePieces.Length; iCounter++)
                {
                    buttons_puzzlePieces[iCounter].SetActive(isOn);

                }
                break;
            case 3:
                bCharacterSectionEnabled = isOn;
                for (iCounter = 0; iCounter < buttons_characters.Length; iCounter++)
                {
                    buttons_characters[iCounter].SetActive(isOn);

                }
                break;
        }
        ArrangeToolkitButtons();
    }


    public void ChangeMenu(int newState)
    {
        switch (newState)
        {
            case 0:
                scrollTiles.SetActive(false);
                mainPanel.SetActive(true);
                playerItems.SetActive(false);
                break;
            case 1:
                mainPanel.SetActive(false);
                scrollTiles.SetActive(true);
                playerItems.SetActive(false);
                break;
            case 2:
                mainPanel.SetActive(false);
                scrollTiles.SetActive(false);
                playerItems.SetActive(true);
                break;
            default:
                break;

        }
    }


    private void ArrangeToolkitButtons()
    {
        float buttonSize = buttons_groundTiles[0].GetComponent<RectTransform>().rect.width;
        float buttonBuffer = buttonSize * 0.2f; //Space between each button
        float arrangeYPosition = (scrollTiles.transform.Find("Viewport").transform.Find("Content").gameObject.GetComponent<RectTransform>().rect.height / 2f) - buttonBuffer; //Current Y position, starting at the top of the toolkit area
        arrangeYPosition = -buttonBuffer; //Overriding the Y start position since I moved the pivot to the top
        float arrangeXLeftMargin = (-scrollTiles.transform.Find("Viewport").transform.Find("Content").gameObject.GetComponent<RectTransform>().rect.width / 2f) + buttonBuffer;

        int arrangeXMaxNumber = (int)Mathf.Floor(scrollTiles.transform.Find("Viewport").transform.Find("Content").gameObject.GetComponent<RectTransform>().rect.width / ((buttons_groundTiles[0].GetComponent<RectTransform>().rect.width + buttonBuffer) + buttonBuffer));
        int arrangeXIndex; //Max no. of buttons per row, and current index on row. 

        //---Positioning terrain tile section----------
        tglTerrainSection.transform.localPosition = new Vector2(arrangeXLeftMargin, arrangeYPosition);
        arrangeYPosition -= tglTerrainSection.GetComponent<RectTransform>().rect.height + buttonBuffer;
        if (bTerrainSectionEnabled)
        {
            arrangeXIndex = 0;
            for (iCounter = 0; iCounter < buttons_groundTiles.Length; iCounter++)
            {
                buttons_groundTiles[iCounter].transform.localPosition = new Vector2(arrangeXLeftMargin + ((buttonBuffer + buttonSize) * arrangeXIndex), arrangeYPosition);
                arrangeXIndex++;
                if (arrangeXIndex >= arrangeXMaxNumber && iCounter + 1 < buttons_groundTiles.Length)
                {
                    arrangeXIndex = 0;
                    arrangeYPosition -= buttonSize + buttonBuffer;
                }
            }
            arrangeYPosition -= buttonSize + buttonBuffer;
        }
        //---Positioning Wall tile section-----------
        tglWallSection.transform.localPosition = new Vector2(arrangeXLeftMargin, arrangeYPosition);
        arrangeYPosition -= tglWallSection.GetComponent<RectTransform>().rect.height + buttonBuffer;
        if (bWallSectionEnabled)
        {
            arrangeXIndex = 0;
            for (iCounter = 0; iCounter < buttons_wallTiles.Length; iCounter++)
            {
                buttons_wallTiles[iCounter].transform.localPosition = new Vector2(arrangeXLeftMargin + ((buttonBuffer + buttonSize) * arrangeXIndex), arrangeYPosition);
                arrangeXIndex++;
                if (arrangeXIndex >= arrangeXMaxNumber && iCounter+1 < buttons_wallTiles.Length)
                {
                    arrangeXIndex = 0;
                    arrangeYPosition -= buttonSize + buttonBuffer;
                }
            }
            arrangeYPosition -= buttonSize + buttonBuffer;
        }
        //-----Positioning Puzzle Pieces section-----------

        tglPuzzleSection.transform.localPosition = new Vector2(arrangeXLeftMargin, arrangeYPosition);
        arrangeYPosition -= tglPuzzleSection.GetComponent<RectTransform>().rect.height + buttonBuffer;
        if (bPuzzleSectionEnabled)
        {
            arrangeXIndex = 0;
            for (iCounter = 0; iCounter < buttons_puzzlePieces.Length; iCounter++)
            {
                buttons_puzzlePieces[iCounter].transform.localPosition = new Vector2(arrangeXLeftMargin + ((buttonBuffer + buttonSize) * arrangeXIndex), arrangeYPosition);
                arrangeXIndex++;
                if (arrangeXIndex >= arrangeXMaxNumber && iCounter + 1 < buttons_puzzlePieces.Length)
                {
                    arrangeXIndex = 0;
                    arrangeYPosition -= buttonSize + buttonBuffer;
                }
            }
            arrangeYPosition -= buttonSize + buttonBuffer;
        }

        //-----Positioning Characters section---------

        tglCharacterSection.transform.localPosition = new Vector2(arrangeXLeftMargin, arrangeYPosition);
        arrangeYPosition -= tglCharacterSection.GetComponent<RectTransform>().rect.height + buttonBuffer;
        if (bCharacterSectionEnabled)
        {
            arrangeXIndex = 0;
            for (iCounter = 0; iCounter < buttons_characters.Length; iCounter++)
            {
                buttons_characters[iCounter].transform.localPosition = new Vector2(arrangeXLeftMargin + ((buttonBuffer + buttonSize) * arrangeXIndex), arrangeYPosition);
                arrangeXIndex++;
                if (arrangeXIndex >= arrangeXMaxNumber && iCounter + 1 < buttons_characters.Length)
                {
                    arrangeXIndex = 0;
                    arrangeYPosition -= buttonSize + buttonBuffer;
                }
            }
            arrangeYPosition -= buttonSize + buttonBuffer;
        }

        //-----------------------------------------

        //Change the height of the 'content' rect transform to match its height with the number of objects, when it goes out of bounds this tells the scrollbar it can start working. 
        RectTransform contentRect = scrollTiles.transform.Find("Viewport").transform.Find("Content").gameObject.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, -1 * arrangeYPosition);


    }



    //Toolkit Movement-------------------------------
    public void StartMovingToolkit()
    {
        bToolkitMoving = true;

        RectTransform canvasRect = transform.parent.gameObject.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out localPoint);

        toolkitMoveOffset = new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y) - localPoint;
    }

    public void StopMovingToolkit()
    {
        bToolkitMoving = false;
    }

    public void MouseMoveToolkit()
    {
        RectTransform canvasRect = transform.parent.gameObject.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out localPoint);

        localPoint += toolkitMoveOffset;

        Vector2 finalMovePoint = gameObject.transform.localPosition;
        Vector2 safetyBuffer = new Vector2(gameObject.GetComponent<RectTransform>().rect.width * bufferPercentage, gameObject.GetComponent<RectTransform>().rect.height * bufferPercentage);
        //% size of the toolkit as buffer

        //localPoint.x += GetComponent<RectTransform>().rect.width/2;
        //localPoint.y -= GetComponent<RectTransform>().rect.height/2;

        //this.gameObject.GetComponent<RectTransform>().anchoredPosition = localPoint; //This changes the position of the 'anchor', the object then positions itself according to its anchor settings.

        if (localPoint.x < (canvasRect.rect.width/2 - safetyBuffer.x) && localPoint.x > -canvasRect.rect.width / 2)//This section prevents the toolkit from being moved off-screen
        {
            finalMovePoint.x = localPoint.x;
        }
        if(localPoint.y < canvasRect.rect.height/2 && localPoint.y > (-canvasRect.rect.height / 2 + safetyBuffer.y))
        {
            finalMovePoint.y = localPoint.y; 
        }

        gameObject.transform.localPosition = finalMovePoint;
    }



}
