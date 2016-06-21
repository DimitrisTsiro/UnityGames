using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using OptitrackManagement;

[AddComponentMenu("Optitrack/Optitrack Input Module")]
[RequireComponent(typeof(EventSystem))]
public class OptiTrackInputModule : BaseInputModule
{
	public OptitrackManager _inputData;
	[SerializeField]
	private float _scrollTreshold = .5f;
	[SerializeField]
	private float _scrollSpeed = 3.5f;
	[SerializeField]
	private float _waitOverTime = 2f;
	
	PointerEventData _handPointerData;
	
	static OptiTrackInputModule _instance = null;
	public static OptiTrackInputModule instance
	{
		get
		{
			if (!_instance)
			{
				_instance = FindObjectOfType(typeof(OptiTrackInputModule)) as OptiTrackInputModule;
				_instance._inputData = FindObjectOfType(typeof(OptitrackManager)) as OptitrackManager;
				if (!_instance)
				{
					if (EventSystem.current){
						EventSystem.current.gameObject.AddComponent<OptiTrackInputModule>();
						Debug.LogWarning("Add Optitrack Input Module to your EventSystem!");
					}
					else
						Debug.LogWarning("Create your UI first");
				}
			}
			return _instance;
		}
	}

	// get a pointer event data for a screen position
	private PointerEventData GetLookPointerEventData(Vector3 componentPosition)
	{
		if (_handPointerData == null)
		{
			_handPointerData = new PointerEventData(eventSystem);
		}
		_handPointerData.Reset();
		_handPointerData.delta = Vector2.zero;
		_handPointerData.position = componentPosition;
		_handPointerData.scrollDelta = Vector2.zero;
		eventSystem.RaycastAll(_handPointerData, m_RaycastResultCache);
		_handPointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
		m_RaycastResultCache.Clear();
		return _handPointerData;
	}
	
	public override void Process()
	{
		ProcessHover();
		ProcessPress();
		ProcessDrag();
		ProcessWaitOver();
	}
	/// <summary>
	/// Processes waitint over componens, if hovererd buttons click type is waitover, process it!
	/// </summary>
	private void ProcessWaitOver()
	{
		/*
		for (int j = 0; j < _inputData.Length; j++)
		{
			if (!_inputData[j].IsHovering || _inputData[j].ClickGesture != KinectUIClickGesture.WaitOver) continue;
			_inputData[j].WaitOverAmount = (Time.time - _inputData[j].HoverTime) / _waitOverTime;
			if (Time.time >= _inputData[j].HoverTime + _waitOverTime)
			{
				PointerEventData lookData = GetLookPointerEventData(_inputData[j].GetHandScreenPosition());
				GameObject go = lookData.pointerCurrentRaycast.gameObject;
				ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.submitHandler);
				// reset time
				_inputData[j].HoverTime = Time.time;
			}
		}
		*/
	}
	
	private void ProcessDrag()
	{
		/*
		for (int i = 0; i < _inputData.Length; i++)
		{
			// if not pressing we can't drag
			if (!_inputData[i].IsPressing)continue;
			//Debug.Log("drag " + Mathf.Abs(_inputData[i].TempHandPosition.x - _inputData[i].HandPosition.x));
			// Check if we reach drag treshold for any axis, temporary position set when we press an object
			if (Mathf.Abs(_inputData[i].TempHandPosition.x - _inputData[i].HandPosition.x) > _scrollTreshold || Mathf.Abs(_inputData[i].TempHandPosition.y - _inputData[i].HandPosition.y) > _scrollTreshold)
			{
				_inputData[i].IsDraging = true;
			}
			else
			{
				_inputData[i].IsDraging = false;
			}
			//Debug.Log("drag " + _inputData[i].IsDraging + " press " + _inputData[i].IsPressing);
			// If dragging use unit's eventhandler to send an event to a scrollview like component
			if (_inputData[i].IsDraging)
			{
				PointerEventData lookData = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
				eventSystem.SetSelectedGameObject(null);
				//Debug.Log("drag");
				GameObject go = lookData.pointerCurrentRaycast.gameObject;
				PointerEventData pEvent = new PointerEventData(eventSystem);
				pEvent.dragging = true;
				pEvent.scrollDelta = (_inputData[i].TempHandPosition - _inputData[i].HandPosition) * _scrollSpeed;
				pEvent.useDragThreshold = true;
				ExecuteEvents.ExecuteHierarchy(go, pEvent, ExecuteEvents.scrollHandler);
			}
		}
		*/
	}
	/// <summary>
	///  Process pressing, event click trigered on button by closing and opening hand,sends submit event to gameobject
	/// </summary>
	private void ProcessPress()
	{
		if (_inputData.ClickGesture)
		{
			PointerEventData lookData = GetLookPointerEventData(_inputData.GetHandScreenPosition());
			eventSystem.SetSelectedGameObject(null);

			Debug.Log(lookData.pointerCurrentRaycast.gameObject);

			if (lookData.pointerCurrentRaycast.gameObject != null)
			{
				GameObject go = lookData.pointerCurrentRaycast.gameObject;
				ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.submitHandler);
				ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.pointerUpHandler);
				ExecuteEvents.ExecuteHierarchy(go, lookData, ExecuteEvents.pointerClickHandler);
			}		
			_inputData.ClickGesture = false;
		}
	}
	/// <summary>
	/// Process hovering over component, sends pointer enter exit event to gameobject
	/// </summary>
	private void ProcessHover()
	{
		/*
		for (int i = 0; i < _inputData.Length; i++)
		{
			PointerEventData pointer = GetLookPointerEventData(_inputData[i].GetHandScreenPosition());
			var obj = _handPointerData.pointerCurrentRaycast.gameObject;
			HandlePointerExitAndEnter(pointer, obj);
			// Hover update
			_inputData[i].IsHovering = obj != null ? true : false;
			//if (obj != null)
			_inputData[i].HoveringObject = obj;
		}
		*/
	}
}

