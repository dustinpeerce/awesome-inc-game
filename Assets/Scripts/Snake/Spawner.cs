using UnityEngine;
using System.Collections;

// Spawner script used to spawn food and bonuses

public class Spawner : MonoBehaviour {

	// Food prefab
	public GameObject foodPrefab;

	// Borders
	public Transform borderTop;
	public Transform borderBottom;
	public Transform borderLeft;
	public Transform borderRight;

    private float extraBorderSpace = 0;

	void Start () {
	    // Spawn food 
	    SpawnFood ();

        if (GameManager.instance.Medals >= 5) {
            extraBorderSpace = 0;
        }
        else {
            extraBorderSpace = 1;
        }

        Debug.Log(extraBorderSpace);
	}

	// Spawn food
	public void SpawnFood () {
	    // x position between left and right border
        int x = (int)Random.Range(borderLeft.position.x + extraBorderSpace, borderRight.position.x - extraBorderSpace);

	    // y position between top and bottom border
        int y = (int)Random.Range(borderBottom.position.y + extraBorderSpace, borderTop.position.y - extraBorderSpace);

        Debug.Log(x + ", " + y);

	    // Instantiate the food at (x, y)
	    Instantiate (foodPrefab, new Vector2 (x, y), Quaternion.identity);
	}
}
