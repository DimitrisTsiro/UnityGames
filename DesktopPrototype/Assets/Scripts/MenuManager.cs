using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
	public Menu CurrentMenu;
	private Animator _animator;

	public void Start()
	{
		ShowMenu (CurrentMenu);
	}

	public void ShowMenu(Menu menu)
	{
		if (CurrentMenu != null) 
		{
			_animator = CurrentMenu.GetComponent<Animator> ();
			CurrentMenu.IsOpen = false;
		}

		CurrentMenu = menu;
		CurrentMenu.IsOpen = true;
	}

}
