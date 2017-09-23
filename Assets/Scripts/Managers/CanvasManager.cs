using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour {

    public static CanvasManager Instance;

    public class CoreValueGameCanvas
    {

        public GameObject panel;
        public Text gameTitleText;
        public Text instructionsText;
        public Image[] controlsImage;
        public Text[] controlsText;

    } private CoreValueGameCanvas coreValueGameCanvas = new CoreValueGameCanvas();

    

    private CoreValueGame[] coreValueGames;

    


    private void Awake() {
        if (Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }


    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        //ResetCanvas();
        
        
    }


    private void ResetCanvas() {
        //coreValueGameCanvas.panel.SetActive(false);
    }


    public void DrawGamePanel(CoreValueGame game)
    {
        coreValueGameCanvas.gameTitleText.text = game.gameTitle;
        coreValueGameCanvas.instructionsText.text = game.instructionsParagraph;
        
        // Set the Controls images
        if (game.controlsImage.Length > 0)
        {
            if (game.controlsImage.Length > 1)
            {
                // Set the left and right images (we have two sprites for controls)
                coreValueGameCanvas.controlsImage[1].sprite = game.controlsImage[0];
                coreValueGameCanvas.controlsImage[2].sprite = game.controlsImage[1];
            }
            else
            {
                // Set the fullwidth image (we have only one sprite for controls)
                coreValueGameCanvas.controlsImage[0].sprite = game.controlsImage[0];
            }
        }

        // Set the Controls text labels
        if (game.controlsText.Length > 0)
        {
            if (game.controlsText.Length > 1)
            {
                // Set the left and right text elements (we have two text labels for controls)
                coreValueGameCanvas.controlsText[1].text = game.controlsText[0];
                coreValueGameCanvas.controlsText[2].text = game.controlsText[1];
            }
            else
            {
                // Set the fullwidth text element (we have only one text label for controls)
                coreValueGameCanvas.controlsText[0].text = game.controlsText[0];
            }
        }

        coreValueGameCanvas.panel.SetActive(true);
    }
    
}
