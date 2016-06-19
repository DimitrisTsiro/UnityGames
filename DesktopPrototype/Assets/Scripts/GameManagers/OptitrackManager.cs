using UnityEngine;
using System;
using System.Collections;
using OptitrackManagement;

public class OptitrackManager : MonoBehaviour
{
	// We can normalize camera z position with this
	public float handScreenPositionMultiplier = 5f;

    public Transform origin;
    public GameObject wrist;

    public GameObject poin1;
    public GameObject point2;
    public GameObject laserpoint;
    public GameObject thumb; 

    private Vector3 _moveVector;
    private int laser;
    private int middle;
    private int theThumb;
    private int firstPoint;

    private Vector3 firstPointVec;
    private Vector3 middlePointVec;
    private Vector3 laserPointVec;
    private Vector3 thumbPointVec;
	private Vector3 cursorPointVec;
    private Transform theWrist;

    private bool isLaserPointing;
    private bool isSelectGesture = false;
    private bool isClickGesture;

    public event Action<Vector3, Vector3> OnPointGesture;
    public event Action<Vector3, Vector3> OnPointGestureStop;
    public event Action<String,Vector3> OnClickGesture;

    public Transform TheWrist
    {
    get { return theWrist; }
        set { theWrist = value;}
    }

    public bool LaserPointing
    {
        get { return isLaserPointing; }
        set
        {
            isLaserPointing = value;
            if (value == true)
                OnPointGesture(middlePointVec, laserPointVec);
           // else
               // OnPointGestureStop(middlePointVec, laserPointVec);
        }
    }

    public bool SelectGesture
    {
        get { return isSelectGesture; }
        set {isSelectGesture = value;}
    }

    public bool ClickGesture
    {
		get { return isClickGesture; }
		set {isClickGesture = value;}
    }

    public Vector3 FirstPoint
    {
        get { return firstPointVec; }
        set { firstPointVec = value;}
    }

    public Vector3 MiddlePoint
    {
        get { return middlePointVec; }
        set { middlePointVec = value; }
    }

    public Vector3 LaserPoint
    {
        get { return laserPointVec; }
        set { laserPointVec = value; }
    }

    public Vector3 ThumbPoint
    {
        get { return thumbPointVec; }
        set { thumbPointVec = value; }
    }

	public Vector3 CursorPoint
	{
		get { return cursorPointVec; }
		set { cursorPointVec = value; }
	}

   	// Converts hand position to screen coordinates
	public Vector3 GetHandScreenPosition()
	{
		return Camera.main.WorldToScreenPoint(new Vector3(cursorPointVec.x, cursorPointVec.y, cursorPointVec.z - handScreenPositionMultiplier));
	}

    public bool checkWristOrientation(StreemData networkData)
    {
        _moveVector = transform.position;     
        _moveVector = networkData._rigidBody[0].pos + origin.position;

        // Rigid body rotation streamed from Motive Tracker 
        Quaternion rot = networkData._rigidBody[0].ori * origin.rotation;

        // Invert pitch and yaw
        Vector3 euler = rot.eulerAngles;
        rot.eulerAngles = new Vector3(-euler.x, euler.y, -euler.z);

        // Apply rotation
        wrist.transform.rotation = rot;
        wrist.transform.position = _moveVector;

        TheWrist = wrist.transform;

        if ((wrist.transform.rotation.eulerAngles.x >= 0.0f && wrist.transform.rotation.eulerAngles.x <= 25.0f)
            || (wrist.transform.rotation.eulerAngles.x >= 315.0f && wrist.transform.rotation.eulerAngles.x <= 360.0f))
        {
            return true;
        }
        else
        {
            //LaserPointing = false;
            return false;
        }
    }

    public bool checkPointGesture(StreemData networkData)
    {
        //Sort the array to take the 4 closest to the wrist markers
        Array.Sort(networkData._markers, delegate(Marker marker1, Marker marker2)
        {
            float distance1 = Vector3.Distance(wrist.transform.position, marker1.pos);
            float distance2 = Vector3.Distance(wrist.transform.position, marker2.pos);

            return distance1.CompareTo(distance2);
        });

        firstPoint = findFirstPoint(networkData);
        middle = findMiddlePoint(networkData, firstPoint);
        theThumb = findThumb(networkData, firstPoint, middle);           
        laser = findLaserPoint(networkData, firstPoint, theThumb, middle);

        if (checkFingerAngle(networkData, firstPoint, middle, laser))
        {
           // LaserPointing = true;
            return true;
        }
        else
        {
            //LaserPointing = false;
            return false;
        }
    }

    private int findFirstPoint(StreemData networkData)
    {
        float minDistance = Vector3.Distance(wrist.transform.position, networkData._markers[0].pos);
        int result = 0;

        for (int i = 0; i < 4; i++)
        {
            float tempDist = Vector3.Distance(wrist.transform.position, networkData._markers[i].pos);

            if (tempDist < minDistance)
            {
                minDistance = tempDist;
                result = i;
            }
        }

        poin1.transform.position = networkData._markers[result].pos;
        firstPointVec = networkData._markers[result].pos;

        return result;
    }

    private int findThumb(StreemData networkData, int firstPoint, int middle)
    {
        int result = 0;
        float minDistance = 0.0f;
        bool found = false;

        for (int i = 0; i < 4; i++)
        {
             if (i != firstPoint && i !=middle)
            {
                float tempminDistance = Vector3.Distance(wrist.transform.position, networkData._markers[i].pos);
                 
                if (!found)
                {
                    minDistance = tempminDistance;
                    found = true;
                    result = i;
                }
                else if (tempminDistance < minDistance)
                {
                    minDistance = tempminDistance;
                    result = i;
                }
            }
        }

        thumb.transform.position = networkData._markers[result].pos;
        thumbPointVec = networkData._markers[result].pos;

        return result;
    }

    private int findMiddlePoint(StreemData networkData,int firstPoint)
    {
        float minDistance = 0;
        int result = 0;
        bool found = false;

        for (int i = 0; i < 4; i++)
        {
            if (i != firstPoint)
            {
                float tempMinDist = Vector3.Distance(networkData._markers[firstPoint].pos, networkData._markers[i].pos);

                if (!found)
                {
                    minDistance = tempMinDist;
                    found = true;
                    result = i;
                }
                else if (tempMinDist < minDistance)
                {
                    minDistance = tempMinDist;
                    result = i;
                }
            }
        }

        point2.transform.position = networkData._markers[result].pos;
        middlePointVec = networkData._markers[result].pos;

        return result;
    }

    private int findLaserPoint(StreemData networkData, int firstPoint, int theThumb, int middle)
    {
        int result = 0;
        for (int i = 0; i < 4; i++)
        {
            if (i != firstPoint && i != theThumb && i !=middle )
            {
                result = i;
            }
        }

        laserpoint.transform.position = networkData._markers[result].pos;
        laserPointVec = networkData._markers[result].pos;

        return result;
    }

    private bool checkFingerAngle(StreemData networkData, int firstPoint,int middle, int laser)
    {
        Vector3 v1 = networkData._markers[firstPoint].pos;
        Vector3 v2 = networkData._markers[middle].pos;
        Vector3 v3 = networkData._markers[laser].pos;

        float angle = signedAngleBetween(v1, v3, v2);

        if (angle >= 2.30f && angle <=2.95f)
            return true;
       else 
           return false;
   }

   private float signedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        Vector3 v1 = new Vector3(a.x-n.x,a.y-n.y,a.z-n.z);
        Vector3 v2 = new Vector3(b.x - n.x, b.y - n.y, b.z - n.z);

        float v1mag = (float)Math.Sqrt(v1.x * v1.x + v1.y * v1.y + v1.z * v1.z);
        Vector3 v1norm = new Vector3(v1.x/v1mag,v1.y/v1mag,v1.z/v1mag);

        float v2mag = (float)Math.Sqrt(v2.x * v2.x + v2.y * v2.y + v2.z * v2.z);
        Vector3 v2norm = new Vector3(v2.x / v2mag, v2.y / v2mag, v2.z / v2mag);

        double res = v1norm.x * v2norm.x + v1norm.y * v2norm.y + v1norm.z * v2norm.z;
        float angle = (float) Math.Acos(res);

        return angle;
    }

   public bool checkSelectGesture(StreemData networkData)
    {
        Vector3 v1 = networkData._markers[middle].pos;
        Vector3 v2 = networkData._markers[firstPoint].pos;
        Vector3 v3 = networkData._markers[theThumb].pos;

        float angle = signedAngleBetween(v1, v3, v2);
       // Debug.Log(angle);
        if (angle >= 1.05f)
            return true;
        else
            return false;
    }

	public bool checkForClick (StreemData networkData)
	{
		return false;
	}
}