using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Editor editorReference;
    private LevelController levelControllerReference;
    private MainUIController mainUIControllerReference;

    public GameState currentState;

    public enum GameState
    {
        Menu,
        Editor,
        GameIntro,
        GamePlanning,
        GamePlaying,
        GamePaused,
    }


    void Start()
    {
        editorReference = GameObject.Find("EditorController").GetComponent<Editor>();
        levelControllerReference = GameObject.Find("LevelController").GetComponent<LevelController>();
        mainUIControllerReference = GameObject.Find("MainUI").GetComponent<MainUIController>();

        levelControllerReference.Initialize();
        editorReference.Initialize();
        mainUIControllerReference.Initialize();

        StartEditor();
    }

    
    void Update()
    {
        
    }

    public void StartEditor()
    {
        currentState = GameState.Editor;
        editorReference.StartEditor();
    }

    public void TestLevel(string levelName)
    {
        editorReference.CloseEditor();
        levelControllerReference.StartTestLevel(levelName);
    }
}
