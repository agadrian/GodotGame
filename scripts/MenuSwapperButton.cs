using Godot;
using System;

public partial class MenuSwapperButton : Button
{
	[Export] private Node SwitchToMenu;

	public override void _Ready()
	{
		Pressed += OnMenuSwapperButtonPressed;
	}

	public void OnMenuSwapperButtonPressed()
	{
		if (GetParent().GetParent() is MenuTab menuTab)
		{
			menuTab.OnMenuSwapButtonPressed(SwitchToMenu.GetIndex());
		}
	}
	
}
