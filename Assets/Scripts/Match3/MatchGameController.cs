using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Holoville.HOTween;


public class MatchGameController : MonoBehaviour {
    
    // Public Attributes
	public GameObject indicator;                // Indicator for selected gem
	public GameObject[,]  arrayOfShapes;        // Array that contains all the gems
	public GameObject [] listOfGems;            // List of gem prefabs
    public GameObject emptyGameobject;          // Empty object that goes in place of destroyed gems
	public GameObject particleEffectWhenMatch;  // Particle Effect for matching gems
    public Text levelText;                      // Text object for the current level
    public Text scoreText;                      // Text object for the player's score
    public Text timeText;                       // Text object for the remaining time
    public AudioClip matchSound;                // Sound Effect for matching gems
    public AudioClip play_sfx;
    public AudioClip select_sfx;
    public AudioClip lose_sfx;

    // Private Attributes
    private GameObject currentIndicator;        // Current indicator
    private GameObject firstObject;             // First gem selected
    private GameObject secondObject;            // Second gem selected
    private int scoreIncrement = 5;             // Amount of points to increment each time we find matching gems
    private int scoreTotal = 0;                 // Current Score 
    private int scoreGoal = 400;                // Required score to beat the level
    private int timeRemaining = 0;              // Amount of time left
    private int timeMax = 99;                  // Max amount of time
    private float timer = 0.0f;                 // Determines how fast the time decreases
    private bool canTransitDiagonally = false;  // Indicate if we can switch diagonally
    private bool readyToPlay = false;
    private bool gameIsRunning = false;         // Indicates whether a game session is running
    private int width = 7;                      // Number of grid cells horizontally
    private int height = 9;                     // Number of grid cells vertically


    void Start() {
        arrayOfShapes = new GameObject[width, height];

        if (GameManager.instance.Medals >= 5) {
            scoreGoal = 800;
            timeMax = 99;

            levelText.text = "Advanced";
            scoreText.text = "Score: " + scoreTotal + "/" + scoreGoal;
            timeText.text = "Time: " + timeRemaining;
        }
    }


    public void PrepareNewGame() {
        // Clear the Grid of gems
        GemContainer.instance.RemoveChildren();

        // Clear the array of shapes
        ((IList)arrayOfShapes).Clear();

        // Reset Progress Values
        scoreTotal = 0;
        timeRemaining = timeMax;

        scoreText.text = "Score: " + scoreTotal + "/" + scoreGoal;
        timeText.text = "Time: " + timeRemaining;

        readyToPlay = true;
    }


	public void Play () {
        if (readyToPlay) {
            AudioSource.PlayClipAtPoint(play_sfx, Camera.main.transform.position);

            PrepareNewGame();

            // Create the gems and fill the array of shapes
            for (int i = 0; i <= width - 1; i++) {
                for (int j = 0; j <= height - 1; j++) {
                    var gameObject = GameObject.Instantiate(listOfGems[Random.Range(0, listOfGems.Length)] as GameObject, new Vector3(i, j, 0), transform.rotation) as GameObject;
                    arrayOfShapes[i, j] = gameObject;
                }
            }

            // Begin the game
            gameIsRunning = true;
        }
	}


	void Update () {
        if (gameIsRunning) {

            timer += Time.deltaTime;
            if (timer >= 1.0f) {
                timeRemaining--;
                timer = 0.0f;
                timeText.text = "Time: " + timeRemaining;
                CheckTimeRemaining();
            }

            bool shouldTransit = false;

            // Check if the player clicked on the left mouse button and if there is no animation playing
            if (Input.GetButtonDown("Fire1") && HOTween.GetTweenInfos() == null) {

                Destroy(currentIndicator);

                // Get the clicked GameObject
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.transform != null) {

                    // Check if the user already selected a gem
                    if (firstObject == null)
                        firstObject = hit.transform.gameObject;
                    else {
                        secondObject = hit.transform.gameObject;
                        shouldTransit = true;
                    }

                    // Set the current indicator
                    AudioSource.PlayClipAtPoint(select_sfx, Camera.main.transform.position, 0.3f);
                    currentIndicator = GameObject.Instantiate(indicator, new Vector3(hit.transform.position.x,
                                                                                            hit.transform.position.y, -1),
                                                                                            transform.rotation) as GameObject;

                    // If two gems are selected, we will swap them
                    if (shouldTransit) {

                        // Get the distance between the 2 gems
                        var distance = firstObject.transform.position - secondObject.transform.position;

                        // Check if they are neighbors, otherwise we will NOT swap them 
                        if (Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.y) <= 1) {
                            // If we dont want the player to swap diagonally
                            if (!canTransitDiagonally) {
                                if (distance.x != 0 && distance.y != 0) {
                                    Destroy(currentIndicator);
                                    firstObject = null;
                                    secondObject = null;
                                    return;
                                }
                            }

                            // Animate the transition
                            DoSwapMotion(firstObject.transform, secondObject.transform);
                            // Swap the objects in the array of shapes
                            DoSwapTile(firstObject, secondObject, ref arrayOfShapes);
                        }
                        // Otherwise, the gems are NOT neighbors, so don't do anything
                        else {
                            firstObject = null;
                            secondObject = null;
                        }
                        Destroy(currentIndicator);
                    }
                }
            }

            // If no animation is playing
            if (HOTween.GetTweenInfos() == null) {

                // Search for matches
                var Matches = FindMatch(arrayOfShapes);

                // If we find any matches
                if (Matches.Count > 0) {

                    // Update the score
                    UpdateScore(Matches.Count * scoreIncrement);

                    // Play the matching sound
                    AudioSource.PlayClipAtPoint(matchSound, Camera.main.transform.position, 0.3f);

                    foreach (GameObject go in Matches) {
                        

                        // Create and destroy the particle effect for matching
                        var destroyingParticle = GameObject.Instantiate(particleEffectWhenMatch as GameObject, new Vector3(go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
                        Destroy(destroyingParticle, 1f);

                        // Replace the matching space with an empty object
                        arrayOfShapes[(int)go.transform.position.x, (int)go.transform.position.y] = GameObject.Instantiate(emptyGameobject, new Vector3((int)go.transform.position.x, (int)go.transform.position.y, -1), transform.rotation) as GameObject;

                        //Destroy the matching gems
                        Destroy(go, 0.1f);
                    }

                    firstObject = null;
                    secondObject = null;

                    // Move the gems down to replace the empty objects
                    DoEmptyDown(ref arrayOfShapes);
                }
                // If no matching gems are found, remake the gems at their places
                else if (firstObject != null && secondObject != null) {
                    // Animate the gems
                    DoSwapMotion(firstObject.transform, secondObject.transform);
                    // Swap the gems in the array of shapes
                    DoSwapTile(firstObject, secondObject, ref arrayOfShapes);

                    firstObject = null;
                    secondObject = null;
                }
            }
        }
	} // END OF: Update()
    

    // Check the timer
    void CheckTimeRemaining() {
        if (timeRemaining <= 0) {
            AudioSource.PlayClipAtPoint(lose_sfx, Camera.main.transform.position);
            gameIsRunning = false;
            readyToPlay = false;
            GameManager.instance.DisplayLosePanel();
        }
    }


    // Updates the Player's score 
    void UpdateScore(int points) {
        scoreTotal += points;
        scoreText.text = "Score: " + scoreTotal + "/" + scoreGoal;

        if (scoreTotal >= scoreGoal) {
            gameIsRunning = false;
            readyToPlay = false;

            if (GameManager.instance.Medals == 1) {
                GameManager.instance.Medals++;
                GameManager.instance.DisplayEndPanel();
            }
            else {
                GameManager.instance.AdvancedMedalTwo = true;
                GameManager.instance.DisplayAdvancedPanel();
            }
        }
    }

    // Find Match-3 Tile
    private ArrayList FindMatch(GameObject[,] cells) {
        
        // Create an arraylist to store the matching tiles
        ArrayList stack = new ArrayList();

		// Check the vertical gems
        for (var x = 0; x <= cells.GetUpperBound(0); x++) {
            for (var y = 0; y <= cells.GetUpperBound(1); y++) {
                var thiscell = cells[x, y];
				// If it's empty, continue
                if (thiscell.name == "Empty(Clone)") continue;
                int matchCount = 0;
                int y2 = cells.GetUpperBound(1);
                int y1;

				// Getting the number of gems of the same type
                for (y1 = y + 1; y1 <= y2; y1++) {
                    if (cells[x, y1].name == "Empty(Clone)" || thiscell.name != cells[x, y1].name) 
                        break;
                    matchCount++;
                }

				// If we found more than 2 gems close, we add them in the array of matching gems
                if (matchCount >= 2) {
                    y1 = Mathf.Min(cells.GetUpperBound(1) , y1 - 1);
                    for (var y3 = y; y3 <= y1; y3++) {
                        if (!stack.Contains(cells[x, y3])) {
                            stack.Add(cells[x, y3]);
                        }
                    }
                }
            }
        }
		// Check the horizontal gems
        for (var y = 0; y < cells.GetUpperBound(1) + 1; y++) {
            for (var x = 0; x < cells.GetUpperBound(0) + 1; x++) {
                var thiscell = cells[x, y];
                // If it's empty, continue
                if (thiscell.name == "Empty(Clone)") continue;
                int matchCount = 0;
                int x2 = cells.GetUpperBound(0);
                int x1;

                // Getting the number of gems of the same type
                for (x1 = x + 1; x1 <= x2; x1++) {
                    if (cells[x1, y].name == "Empty(Clone)" || thiscell.name != cells[x1, y].name) 
                        break;
                    matchCount++;
                }

                // If we found more than 2 gems close, we add them in the array of matching gems
                if (matchCount >= 2) {
                    x1 = Mathf.Min(cells.GetUpperBound(0), x1 - 1);
                    for (var x3 = x; x3 <= x1; x3++) {
                        if (!stack.Contains(cells[x3, y])) {
                            stack.Add(cells[x3, y]);
                        }
                    }
                }
            }
        }
        return stack;
    }


    // Swap Motion Animation, to animate the switching arrays
    void DoSwapMotion(Transform a, Transform b) {
        Vector3 posA = a.localPosition;
        Vector3 posB = b.localPosition;
        TweenParms parms = new TweenParms().Prop("localPosition", posB).Ease(EaseType.EaseOutQuart);
        HOTween.To(a, 0.25f, parms).WaitForCompletion();
        parms = new TweenParms().Prop("localPosition", posA).Ease(EaseType.EaseOutQuart);
        HOTween.To(b, 0.25f, parms).WaitForCompletion();
    }


	// Swap two gems; it swaps the position of two objects in the grid array
	void DoSwapTile(GameObject a, GameObject b, ref GameObject[,] cells) {
        GameObject cell = cells[(int)a.transform.position.x, (int)a.transform.position.y];
        cells[(int)a.transform.position.x, (int)a.transform.position.y] = cells[(int)b.transform.position.x, (int)b.transform.position.y];
        cells[(int)b.transform.position.x, (int)b.transform.position.y] = cell;
    }


    // Do Empty Tile Move Down
    private void DoEmptyDown(ref GameObject[,] cells) {
 
        // Replace the empty objects with the ones above
        for (int x= 0; x <= cells.GetUpperBound(0); x++) {
            for (int y = 0; y <= cells.GetUpperBound(1); y++) {

                var thisCell = cells[x, y];
                if (thisCell.name == "Empty(Clone)") {

                    for (int y2 = y; y2 <= cells.GetUpperBound(1); y2++) {
                        if (cells[x, y2].name != "Empty(Clone)") {
                            cells[x, y] = cells[x, y2];
                            cells[x, y2] = thisCell;
                            break;
                        }
                    }
                }
            }
        }

		//Instantiate new gems to replace the ones destroyed
        for (int x = 0; x <= cells.GetUpperBound(0); x++) {
            for (int y = 0; y <= cells.GetUpperBound(1); y++) {
                if (cells[x, y].name == "Empty(Clone)") {
                    Destroy(cells[x, y]);
                    cells[x, y] = GameObject.Instantiate(listOfGems[Random.Range(0, listOfGems.Length)] as GameObject, new Vector3(x, cells.GetUpperBound(1)+2, 0), transform.rotation) as GameObject;
                }
            }
        }

        for (int x = 0; x <= cells.GetUpperBound(0); x++) {
            for (int y = 0; y <= cells.GetUpperBound(1) ; y++) {
                TweenParms parms = new TweenParms().Prop("position", new Vector3(x, y, -1)).Ease(EaseType.EaseOutQuart);
                HOTween.To(cells[x, y].transform, .4f, parms);
            }
        }
    }

   
} // END OF CLASS
