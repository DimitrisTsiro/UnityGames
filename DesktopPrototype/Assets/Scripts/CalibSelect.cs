using UnityEngine;
using System.Collections;
using OptitrackManagement;
using System;

//https://software.intel.com/en-us/articles/intel-inde-media-pack-for-android-tutorials-video-capturing-for-unity3d-applications
//https://www.youtube.com/watch?v=kcOKPnWxqwU
//https://developer.vuforia.com/forum/faq/android-how-can-i-capture-ar-view

public class CalibSelect : MonoBehaviour {

    private OptitrackManager manager;
    private Laser theLaser;
    private Vector3 moveVector;
    
    public float speed = 1.5f;
    public float dropSpeed = 2.0f;

    private bool moveToWrist = false;
    private bool moveToScreen = false;

	// Use this for initialization
	void Start () {
        this.manager = GameObject.Find("Hand").GetComponent<OptitrackManager>();
        this.theLaser = GameObject.Find("LaserPoint").GetComponent<Laser>();

        this.theLaser.OnObjectSelected += OnObjectSelected;
        this.manager.OnDropGesture += OnDropGesture;
	}
	
	// Update is called once per frame
	void Update () {
        if (moveToWrist == true)
        {
            this.renderer.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, this.manager.TheWrist.position, speed * Time.deltaTime);
        }
        else if (moveToScreen == true)// && (transform.position == this.manager.TheWrist.position))
            if (transform.position != moveVector)
                transform.position = moveVector;
            else
            {
                moveToScreen = false;
                this.renderer.enabled = true;
            }
	}

    private void OnObjectSelected(String theHit)
    {
        if (theHit == this.name)
           moveToWrist = true;
    }

    private void OnDropGesture(String theHit, Vector3 targetPosition)
    {
        if (theHit == this.name)
        {
            moveToWrist = false;
            moveToScreen= true;
            this.theLaser.SelectedObject = null;
            moveVector = targetPosition;
        }
    }
}
