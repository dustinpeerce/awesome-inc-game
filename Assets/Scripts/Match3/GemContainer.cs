using UnityEngine;
using System.Collections;

public class GemContainer : MonoBehaviour {

    public static GemContainer instance;

    void Awake() {
        if (instance == null)
            instance = this;
    }

    public void RemoveChildren() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
