using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButton : MonoBehaviour {

    public AudioClip highlightSFX;
    public Sprite highlightSprite;

    void Start() {
        if (this.gameObject.name == "ExitButton") {
            if (Application.platform == RuntimePlatform.OSXWebPlayer
                || Application.platform == RuntimePlatform.WindowsWebPlayer
                || Application.platform == RuntimePlatform.WebGLPlayer) {
                this.gameObject.SetActive(false);
            }
        }
    }

    public void MouseEnter() {
        AudioSource.PlayClipAtPoint(highlightSFX, Camera.main.transform.position);
        GetComponent<Button>().image.overrideSprite = highlightSprite;
    }

    public void MouseExit() {
        GetComponent<Button>().image.overrideSprite = null;
    }

    public void CloseShopPanels() {
        GameManager.instance.CloseShopPanels();
    }

    public void CloseHubPanels() {
        GameManager.instance.CloseHubPanels();
    }

    public void CloseEndPanel() {
        GameManager.instance.CloseEndPanel();
    }

    public void LoadHubScene() {
        Application.LoadLevel(1);
    }
}
