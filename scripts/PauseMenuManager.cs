using Godot;
using System;

public partial class PauseMenuManager : Control
{
	public override void _Input(InputEvent @event)
	{
		// Detectar la tecla ESC
		if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Escape)
		{
			TogglePauseMenu();
		}
	}

	private void TogglePauseMenu()
	{
		// Alternar la visibilidad del menú y pausar el juego
		Visible = !Visible;
		GetTree().Paused = Visible;
	}

	private void OnResumePressed()
	{
		// Reanudar el juego
		Visible = false;
		GetTree().Paused = false;
	}

	private void OnQuitPressed()
	{
		// Salir del juego
		GetTree().Quit();
	}
}
