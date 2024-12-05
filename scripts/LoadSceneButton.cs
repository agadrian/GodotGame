using Godot;
using System;

public partial class LoadSceneButton : Button
{
	
	[Export] String sceneToSwitchTo;

	public override void _Ready()
	{
		Pressed += OnSwitchSceneButtonPressed;
	}

	private void OnSwitchSceneButtonPressed()
	{
		if (GetParent().GetParent() is MenuTab menuTab)
		{
			menuTab.LoadSceneRequest(sceneToSwitchTo);
			
		}
	}
}
