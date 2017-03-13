using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomHelper
{

	readonly float PINCHTOZOOMMULTIPLIER = 0.001f;
	RectTransform zoomableObject;

	private Vector2 maxWidthHeight;
	private Vector2 minWidthHeight;

	private Camera mainCamera;

	public bool isCenterPivotInitialized;

	Vector3[] corners = new Vector3[4];

	public ZoomHelper (RectTransform zoomableObject, Vector2 minWidthHeight, Vector2 maxWidthHeight, Camera mainCamera, float multiplier = 0.001f)
	{
		this.minWidthHeight = minWidthHeight;
		this.maxWidthHeight = maxWidthHeight;
		this.mainCamera = mainCamera;
		this.zoomableObject = zoomableObject;
		PINCHTOZOOMMULTIPLIER = multiplier;
	}


	/***
	 * returns the center of the pinch action, transformed into local coordinates 
	 ***/
	public Vector2 GetLocalTouchCenter ()
	{
		Touch touchZero = Input.GetTouch (0);
		Touch touchOne = Input.GetTouch (1);

		Vector2 multiTouchCenterPos = Vector2.Lerp (touchZero.position, touchOne.position, 0.5f);		

		//conveing it to world space
		Vector2 result;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (zoomableObject,
			multiTouchCenterPos,
			mainCamera,
			out result);
		multiTouchCenterPos = result;
		return multiTouchCenterPos;
	}


	public float GetZoomAmount ()
	{
		// Store both touches.
		Touch touchZero = Input.GetTouch (0);
		Touch touchOne = Input.GetTouch (1);

		// Find the position in the previous frame of each touch.
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
		
		// Find the magnitude of the vector (the distance) between the touches in each frame.
		float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
		
		// Find the difference in the distances between each frame.
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
		deltaMagnitudeDiff = -deltaMagnitudeDiff;
		return deltaMagnitudeDiff;
	}

	public void ZoomUnzoomMove ()
	{
		if (!isCenterPivotInitialized) {
			//InitializeCenterPivot ();
			isCenterPivotInitialized = true;
		}

		Vector2 touchcenter = GetLocalTouchCenter ();
		float zoomAmount = GetZoomAmount ();
		ZoomAndMove (touchcenter, zoomAmount);

	}


	public void ZoomAndMove (Vector2 multiTouchCenterPos, float deltaMagnitudeDiff)
	{
		
		deltaMagnitudeDiff *= PINCHTOZOOMMULTIPLIER;
		
		Vector3 scaleAmountVector = new Vector3 (deltaMagnitudeDiff, deltaMagnitudeDiff, 0f);

		if (((zoomableObject.localScale.x + scaleAmountVector.x) * zoomableObject.rect.width < maxWidthHeight.x && (zoomableObject.localScale.y + scaleAmountVector.y) * zoomableObject.rect.height < maxWidthHeight.y && deltaMagnitudeDiff > 0f) ||
		    (deltaMagnitudeDiff < 0f && (zoomableObject.localScale.x + scaleAmountVector.x) * zoomableObject.rect.width > minWidthHeight.x && (zoomableObject.localScale.y + scaleAmountVector.y) * zoomableObject.rect.height > minWidthHeight.y)) {
			zoomableObject.localScale += scaleAmountVector;
			return;
		} 

		if (deltaMagnitudeDiff < 0) {
			var scale = Mathf.Max (minWidthHeight.x / zoomableObject.rect.width, minWidthHeight.y / zoomableObject.rect.height);
			zoomableObject.localScale = new Vector3 (scale, scale, 1);
			return;
		} 
		if (deltaMagnitudeDiff > 0) {
			var scale = Mathf.Min (maxWidthHeight.x / zoomableObject.rect.width, maxWidthHeight.y / zoomableObject.rect.height);
			zoomableObject.localScale = new Vector3 (scale, scale, 1);
			return;
		}
		
	}

	private void InitializeCenterPivot ()
	{
		Vector2 roughTouchCenter = Vector2.Lerp (Input.GetTouch (0).position, Input.GetTouch (1).position, 0.5f);
		Vector2 pivotCandidate = new Vector2 ();
		Vector2 oldPivot = zoomableObject.pivot;

		RectTransformUtility.ScreenPointToLocalPointInRectangle (zoomableObject, roughTouchCenter, mainCamera, out pivotCandidate);
		pivotCandidate = new Vector2 (pivotCandidate.x / zoomableObject.rect.width, pivotCandidate.y / zoomableObject.rect.height) + zoomableObject.pivot;
		Vector2 pivotDelta = oldPivot - pivotCandidate;
		Vector3 deltaPosition = new Vector3 (pivotDelta.x * zoomableObject.rect.width * zoomableObject.localScale.x, pivotDelta.y * zoomableObject.rect.height * zoomableObject.localScale.y);
		zoomableObject.pivot = pivotCandidate;
		zoomableObject.localPosition -= deltaPosition;
	}
}
