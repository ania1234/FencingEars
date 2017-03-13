using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GetTextureOnAwake : MonoBehaviour
{

	[Inject]
	private TextureManager textureManager;

	public SpriteRenderer spriteRenderer;

	void Start ()
	{
		Debug.LogError (GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit);
		if (TextureManager.portraitTexture != null) {
			//We have to calculate new pixels per unit, so that the new sprite will be the same size as the previous one.
			float newPixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit * TextureManager.portraitTexture.width / spriteRenderer.sprite.rect.width;
			spriteRenderer.sprite = Sprite.Create (TextureManager.portraitTexture, new Rect (new Vector2 (0, 0), new Vector2 (TextureManager.portraitTexture.width, TextureManager.portraitTexture.height)), new Vector2 (0.5f, 0.5f), newPixelsPerUnit);
		}
	}
}
