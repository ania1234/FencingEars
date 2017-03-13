using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class ZoomableImage : MonoBehaviour
{
	private const float MIN_ZOOM = 0.75f;
	private const float MAX_ZOOM = 2.0f;

	private bool isInPlace = true;

	private float dragThreshold;
	private Vector2 initialTouch1;
	private Vector2 initialTouch2;
	private Vector2 touch1;
	private Vector2 touch2;
	private Vector3[] corners = new Vector3[4];
	private ZoomHelper zoomHelper;


	[Inject (Id = "MainCamera")]
	private Camera mainCamera;

	[Inject (Id = "MainScrollRect")]
	private ScrollRect scrollRect;

	[Inject (Id = "ImageCutRectTransform")]
	private RectTransform imageCutRectTransform;



	public Image image;

	void Awake ()
	{
		dragThreshold = 0.0000004f * Screen.width;
		zoomHelper = new ZoomHelper (image.rectTransform, new Vector2 (imageCutRectTransform.rect.width, imageCutRectTransform.rect.height), new Vector2 (2048, 2048), mainCamera, 0.00004f * Screen.width);
	}

	void Update ()
	{		
		if (Input.touchCount == 2) {
			if (isInPlace) {
				//if (BothTouchesOnImage ()) {
				GoOutOfPlace ();
				//}
			} else {
				Vector2 touch1 = Input.GetTouch (0).position;
				Vector2 touch2 = Input.GetTouch (1).position;
				if (Vector2.Distance (touch1, initialTouch1) >= dragThreshold || Vector2.Distance (touch2, initialTouch2) >= dragThreshold) {
					zoomHelper.ZoomUnzoomMove ();
				}
			}
			return;
		} else {
			if (zoomHelper.isCenterPivotInitialized) {
				GoBackToPlace ();
			}
		}
	}

	private bool BothTouchesOnImage ()
	{
		Vector2 checkVector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (image.transform.parent.GetComponent<RectTransform> (), Input.GetTouch (0).position, mainCamera, out checkVector);
		checkVector = new Vector2 (checkVector.x / image.rectTransform.rect.width, checkVector.y / image.rectTransform.rect.height) + image.rectTransform.pivot;
		if (checkVector.x < 0 || checkVector.x > 1 || checkVector.y < 0 || checkVector.y > 1) {
			return false;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle (image.rectTransform, Input.GetTouch (1).position, mainCamera, out checkVector);
		checkVector = new Vector2 (checkVector.x / image.rectTransform.rect.width, checkVector.y / image.rectTransform.rect.height) + image.rectTransform.pivot;
		if (checkVector.x < 0 || checkVector.x > 1 || checkVector.y < 0 || checkVector.y > 1) {
			return false;
		}

		return true;
	}

	private void GoBackToPlace ()
	{
		isInPlace = true;
		scrollRect.enabled = true;
		zoomHelper.isCenterPivotInitialized = false;
	}

	private void GoOutOfPlace ()
	{
		isInPlace = false;
		scrollRect.enabled = false;
		if (Input.touchCount == 2) {
			initialTouch1 = Input.GetTouch (0).position;
			initialTouch2 = Input.GetTouch (1).position;
		}
	}
}