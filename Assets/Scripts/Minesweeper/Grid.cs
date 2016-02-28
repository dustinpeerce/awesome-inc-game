using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Grid : MonoBehaviour {

    // Instance of the Grid
    public static Grid instance;

    // Public Attributes
    public Transform tilePrefab;    // Prefab for the tiles
    public Text levelText;          // Text for current level
    public Text remainingText;      // Text for remaining flags
    public Element[,] elements;     // array to store tiles
    public AudioClip play_sfx;

    // Private Attributes
    private int width = 10;             // width of the grid
    private int height = 13;            // height of the grid
    private int mineCount = 15;         // number of mine tiles
    private int remainingFlags = 0;     // number of remaining flags
    private bool gameIsOver = false;
    private bool readyToPlay = false;
    private bool minesPlanted = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }


    void Start() {
        elements = new Element[width, height];

        if (GameManager.instance.Medals >= 5) {
            width = 15;
            mineCount = 35; 
            levelText.text = "Advanced";
        }

        this.RemainingFlags = mineCount;
    }


    public void PrepareNewGame() {
        minesPlanted = false;
        this.RemainingFlags = mineCount;

        foreach (Element e in elements) {
            if (e != null)
                Destroy(e.gameObject);
        }

        elements = new Element[width, height];

        readyToPlay = true;
    }


    public void Play() {
        if (readyToPlay) {
            AudioSource.PlayClipAtPoint(play_sfx, Camera.main.transform.position);

            PrepareNewGame();

            gameIsOver = false;
            BuildGrid();
        }
    }

    // Build the Grid
    public void BuildGrid() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                GameObject.Instantiate(tilePrefab, new Vector3(x, y, 0), transform.rotation);
            }
        }
    }

    // Plant all the mines
    public void PlantMines(Element clickedElement) {
        int counter = 0;

        while (counter < mineCount) {
            int minePosX = (int)Random.Range(0f, (float)width);
            int minePosY = (int)Random.Range(0f, (float)height);
            if (!elements[minePosX, minePosY].isMine && elements[minePosX, minePosY].gameObject != clickedElement.gameObject) {
                elements[minePosX, minePosY].isMine = true;
                counter++;
            }
        }

        minesPlanted = true;
    }

    // Uncover all the Mines
    public void uncoverMines() {
        foreach (Element e in elements) {
            if (e.isMine)
                e.loadSprite(0);
        }
    }

    // Find out if a mine is at the coordinates
    public bool mineAt(int x, int y) {
        // Coordinates in range? Then check for mine
        if (x >= 0 && y >= 0 && x < width && y < height)
            return elements[x, y].isMine;
        return false;
    }

    // Count adjacent mines for an element
    public int adjacentMines(int x, int y) {
        int count = 0;

        if (mineAt(x, y + 1)) ++count;      // top
        if (mineAt(x + 1, y + 1)) ++count;  // top-right
        if (mineAt(x + 1, y)) ++count;      // right
        if (mineAt(x + 1, y - 1)) ++count;  // bottom-right
        if (mineAt(x, y - 1)) ++count;      // bottom
        if (mineAt(x - 1, y - 1)) ++count;  // bottom-left
        if (mineAt(x - 1, y)) ++count;      // left
        if (mineAt(x - 1, y + 1)) ++count;  // top-left

        return count;
    }

    // Flood Fill the empty elements
    public void FFUncover(int x, int y, bool[,] visited) {
        // Coordinates in range?
        if (x >= 0 && y >= 0 && x < width && y < height) {
            // visited already?
            if (visited[x, y] || elements[x, y].isFlagged())
                return;

            // uncover element
            elements[x, y].loadSprite(adjacentMines(x, y));

            // close to a mine? then no more work needed here
            if (adjacentMines(x, y) > 0)
                return;

            // set visited flag
            visited[x, y] = true;

            // recursion
            FFUncover(x - 1, y, visited);   // left
            FFUncover(x + 1, y, visited);   // right
            FFUncover(x, y - 1, visited);   // down
            FFUncover(x, y + 1, visited);   // up
            FFUncover(x - 1, y - 1, visited);   // bot-left
            FFUncover(x - 1, y + 1, visited);   // top-left
            FFUncover(x + 1, y - 1, visited);   // bot-right
            FFUncover(x + 1, y + 1, visited);   // top-right
        }
    }

    // Check if the game is done
    public bool isFinished() {
        // Try to find a covered element that is not a mine
        foreach (Element e in elements)
            if (e.isCovered() && !e.isMine)
                return false;

        // Only mines remain; the game is won
        return true;
    }

    public void BlowUp() {
        gameIsOver = true;
        readyToPlay = false;
        StartCoroutine(DelayLosePanel());
    }

    public void Win() {
        gameIsOver = true;
        readyToPlay = false;

        if (GameManager.instance.Medals == 3) {
            GameManager.instance.Medals++;
            GameManager.instance.DisplayEndPanel();
        }
        else {
            GameManager.instance.AdvancedMedalFour = true;
            GameManager.instance.DisplayAdvancedPanel();
        }
    }

    IEnumerator DelayLosePanel() {
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.DisplayLosePanel();
    }

    public int Width {
        get { return width; }
    }

    public int Height {
        get { return height; }
    }

    public int MineCount {
        get { return mineCount; }
        set { mineCount = value; }
    }

    public int RemainingFlags {
        get { return remainingFlags; }
        set { 
            remainingFlags = value;
            remainingText.text = "Remaining: " + remainingFlags;
        }
    }

    public bool MinesPlanted {
        get { return minesPlanted; }
    }

    public bool GameIsOver {
        get { return gameIsOver; }
        set { gameIsOver = value; }
    }
}
