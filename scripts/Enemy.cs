using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	
	public const float Speed = 50f;
	public const float DetectionRange = 200f;
	private const float AttackRange = 20f;
	public int Health = 100;
	
	// Refernciar al jugador principal
	private Node2D player;
	// Referencia al sprite del enemy
	private AnimatedSprite2D animatedSprite;

	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent().GetNode<Node2D>("Player");
		
		// Obtener referencia del AnimatedSprite2D del enemy
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (player != null && IsPlayerInRange())
		{
			if (GetDistanceToPlayer() < AttackRange)
			{
				Velocity = Vector2.Zero;
				animatedSprite.Play("attack");
			}
			else
			{
				Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();

				Velocity = new Vector2(directionToPlayer.X * Speed, 0);

				// Dirección del enemy
				if (Velocity.X > 0)
				{
					animatedSprite.FlipH = false;
				}
				else
				{
					animatedSprite.FlipH = true;
				}
			
				animatedSprite.Play("run");
			}
			
		}
		else
		{
			Velocity = Vector2.Zero;
			animatedSprite.Play("idle");
		}

		MoveAndSlide();
	}


	public void TakeDamage(int damage)
	{
		Health -= damage;
		
		animatedSprite.Play("hurt");

		if (Health <= 0)
		{
			Die();
		}
	}

	
	private void Die()
	{
		animatedSprite.Play("death");
		QueueFree();
	}

	
	private float GetDistanceToPlayer()
	{
		return GlobalPosition.DistanceTo(player.GlobalPosition);
	}
	
	// Verificar si el jugador esta en el rango de deteccion del enemigo
	private bool IsPlayerInRange()
	{
		return GlobalPosition.DistanceTo(player.GlobalPosition) <= DetectionRange;
	}
}
