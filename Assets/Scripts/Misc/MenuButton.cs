﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButton : MonoBehaviour {

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
        AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxHighlight);
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
