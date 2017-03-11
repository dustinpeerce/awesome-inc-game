using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSpriteToScreen : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private float width;
    private float height;
    private float worldScreenHeight;
    private float worldScreenWidth;
	
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        width = spriteRenderer.sprite.bounds.size.x;
        height = spriteRenderer.sprite.bounds.size.y;

        worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight/height, transform.localScale.z);
	}

}
