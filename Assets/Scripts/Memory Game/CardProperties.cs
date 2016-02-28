using UnityEngine;
using System.Collections;

public class CardProperties : MonoBehaviour {

    // Attributes
	int pair = 0;
    bool solved = false;
    bool selected = false;

    void OnMouseEnter() {
        foreach (Transform child in transform) {
           if (!selected && !solved)
                child.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
    }

    void OnMouseExit() {
        foreach (Transform child in transform) {
            if (!selected && !solved)
                child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // Accessors/Mutators
	public int Pair {
		get { return pair; }
		set { pair = value; }
	}
	public bool Solved {
		get { return solved; }
		set { solved = value; }
	}
	public bool Selected {
		get { return selected; }
		set { 
            selected = value;
            foreach (Transform child in transform)
                child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
	}
}

