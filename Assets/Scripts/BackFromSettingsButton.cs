using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class BackFromSettingsButton : MonoBehaviour
{
	[Inject]
	private ZenjectSceneLoader sceneLoader;

	[Inject]
	private TextureManager textureManager;

	[Inject (Id = "ImageCutRectTransform")]
	private RectTransform imageCutRectTransform;

	[Inject (Id = "MainTextureImage")]
	private Image mainTextureImage;

	public void BackButtonClicked ()
	{
		PrepareNewTexture ();
		sceneLoader.LoadScene ("MainScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

	void PrepareNewTexture ()
	{
		Rect cutRect = imageCutRectTransform.rect;
		Rect imageRect = mainTextureImage.rectTransform.rect;

		Vector3[] cutRectCorners = new Vector3[4];
		imageCutRectTransform.GetWorldCorners (cutRectCorners);
		Rect newCutRect = new Rect (cutRectCorners [0], cutRectCorners [2] - cutRectCorners [0]);

		Vector3[] imageRectCorners = new Vector3[4];
		mainTextureImage.rectTransform.GetWorldCorners (imageRectCorners);
		Rect newImageRect = new Rect (imageRectCorners [0], imageRectCorners [2] - imageRectCorners [0]);

		//Debug.LogError ("New Image rect starts at x = " + (newImageRect.xMin * mainTextureImage.rectTransform.localScale.x).ToString () + " y = " + (newImageRect.yMin * mainTextureImage.rectTransform.localScale.y).ToString () + "and has width = " + (newImageRect.width).ToString () + " height " + (newImageRect.height).ToString ());
		//Debug.LogError ("Image rect starts at x = " + (imageRect.xMin * mainTextureImage.rectTransform.localScale.x).ToString () + " y = " + (imageRect.yMin * mainTextureImage.rectTransform.localScale.y).ToString () + "and has width = " + (imageRect.width * mainTextureImage.rectTransform.localScale.x).ToString () + " height " + (imageRect.height * mainTextureImage.rectTransform.localScale.y).ToString ());

		Vector2 percentFromLowerLeftCorner = new Vector2 (Mathf.Abs (newImageRect.xMin - newCutRect.xMin) / newImageRect.width, Mathf.Abs (newImageRect.yMin - newCutRect.yMin) / newImageRect.height);

		int startx = Mathf.FloorToInt (percentFromLowerLeftCorner.x * mainTextureImage.mainTexture.width);
		int starty = Mathf.FloorToInt (percentFromLowerLeftCorner.y * mainTextureImage.mainTexture.height);
		float percentageWidth = newCutRect.width / newImageRect.width;
		float percentageHeight = newCutRect.height / newImageRect.height;

		Texture2D finalTexture = new Texture2D (Mathf.FloorToInt (mainTextureImage.mainTexture.width * percentageWidth), Mathf.FloorToInt (mainTextureImage.mainTexture.height * percentageHeight));
		finalTexture.SetPixels (((Texture2D)mainTextureImage.mainTexture).GetPixels (startx, starty, Mathf.FloorToInt (mainTextureImage.mainTexture.width * percentageWidth), Mathf.FloorToInt (mainTextureImage.mainTexture.height * percentageHeight)));
		finalTexture.Apply ();
		TextureManager.portraitTexture = finalTexture;
	}
}
