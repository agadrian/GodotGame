using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainMenuManager : Control
{
	List<int> goBackList = new();

	public void SwapMenu(int menuIndex, int returnIndex)
	{
		if (GetChild(menuIndex) is MenuTab menuTab)
		{
			menuTab.Visible = true;
		}

		if (returnIndex > 0) return;
		goBackList.Add(returnIndex);
	}
	
	public void SwapMenuToPrevious()
	{
		if (!goBackList.Any()) return;
		SwapMenu(goBackList[goBackList.Count - 1], -1);
		goBackList.RemoveAt(goBackList.Count - 1);
	}

	public void OnSwapScene(PackedScene loadScene)
	{
		GetTree().Root.AddChild(loadScene.Instantiate());
		QueueFree();
	}

	private void OnQuitGameBtnPressed()
	{
		GetTree().Quit();
	}
}
