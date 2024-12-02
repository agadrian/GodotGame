using Godot;
using System;

public partial class MenuTab : PanelContainer
{
	private MainMenuManager mainMenu;

	public override void _Ready()
	{
		if (GetParent() is MainMenuManager)
		{
			mainMenu = GetParent() as MainMenuManager;
		}
	}

	public void OnMenuSwapButtonPressed(int swapIndex)
	{
		mainMenu.SwapMenu(swapIndex, GetIndex());
		Visible = false;
	}

	public void OnMenuReturnButtonPressed()
	{
		mainMenu.SwapMenuToPrevious();
		Visible = false;
	}

	public void LoadSceneRequest(PackedScene loadScene)
	{
		mainMenu.OnSwapScene(loadScene);
	}
	
}
