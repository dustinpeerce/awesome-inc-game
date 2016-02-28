using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public AudioClip menuSelect_sfx;
    public AudioClip escape_sfx;

    // Medal Counter
    public int medals = 0;

    // Advanced Medal Trackers
    private bool advancedMedalOne = false;
    private bool advancedMedalTwo = false;
    private bool advancedMedalThree = false;
    private bool advancedMedalFour = false;

    // Game Panels
    private GameObject gameBeginPanel;
    private GameObject gameEndPanel;
    private GameObject gameAdvancedPanel;
    private GameObject gameLosePanel;
    private GameObject coreValueOnePanel;
    private GameObject coreValueTwoPanel;
    private GameObject coreValueThreePanel;
    private GameObject coreValueFourPanel;

    // House Pieces
    private GameObject[] housePieces = new GameObject[4];

    // Player Control Script
    private Platformer2DUserControl playerControlScript;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void OnLevelWasLoaded(int level) {
        if (level == 1) {
            playerControlScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Platformer2DUserControl>();

            gameBeginPanel = GameObject.Find("GameBeginPanel");
            gameLosePanel = null;
            coreValueOnePanel = GameObject.Find("CoreValueOnePanel");
            coreValueTwoPanel = GameObject.Find("CoreValueTwoPanel");
            coreValueThreePanel = GameObject.Find("CoreValueThreePanel");
            coreValueFourPanel = GameObject.Find("CoreValueFourPanel");
            gameEndPanel = GameObject.Find("GameEndPanel");
            gameAdvancedPanel = GameObject.Find("GameAdvancedPanel"); ;

            housePieces[0] = GameObject.Find("houseOne");
            housePieces[1] = GameObject.Find("houseTwo");
            housePieces[2] = GameObject.Find("houseThree");
            housePieces[3] = GameObject.Find("houseFour");

            switch (medals) {
                case 0:
                    gameBeginPanel.SetActive(true);
                    coreValueOnePanel.SetActive(false);
                    coreValueTwoPanel.SetActive(false);
                    coreValueThreePanel.SetActive(false);
                    coreValueFourPanel.SetActive(false);
                    gameEndPanel.SetActive(false);
                    gameAdvancedPanel.SetActive(false);

                    housePieces[0].SetActive(false);
                    housePieces[1].SetActive(false);
                    housePieces[2].SetActive(false);
                    housePieces[3].SetActive(false);
                    break;
                case 1:
                    gameBeginPanel.SetActive(false);
                    coreValueOnePanel.SetActive(true);
                    coreValueTwoPanel.SetActive(false);
                    coreValueThreePanel.SetActive(false);
                    coreValueFourPanel.SetActive(false);
                    gameEndPanel.SetActive(false);
                    gameAdvancedPanel.SetActive(false);

                    housePieces[0].SetActive(true);
                    housePieces[1].SetActive(false);
                    housePieces[2].SetActive(false);
                    housePieces[3].SetActive(false);
                    break;
                case 2:
                    gameBeginPanel.SetActive(false);
                    coreValueOnePanel.SetActive(false);
                    coreValueTwoPanel.SetActive(true);
                    coreValueThreePanel.SetActive(false);
                    coreValueFourPanel.SetActive(false);
                    gameEndPanel.SetActive(false);
                    gameAdvancedPanel.SetActive(false);

                    housePieces[0].SetActive(true);
                    housePieces[1].SetActive(true);
                    housePieces[2].SetActive(false);
                    housePieces[3].SetActive(false);
                    break;
                case 3:
                    gameBeginPanel.SetActive(false);
                    coreValueOnePanel.SetActive(false);
                    coreValueTwoPanel.SetActive(false);
                    coreValueThreePanel.SetActive(true);
                    coreValueFourPanel.SetActive(false);
                    gameEndPanel.SetActive(false);
                    gameAdvancedPanel.SetActive(false);

                    housePieces[0].SetActive(true);
                    housePieces[1].SetActive(true);
                    housePieces[2].SetActive(true);
                    housePieces[3].SetActive(false);
                    break;
                case 4:
                    gameBeginPanel.SetActive(false);
                    coreValueOnePanel.SetActive(false);
                    coreValueTwoPanel.SetActive(false);
                    coreValueThreePanel.SetActive(false);
                    coreValueFourPanel.SetActive(true);
                    gameEndPanel.SetActive(false);
                    gameAdvancedPanel.SetActive(false);

                    housePieces[0].SetActive(true);
                    housePieces[1].SetActive(true);
                    housePieces[2].SetActive(true);
                    housePieces[3].SetActive(true);
                    break;
                default:
                    gameBeginPanel.SetActive(false);
                    coreValueOnePanel.SetActive(false);
                    coreValueTwoPanel.SetActive(false);
                    coreValueThreePanel.SetActive(false);
                    coreValueFourPanel.SetActive(false);
                    gameEndPanel.SetActive(false);
                    gameAdvancedPanel.SetActive(false);

                    housePieces[0].SetActive(true);
                    housePieces[1].SetActive(true);
                    housePieces[2].SetActive(true);
                    housePieces[3].SetActive(true);

                    if (advancedMedalOne && advancedMedalTwo && advancedMedalThree && advancedMedalFour) {
                        gameAdvancedPanel.SetActive(true);
                    }
                    else {
                        playerControlScript.enabled = true;
                    }

                    break;
            }
        }
        else if (level != 0) {
            gameBeginPanel = GameObject.Find("GameBeginPanel");
            gameEndPanel = GameObject.Find("GameEndPanel");
            gameAdvancedPanel = GameObject.Find("GameAdvancedPanel");
            gameLosePanel = GameObject.Find("GameLosePanel");

            coreValueOnePanel = null;
            coreValueTwoPanel = null;
            coreValueThreePanel = null;
            coreValueFourPanel = null;

            Array.Clear(housePieces, 0, housePieces.Length);

            gameBeginPanel.SetActive(true);
            gameEndPanel.SetActive(false);
            gameAdvancedPanel.SetActive(false);
            gameLosePanel.SetActive(false);
        }
    }

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
            if (Application.loadedLevel == 1) {
                medals = 0;
                StartCoroutine(DelayLoadAction(0, escape_sfx));
            }
            else if (Application.loadedLevel != 0) {
                StartCoroutine(DelayLoadAction(1, escape_sfx));
            }
		}
	}

    public void DisplayAdvancedPanel() {
        gameAdvancedPanel.SetActive(true);
    }

    public void DisplayEndPanel() {
        gameEndPanel.SetActive(true);
    }

    public void DisplayLosePanel() {
        gameLosePanel.SetActive(true);
    }

    public void CloseEndPanel() {
        medals = 0;
        StartCoroutine(DelayLoadAction(0, menuSelect_sfx));
    }

    public void CloseShopPanels() {
        AudioSource.PlayClipAtPoint(menuSelect_sfx, Camera.main.transform.position);

        gameBeginPanel.SetActive(false);
        gameAdvancedPanel.SetActive(false);
        gameEndPanel.SetActive(false);
        gameLosePanel.SetActive(false);
    }

    public void CloseHubPanels() {
        AudioSource.PlayClipAtPoint(menuSelect_sfx, Camera.main.transform.position);

        gameBeginPanel.SetActive(false);
        coreValueOnePanel.SetActive(false);
        coreValueTwoPanel.SetActive(false);
        coreValueThreePanel.SetActive(false);
        coreValueFourPanel.SetActive(false);
        gameEndPanel.SetActive(false);

        playerControlScript.enabled = true;
    }

    IEnumerator DelayLoadAction(int level, AudioClip soundEffect) {
        AudioSource.PlayClipAtPoint(soundEffect, Camera.main.transform.position);
        yield return new WaitForSeconds(0.2f);

        if (level == -1)
            Application.Quit();
        else
            Application.LoadLevel(level);
    }

    public int Medals {
        get { return medals; }
        set { medals = value; }
    }

    public bool AdvancedMedalOne {
        get { return advancedMedalOne; }
        set { advancedMedalOne = value; }
    }

    public bool AdvancedMedalTwo {
        get { return advancedMedalTwo; }
        set { advancedMedalTwo = value; }
    }

    public bool AdvancedMedalThree {
        get { return advancedMedalThree; }
        set { advancedMedalThree = value; }
    }

    public bool AdvancedMedalFour {
        get { return advancedMedalFour; }
        set { advancedMedalFour = value; }
    }
}
