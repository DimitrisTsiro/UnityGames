using UnityEngine;
using System.Collections;
using OptitrackManagement;
using System;

public class LaserAppController : MonoBehaviour {

	public int framesOfSelectGesture=10;
    private OptitrackManager _manager;
	private int frameCounter=0;

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

		OptiTrackInputModule.instance._inputData = _manager;
	}

    // Update is called once per frame
    void Update()
    {
        if (OptitrackManagement.DirectMulticastSocketClient.IsInit () && OptitrackManagement.DirectMulticastSocketClient.IsDataReceivedCorrectly ()) 
		{
			StreemData networkData = OptitrackManagement.DirectMulticastSocketClient.GetStreemData ();

			if (_manager.checkWristOrientation (networkData)) 
			{
				if (_manager.checkPointGesture (networkData)) 
				{
					_manager.LaserPointing = true;

					if (_manager.SelectGesture) 
					{
						if(frameCounter<=framesOfSelectGesture)
						{
							if (_manager.checkForClick (networkData)) 
							{
								_manager.ClickGesture = true;
								_manager.SelectGesture = false;
								frameCounter = 0;
							} 
							else
								frameCounter++;
						}
						else
						{
							frameCounter = 0;
							_manager.SelectGesture =false;
						}
					}
					else if (_manager.checkSelectGesture (networkData)) 
					{
						_manager.SelectGesture = true;
						_manager.ClickGesture = false;
					}
					else
					{
						_manager.SelectGesture = false;
						_manager.ClickGesture = false;
					}
				}
				else
					noGestures ();
			}
			else
				noGestures ();
		} 
		else
			noGestures ();
    }

	private void noGestures()
	{
		_manager.LaserPointing = false;
		_manager.SelectGesture = false;
		_manager.ClickGesture = false;
		frameCounter = 0;
	}
}
