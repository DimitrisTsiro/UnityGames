using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Board : MonoBehaviour {

	public int numPoints = 25;                        //number of points on radius to place prefabs
	public Vector3 centerPos = new Vector3(0,0,0);    //center of circle/elipsoid
	
	public Circle pointPrefab;                    //generic prefab to place on each point
	public float radiusX,radiusY;                    //radii for each x,y axes, respectively
	
	public bool isCircular = true;                    //is the drawn shape a complete circle?
	public bool vertical = false;                    //is the drawb shape on the xy-plane?

	public event Action<int> NextCircleToHit;
	private int circleToHit = 25;
	public int CircleNumber
	{
		get{return circleToHit;}
		set
		{
			circleToHit = findNextTarget(value);
			NextCircleToHit(circleToHit);
		}
	}

	private int findNextTarget (int number)
	{
		if (number == numPoints) 
			return 1;
		else
			return number + 1;
	}

	Vector3 pointPos;                                //position to place each prefab along the given circle/eliptoid
	//*is set during each iteration of the loop

	private int startover=-1;
	// Use this for initialization
	void Start () {
		for(int i = 0; i<numPoints;i++){
			//multiply 'i' by '1.0f' to ensure the result is a fraction
			float pointNum = (i*1.0f)/numPoints;
			
			//angle along the unit circle for placing points
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*radiusX;
			float y = Mathf.Cos (angle)*radiusY;
			
			//position for the point prefab
			if(vertical)
				pointPos = new Vector3(x, y)+centerPos;
			else if (!vertical){
				pointPos = new Vector3(x, y, 0)+centerPos;
			}
			
			//place the prefab at given position
			Circle point = (Circle)Instantiate (pointPrefab);

			point.transform.parent = this.transform;
			point.transform.localScale=new Vector3(0.2f,0.2f,0.2f);
			point.transform.localRotation = Quaternion.Euler(0,0,0);
			point.transform.localPosition = pointPos;
			point.GetComponent<RectTransform>().sizeDelta= new Vector2(100,100);

			if(i==0)
			{
				point.number = 25;
				point.isTurn = true;
				point.GetComponentInChildren<Text>().text = "25";
				point.GetComponentInChildren<Image>().color = Color.yellow;
			}
			else if(i==1)
			{
				point.number = 2;
				point.isTurn = false;
				point.GetComponentInChildren<Text>().text = "2";
				point.GetComponentInChildren<Image>().color = Color.red;
			}
			else if (i*2 < numPoints)
			{
				point.number = i * 2;
				point.isTurn = false;
				point.GetComponentInChildren<Text>().text = ""+i * 2+"";
				point.GetComponentInChildren<Image>().color = Color.red;
			}
			else
			{
				point.number = startover + 2;
				point.isTurn = false;
				startover= startover + 2;
				point.GetComponentInChildren<Text>().text = ""+startover+"";
				point.GetComponentInChildren<Image>().color = Color.red;
			}
		}
		
		//keeps radius on both axes the same if circular
		if(isCircular){
			radiusY = radiusX;
		}
	}
}
