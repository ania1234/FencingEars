using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTextureForQuad : MonoBehaviour
{

	void Start ()
	{
		if (TextureManager.portraitTexture != null) {
			Debug.LogError ("Swapping texture");
			this.GetComponent<Renderer> ().material.mainTexture = TextureManager.portraitTexture;
		}
	}
}
