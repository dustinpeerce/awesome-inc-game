using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SnakeGameController : MonoBehaviour {

    // Public Attributes
    public Sprite headRight;
    public Sprite headDown;
    public Sprite headLeft;
    public Sprite headUp;
    public GameObject tailPrefab;
    public int length = 0;
    public Text levelText;
    public Text lengthText;

    // Private Attributes
    private Vector2 dir = Vector2.right;
    private List<Transform> tail = new List<Transform>();
    private bool ate = false;
    private GameObject spawner;
    private Spawner spawnerScript;
    private bool facingLeftOrRight = true;
    private bool facingUpOrDown = false;
    private Vector3 startPos;
    private int lengthGoal = 8;
    private bool gameOver = true;
    private bool readyToPlay = false;
    private float moveDelay = 0.15f;


    void Start() {
        startPos = transform.position;

        spawner = GameObject.Find("Spawner");
        spawnerScript = spawner.GetComponent<Spawner>();

        if (GameManager.instance.Medals >= 5) {
            lengthGoal = 25;
            moveDelay = 0.1f;
            levelText.text = "Advanced";

            ResetLength();
        }
    }


    public void PrepareNewGame() {
        // Stop the snake from moving
        CancelInvoke();

        ate = false;

        // Clear the tail
        foreach (Transform t in tail) {
            Destroy(t.gameObject);
        }
        tail.Clear();

        // Reset the position
        transform.position = startPos;

        // Assign the default sprite for the head
        GetComponent<SpriteRenderer>().sprite = headRight;

        // Reset the bools 
        facingLeftOrRight = true;
        facingUpOrDown = false;

        // Reset direction
        dir = Vector2.right;

        // Reset the progression text
        ResetLength();

        readyToPlay = true;
    }


    public void Play() {
        if (readyToPlay) {
            AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxMenuSelect);
            PrepareNewGame();

            gameOver = false;

            // Move the Snake
            InvokeRepeating("Move", 0.5f, moveDelay);
        }
    }


    void Update() {
        if (!gameOver) {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                if (!facingLeftOrRight) {
                    dir = Vector2.right;
                    GetComponent<SpriteRenderer>().sprite = headRight;
                    facingLeftOrRight = true;
                    facingUpOrDown = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                if (!facingUpOrDown) {
                    dir = -Vector2.up;
                    GetComponent<SpriteRenderer>().sprite = headDown;
                    facingUpOrDown = true;
                    facingLeftOrRight = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                if (!facingLeftOrRight) {
                    dir = -Vector2.right;
                    GetComponent<SpriteRenderer>().sprite = headLeft;
                    facingLeftOrRight = true;
                    facingUpOrDown = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                if (!facingUpOrDown) {
                    dir = Vector2.up;
                    GetComponent<SpriteRenderer>().sprite = headUp;
                    facingUpOrDown = true;
                    facingLeftOrRight = false;
                }
            }
        }
    }
    
    void Move() {
        // Save current position 
        Vector2 v = transform.position;

        // Move head into new direction
        transform.Translate(dir);

        // Insert new element if the snake has eaten
        if (ate) {
            // Load Prefab
            GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

            // Keep track of it the tail list
            tail.Insert(0, g.transform);

            // Reset the ate bool
            ate = false;
        }
        // Is there a tail
        else if (tail.Count > 0) {
            // Move last Tail to where the Head was
            tail.Last().position = v;

            // Add to front of list and remove from the back
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }
    }

    void OnTriggerEnter2D(Collider2D coll) {

        // If the snake eats food
        if (coll.name.StartsWith("Food")) {   
            // Make the snake longer
            ate = true;   
            // Destroy food
            Destroy(coll.gameObject);
            // Add to score
            IncrementLength();
            // Spawn more food
            spawnerScript.SpawnFood();
            // Play eat sound
            AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxPoint);
        } 
        // Collided with Tail or a Border
        else {
            // Set gameOver to true
            gameOver = true;
            // Stop the snake from moving
            CancelInvoke();

            readyToPlay = false;

            // Play gameOverSound
            AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxCollision);

            GameManager.instance.DisplayLosePanel();
        }
    }

    void IncrementLength() {
        length++;
        lengthText.text = "Length: " + length + "/" + lengthGoal;

        if (length >= lengthGoal) {
            gameOver = true;
            readyToPlay = false;
            CancelInvoke();

            if (GameManager.instance.Medals == 2) {
                GameManager.instance.Medals++;
                GameManager.instance.DisplayEndPanel();
            }
            else {
                GameManager.instance.AdvancedMedalThree = true;
                GameManager.instance.DisplayAdvancedPanel();
            }
        }
    }

    void ResetLength() {
        length = 0;
        lengthText.text = "Length: " + length + "/" + lengthGoal;
    }
}
