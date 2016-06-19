using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
	private Animator _animator;

	/*

	private GameManager _manager;

	public bool IsGameEnd
	{
		get {return _animator.GetBool("IsGameEnd");}
		set{_animator.SetBool("IsGameEnd",value);}
	}
	*/

	public bool IsOpen
	{
		get {return _animator.GetBool("IsOpen");}
		set{
			_animator = GetComponent<Animator> ();
			_animator.SetBool("IsOpen",value);}
	}
	
	public void Awake()
	{
		_animator = GetComponent<Animator> ();
		//Position in the middle
		var rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0,0);
	}

	/*
	private void GameManagerOnGameEnded(GameEndStatus gameEndStatus)
	{
		StartCoroutine (WaitForEndGame()); 
	} 

	IEnumerator WaitForEndGame()
	{
		yield return new WaitForSeconds (3.0f);
		IsGameEnd = true;
	}

	public void Replay()
	{
		IsGameEnd = false;
	}

	public void BackToStore()
	{
		IsGameEnd = false;
	}

	*/

}
