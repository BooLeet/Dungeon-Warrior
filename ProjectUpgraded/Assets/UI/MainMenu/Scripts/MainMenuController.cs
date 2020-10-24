using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public EventSystem eventSystem;
    public SettingsMenuController settingsController;
    public string loadingScreenSceneName = "LoadingScreen";

    [System.Serializable]
    public class MainMenuState
    {
        public GameObject mainObject;
        public GameObject objectToSelect;
        [HideInInspector]public GameObject previouslySelectedObject;

        public void HideObject()
        {
            mainObject.SetActive(false);
        }

        public void ShowObject()
        {
            mainObject.SetActive(true);
        }
    }
    private Stack<MainMenuState> stateStack = new Stack<MainMenuState>();
    public bool showMainOnStart = true;

    public MainMenuState main;
    public MainMenuState settings;
    public MainMenuState quit;
    [Space]
    public MainMenuState play;
    public MainMenuState gameOver;
    public GameOverText gameOverText;
    [Space]
    public MainMenuState settingsGraphics;
    public MainMenuState settingsAudio;
    public MainMenuState settingsGameplay;
    public MainMenuState settingsKeys;


    void Start()
    {
        if(showMainOnStart)
            ShowMain();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GoBack();
    }

    private void PushState(MainMenuState state)
    {
        if(stateStack.Count > 0)
        {
            stateStack.Peek().previouslySelectedObject = eventSystem.currentSelectedGameObject;
            stateStack.Peek().HideObject();
        }
            
        stateStack.Push(state);
        
        state.ShowObject();
        eventSystem.SetSelectedGameObject(state.objectToSelect);
    }

    private void PopState()
    {
        if (stateStack.Count == 0)
            return;

        stateStack.Pop().HideObject();
    }


    #region Public
    public void EnableMenu()
    {
        gameObject.SetActive(true);
    }

    public void DisableMenu()
    {
        gameObject.SetActive(false);
        while(stateStack.Count > 0)
            PopState();
    }

    public void Unpause()
    {
        GameManager.instance.ResumeGame();
        DisableMenu();
    }

    public void GoBack()
    {
        if (stateStack.Count <= 1)
            return;
        PopState();
        MainMenuState newState = stateStack.Peek();
        newState.ShowObject();
        eventSystem.SetSelectedGameObject(newState.previouslySelectedObject ? newState.previouslySelectedObject : newState.objectToSelect);
    }


    public void ShowMain()
    {
        PushState(main);
    }

    // Play
    public void LoadLevel(LevelInfo info)
    {
        GameManager.instance.UnpauseGame();
        SceneNameContainer.instance.sceneName = info.sceneName;
        SceneManager.LoadSceneAsync(loadingScreenSceneName);
    }


    // Settings
    public void ShowSettings()
    {
        PushState(settings);
    }

    public void ShowSettingsGraphics()
    {
        settingsController.LoadGraphicsValues();
        PushState(settingsGraphics);
    }

    public void ShowSettingsAudio()
    {
        settingsController.LoadAudioValues();
        PushState(settingsAudio);
    }

    public void ShowSettingsGameplay()
    {
        settingsController.LoadGameValues();
        PushState(settingsGameplay);
    }

    public void ShowSettingsKeys()
    {
        PushState(settingsKeys);
    }

    // Game Over
    public void ShowGameOver()
    {
        gameOverText.UpdateText();
        PushState(gameOver);
    }

    // Quit
    public void ShowQuit()
    {
        PushState(quit);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}
