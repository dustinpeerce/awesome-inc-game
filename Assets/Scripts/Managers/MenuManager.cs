using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
	

    public void StartGame() {
        StartCoroutine(DelayMenuAction(Application.loadedLevel + 1));
    }

    public void Exit() {
        StartCoroutine(DelayMenuAction(-1));
    }

	IEnumerator DelayMenuAction(int level) {
        AudioManager.Instance.PlayAudioClip(AudioManager.Instance.sfxMenuSelect);
		yield return new WaitForSeconds(0.2f);

        if (level == -1) {
            Application.Quit();
        }
        else {
            Application.LoadLevel(level);
        }
	}
}
