using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {

	void Start () {
        this.transform.parent = GameObject.Find("GemContainer").transform;
	}

    void OnMouseEnter() {
        GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
