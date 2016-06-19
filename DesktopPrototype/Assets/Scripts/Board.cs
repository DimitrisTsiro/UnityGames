using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

	public int numPoints = 20;                        //number of points on radius to place prefabs
	public Vector3 centerPos = new Vector3(0,0,0);    //center of circle/elipsoid
	
	public GameObject pointPrefab;                    //generic prefab to place on each point
	public float radiusX,radiusY;                    //radii for each x,y axes, respectively
	
	public bool isCircular = true;                    //is the drawn shape a complete circle?
	public bool vertical = false;                    //is the drawb shape on the xy-plane?
	
	Vector3 pointPos;                                //position to place each prefab along the given circle/eliptoid
	//*is set during each iteration of the loop
	
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
			GameObject point = (GameObject)Instantiate (pointPrefab);

			point.transform.parent = this.transform;
			point.transform.localScale=new Vector3(0.2f,0.2f,0.2f);
			point.transform.localRotation = Quaternion.Euler(0,0,0);
			point.transform.localPosition = pointPos;
			point.GetComponent<RectTransform>().sizeDelta= new Vector2(100,100);
		}
		
		//keeps radius on both axes the same if circular
		if(isCircular){
			radiusY = radiusX;
		}
	}
}
