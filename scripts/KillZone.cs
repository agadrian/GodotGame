using Godot;
using System;



public partial class KillZone : Area2D
{
	
	private Timer timer;
	
	public void _on_body_entered (Node body)
	{
		GD.Print("You died!");
		timer = GetNode<Timer>("Timer");
		timer.Start();
	}

	public void _on_timer_timeout()
	{
		GetTree().ReloadCurrentScene();
	}
}
