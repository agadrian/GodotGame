using Godot;
using System;

public partial class Spikes : Area2D
{
	private void _on_body_entered(Node2D body)
	{
		if (body is Player player)
		{
			GD.Print("Jugador entra spikes");
			player.TakeDamage(player.Health + 1);
		}

		else if (body is Enemy enemy)
		{
			GD.Print("Enemigo entra spikes");
			enemy.TakeDamage(enemy.Health + 1);
		}
	}
}
