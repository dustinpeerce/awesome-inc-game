using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shop : MonoBehaviour {

    public string sceneToLoad;
    public int medalsRequired = 0;
    public GameObject buttonPrompt;
    public AudioClip door_sfx;

    private bool isActive = false;
    private bool isBouncing = false;

	void Start () {
        buttonPrompt.SetActive(false);
        if (GameManager.instance.Medals == medalsRequired || (GameManager.instance.Medals >= 5 && sceneToLoad != "")) {
            isBouncing = true;
            GetComponent<Animator>().SetBool("Bouncing", true);
        }
	}
	
	void Update () {
        if (isActive) {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                StartCoroutine(DelayDoorAction());
            }
        }
	}

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Player") {
            if (isBouncing) {
                ChangeShopState();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Player") {
            if (isBouncing) {
                ChangeShopState();
            }
        }
    }

    void ChangeShopState() {
        isActive = !isActive;
        buttonPrompt.SetActive(!buttonPrompt.activeSelf);
    }

    IEnumerator DelayDoorAction() {
        AudioSource.PlayClipAtPoint(door_sfx, Camera.main.transform.position, 0.3f);
        yield return new WaitForSeconds(0.35f);

        if (sceneToLoad == "") {
            GameManager.instance.Medals++;
            GameManager.instance.DisplayEndPanel();
        }
        else {
            Application.LoadLevel(sceneToLoad);
        }
    }
}
