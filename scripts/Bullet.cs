using Godot;
using System;

public partial class Bullet : Area2D
{
	public float Speedbullet = 400.0f; // Velocidad de la bala
	private Vector2 _velocity;

	public override void _Ready()
	{
		// Conectar la señal de salida de la pantalla
		var notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");	
		notifier.ScreenExited += OnScreenExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Mover la bala en la dirección configurada
		Position += _velocity * (float)delta;
	}

	// Configura la dirección inicial de la bala
	public void SetDirection(Vector2 direction)
	{
		_velocity = direction.Normalized() * Speedbullet;
	}

	private void OnScreenExited()
	{
		// Destruir la bala cuando salga de la pantalla visible
		QueueFree();
	}
}
