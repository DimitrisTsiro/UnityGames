using UnityEngine;
using System.Collections;
using OptitrackManagement;
using System;

public class LaserAppController : MonoBehaviour {

    private OptitrackManager _manager;
    private Laser _laserManager;
	public bool _deinitValue = false;

    ~LaserAppController()
    {
        Debug.Log("OptitrackManager: Destruct");
        OptitrackManagement.DirectMulticastSocketClient.Close();
    }

	// Use this for initialization
	void Start () 
	{
        OptitrackManagement.DirectMulticastSocketClient.Start();

        _manager = GetComponent<OptitrackManager>();
        _laserManager = GetComponentInChildren<Laser>();
	}

    // Update is called once per frame
    void Update()
    {
        if (OptitrackManagement.DirectMulticastSocketClient.IsInit() && OptitrackManagement.DirectMulticastSocketClient.IsDataReceivedCorrectly())
        {
            StreemData networkData = OptitrackManagement.DirectMulticastSocketClient.GetStreemData();

            if ( _manager.checkWristOrientation(networkData))
            {
                if (_manager.checkPointGesture(networkData))
                {
                    _manager.LaserPointing = true;

                    if (_manager.checkSelectGesture(networkData))
                    {
                        if (_laserManager.SelectedObject == null && _manager.DropGesture == false)
                            _manager.SelectGesture = true;
                        else if (_manager.SelectGesture == false && _laserManager.SelectedObject != null)
                            _manager.DropGesture = true;
                    }
                    else
                    {
                        _manager.SelectGesture = false;
                        _manager.DropGesture = false;
                    }
                }
            }
        }

        if (_deinitValue)
        {
            _deinitValue = false;
            OptitrackManagement.DirectMulticastSocketClient.Close();
        }
    }
}
