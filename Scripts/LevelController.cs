using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    private Editor editorReference;
    private GameController gameControllerReference;
    private MainUIController mainUIControllerReference;
    private CameraController cameraControllerReference;
    private EnemyController enemyControllerReference;

    public bool bTestingLevel;

    public string currentLevelName;
    //Level data
    public Level currentLevelData;
    public GameObject currentLevelObject;
    public int iNumberPressurePlates;
    public int iNumberRemoteExplosives;

    public void Initialize()
    {
        editorReference = GameObject.Find("EditorController").GetComponent<Editor>();
        gameControllerReference = GameObject.Find("GameController").GetComponent<GameController>();
        mainUIControllerReference = GameObject.Find("MainUI").GetComponent<MainUIController>();
        cameraControllerReference = GameObject.Find("Main Camera").GetComponent<CameraController>();
        enemyControllerReference = GameObject.Find("EnemyAIController").GetComponent<EnemyController>();

        bTestingLevel = false;
    }
    void Update()
    {
        
    }

    public void StartTestLevel(string levelName)
    {
        bTestingLevel = true; 
        StartLevel(levelName);
    }

    public void StartLevel(string levelName)
    {
        currentLevelData = LoadUtils.LoadLevel(levelName);
        currentLevelObject = editorReference.LoadLevel(currentLevelData);
        currentLevelName = levelName;

        iNumberPressurePlates = currentLevelData.iPressurePlates;
        iNumberRemoteExplosives = currentLevelData.iRemoteExplosives;

        gameControllerReference.currentState = GameController.GameState.GameIntro;
        //TODO: Play level opening


        gameControllerReference.currentState = GameController.GameState.GamePlanning;
        mainUIControllerReference.UpdateUIValues();
        mainUIControllerReference.SwitchUI(gameControllerReference.currentState);
        mainUIControllerReference.EnableUI();
        

        cameraControllerReference.bPlayerMovement = true;
    }

    public void PlayLevel()
    {
        gameControllerReference.currentState = GameController.GameState.GamePlaying;
        mainUIControllerReference.SwitchUI(gameControllerReference.currentState);
        enemyControllerReference.ActivateAI(true);
    }

    public void StopLevel()
    {
        gameControllerReference.currentState = GameController.GameState.GamePlanning;
        mainUIControllerReference.SwitchUI(gameControllerReference.currentState);
        enemyControllerReference.ActivateAI(false);
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }
        currentLevelObject = editorReference.LoadLevel(currentLevelData);
        iNumberPressurePlates = currentLevelData.iPressurePlates;
        iNumberRemoteExplosives = currentLevelData.iRemoteExplosives;
        mainUIControllerReference.UpdateUIValues();
    }

    public void ExitLevel()
    {
        enemyControllerReference.ActivateAI(false);
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }
        if (bTestingLevel)
        {
            gameControllerReference.currentState = GameController.GameState.Editor;
            mainUIControllerReference.SwitchUI(gameControllerReference.currentState);
            editorReference.StopTestingLevel();
        }
        mainUIControllerReference.ClearPlayerUI();
        mainUIControllerReference.DisableUI();
    }
}
