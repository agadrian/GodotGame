using Godot;
using System;

public partial class Spikes : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is Player player)
		{
			GD.Print("fssdg");
			player.TakeDamage(player.Health + 1);
		}

		else if (body is Enemy enemy)
		{
			GD.Print("");
			enemy.TakeDamage(enemy.Health + 1);
		}

		else
		{
			GD.Print("uknown");
		}
		
	}
}