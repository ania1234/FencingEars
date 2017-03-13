using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using System;
using System.IO;
using Reign;

public class ChooseImageButton : MonoBehaviour, IPointerClickHandler
{
	[Inject (Id = "ChooseImageText")]
	private Text chooseImageText;

	[Inject (Id = "MainTextureImage")]
	private Image mainTextureImage;

	[Inject (Id = "ImageCutRectTransform")]
	private RectTransform imageCutRectTransform;

	[Inject (Id = "UIBlocker")]
	private GameObject UIBlocker;

	//while we are loading file chooser, we should temporarily disable all the buttons on the scene
	private const float DOUBLECLICK_MAX_TIME_DIFF = 0.5f;
	private int clicksCount = 0;
	private float lastClickTime;

	#region IPointerClickHandler implementation

	void IPointerClickHandler.OnPointerClick (PointerEventData eventData)
	{
		clicksCount++;
		if (clicksCount == 1) {
			lastClickTime = Time.time;
			return;
		}

		if (clicksCount == 2 && Time.time - lastClickTime < DOUBLECLICK_MAX_TIME_DIFF) {
			ChooseMainImageButtonClicked ();
			return;
		}

		if (clicksCount >= 2) {
			clicksCount = 1;
			lastClickTime = Time.time;
			return;
		}
	}

	#endregion

	public void ChooseMainImageButtonClicked ()
	{
		UIBlocker.SetActive (true);
		chooseImageText.text = "Loading...";
		StreamManager.LoadFileDialog (FolderLocations.Pictures, 2048, 2048, new string[]{ ".png", ".jpg", ".jpeg" }, imageLoadedCallback);
	}

	private void imageLoadedCallback (Stream stream, bool succeeded)
	{
		UIBlocker.SetActive (false);
		chooseImageText.text = succeeded.ToString ();
		if (!succeeded) {
			if (stream != null)
				stream.Dispose ();
			return;
		}
		
		try {
			var data = new byte[stream.Length];
			stream.Read (data, 0, data.Length);
			var newImage = new Texture2D (4, 4);
			newImage.LoadImage (data);
			newImage.Apply ();
			mainTextureImage.sprite = Sprite.Create (newImage, new Rect (0, 0, newImage.width, newImage.height), new Vector2 (.5f, .5f));
			mainTextureImage.SetNativeSize ();
			if (mainTextureImage.rectTransform.rect.width < imageCutRectTransform.rect.width || mainTextureImage.rectTransform.rect.height < imageCutRectTransform.rect.height) {
				//if we want to scale to match width, we have to scale by...
				var widthScale = imageCutRectTransform.rect.width / mainTextureImage.rectTransform.rect.width;
				//if we want to scale to match height, we have to scale by...
				var heightScale = imageCutRectTransform.rect.height / mainTextureImage.rectTransform.rect.height;
				var result = Mathf.Max (widthScale, heightScale);
				mainTextureImage.rectTransform.sizeDelta = new Vector2 (mainTextureImage.rectTransform.rect.width * result + 1, mainTextureImage.rectTransform.rect.height * result + 1);
			}
		} catch (Exception e) {
			this.GetComponent<Text> ().text = e.Message;
		} finally {
			if (stream != null)
				stream.Dispose ();
		}
	}
	
}
