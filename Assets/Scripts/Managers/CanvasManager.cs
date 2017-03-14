using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour {

    public static CanvasManager Instance;


    [System.Serializable]
    public class CanvasManagerGeneral{
        public GameObject container;

    } public CanvasManagerGeneral canvasManagerGeneral;


    [System.Serializable]
    public class CanvasManagerMainMenu {
        public GameObject container;

    } public CanvasManagerMainMenu canvasManagerMainMenu;


    [System.Serializable]
    public class CanvasManagerHub {
        public GameObject container;

    } public CanvasManagerHub canvasManagerHub;


    [System.Serializable]
    public class CanvasManagerMemory {
        public GameObject container;

    } public CanvasManagerMemory canvasManagerMemory;


    [System.Serializable]
    public class CanvasManagerMatch {
        public GameObject container;

    } public CanvasManagerMatch canvasManagerMatch;


    [System.Serializable]
    public class CanvasManagerSnake {
        public GameObject container;

    } public CanvasManagerSnake canvasManagerSnake;


    [System.Serializable]
    public class CanvasManagerMinesweeper {
        public GameObject container;

    } public CanvasManagerMinesweeper canvasManagerMinesweeper;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    /*
    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        ResetCanvas();
        
        switch (scene.buildIndex) {
            case 0:
                canvasManagerMainMenu.container.SetActive(true);
                break;
            case 1:
                canvasManagerGeneral.container.SetActive(true);
                canvasManagerHub.container.SetActive(true);
                break;
            case 2:
                canvasManagerGeneral.container.SetActive(true);
                canvasManagerMemory.container.SetActive(true);
                break;
            case 3:
                canvasManagerGeneral.container.SetActive(true);
                canvasManagerMatch.container.SetActive(true);
                break;
            case 4:
                canvasManagerGeneral.container.SetActive(true);
                canvasManagerSnake.container.SetActive(true);
                break;
            case 5:
                canvasManagerGeneral.container.SetActive(true);
                canvasManagerMinesweeper.container.SetActive(true);
                break;
            default:
                Debug.LogError("Canvas Manager: Unknown Scene");
                break;
        }
    }
    */

    private void ResetCanvas() {
        canvasManagerGeneral.container.SetActive(false);
        canvasManagerMainMenu.container.SetActive(false);
        canvasManagerHub.container.SetActive(false);
        canvasManagerMemory.container.SetActive(false);
        canvasManagerMatch.container.SetActive(false);
        canvasManagerSnake.container.SetActive(false);
        canvasManagerMinesweeper.container.SetActive(false);
    }

    
}
