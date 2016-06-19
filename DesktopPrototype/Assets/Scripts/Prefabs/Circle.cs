using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems; 

public class Circle : MonoBehaviour,IPointerClickHandler  {

	public int number;
	public bool isTurn = false;

	private Board _board;

	// Use this for initialization
	void Start () {
		_board = GetComponentInParent<Board>();
		_board.NextCircleToHit += NextCircleToHit;
	}

	public void OnPointerClick(PointerEventData eventData) 
	{
		print("I was clicked");
		if (isTurn) 
		{
			this.GetComponent<Image> ().color = Color.red;
			isTurn = false;
			_board.CircleNumber = number;
		}
	}

	private void NextCircleToHit(int circleNumber)
	{
		if (number == circleNumber)
		{
			this.GetComponent<Image> ().color = Color.yellow;
			isTurn = true;
		}
	}
}
