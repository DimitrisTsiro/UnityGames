using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Abstract UI component class for hand cursor objects
/// </summary>
[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public abstract class AbstractOptiTrackUICursor : MonoBehaviour {

    [SerializeField]
	protected OptitrackManager _data;
    protected Image _image;

    public virtual void Start()
    {
        Setup();
    }

    protected void Setup()
    {
		_data = GetComponent<OptitrackManager>();
        // Make sure we dont block raycasts
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().interactable = false;
        // image component
        _image = GetComponent<Image>();
    }

    public virtual void Update()
    {
		if (_data == null || !_data.LaserPointing) return;
        ProcessData();
    }

    public abstract void ProcessData();
}
