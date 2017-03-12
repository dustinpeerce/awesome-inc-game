using UnityEngine;
using System.Collections;

public class Element : MonoBehaviour {

    public bool isMine = false;     // Indicates whether this element is a mine

    // Different Sprites
    public Sprite[] emptySprites;
    public Sprite mineSprite;
    public Sprite flagSprite;

    private Sprite defaultSprite;

	void Start () {

        // Set the parent
        transform.parent = GameObject.Find("Grid").transform;

        // Set the default Sprite
        defaultSprite = GetComponent<SpriteRenderer>().sprite;

        // Grid Registration
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        Grid.instance.elements[x, y] = this;
	}

    // Load another Sprite
    public void loadSprite(int adjacentCount) {
        if (isMine)
            GetComponent<SpriteRenderer>().sprite = mineSprite;
        else
            GetComponent<SpriteRenderer>().sprite = emptySprites[adjacentCount];
    }

    // Is the element still covered?
    public bool isCovered() {
        return GetComponent<SpriteRenderer>().sprite.texture.name == "default" || isFlagged();
    }

    // Is the element flagged?
    public bool isFlagged() {
        return GetComponent<SpriteRenderer>().sprite.texture.name == "flag";
    }

    void OnMouseUpAsButton() {
        if (!Grid.instance.GameIsOver) {
            if (isCovered() && !isFlagged()) {
                GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

                if (!Grid.instance.MinesPlanted) {
                    Grid.instance.PlantMines(this);
                }

                // It's a mine
                if (isMine) {
                    AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxSelectMine);

                    // Uncover all the mines
                    Grid.instance.uncoverMines();

                    // Trigger the explosion
                    Grid.instance.BlowUp();
                }
                // It's not a mine
                else {
                    AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxSelectItem);

                    // Show adjacent mine count
                    int x = (int)transform.position.x;
                    int y = (int)transform.position.y;
                    loadSprite(Grid.instance.adjacentMines(x, y));

                    // Uncover area without mines
                    Grid.instance.FFUncover(x, y, new bool[Grid.instance.Width, Grid.instance.Height]);

                    // Determine if the game was won
                    if (Grid.instance.isFinished()) {
                        // Trigger the Victory
                        Grid.instance.Win();
                    }

                }
            }
        }
    }

    void OnMouseEnter() {
        if (!Grid.instance.GameIsOver) {
            if (isCovered())
                GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
    }

    void OnMouseOver() {
        if (!Grid.instance.GameIsOver) {
            if (Input.GetMouseButtonDown(1)) {
                if (isCovered() && !isFlagged()) {
                    if (Grid.instance.RemainingFlags > 0) {
                        AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxFlagTile);
                        GetComponent<SpriteRenderer>().sprite = flagSprite;
                        Grid.instance.RemainingFlags--;
                    }
                }
                else if (isFlagged()) {
                    AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxFlagTile);
                    GetComponent<SpriteRenderer>().sprite = defaultSprite;
                    Grid.instance.RemainingFlags++;
                }
            }
        }
    }

    void OnMouseExit() {
        if (!Grid.instance.GameIsOver) {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }
	
}
