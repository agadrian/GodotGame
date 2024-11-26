using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120.0f;
	public const float JumpVelocity = -300.0f;
	public const int AttackDamage = 10;
	
	// Referncia al sprite del Pj
	private AnimatedSprite2D _animatedSprite;
	// Referencia al nodo del enemy
	private Enemy _enemy;  
	// Rango de ataque cuerpo a cuerpo
	private const float AttackRange = 30f;
	private string _currentState = "Idle";
	private bool _isAttacking = false;

	
	

	
	public override void _Ready()
	{
		// Inicializar el nodo del sprite animado
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		// Obtener referencia del enemigo
		_enemy = GetParent().GetNode<Enemy>("Enemy");
		
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Agregar gravedad
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Manejarr salto
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Obtener la direccion del sprite x
		float direction = Input.GetAxis("move_left", "move_right");
		
		if (direction > 0)
		{
			_animatedSprite.FlipH = false;
		}
		else if (direction < 0)
		{
			_animatedSprite.FlipH = true;
		}
		
		
		// Aplicar aniamciones segun el estado del Pj
		if (IsOnFloor() && _isAttacking == false)
		{
			if (direction == 0)
			{
				SetState("Idle");
			}
			else
			{
				SetState("Run");
			}
		}
		else if (!IsOnFloor())
		{
			SetState("Jump");
		}
		else
		{
			SetState("Attack");
		}
		
		// Aplicar movimiento
		if (direction != 0)
		{
			velocity.X = direction * Speed;	
		}
		else
		{
			velocity.X = 0;
		}
		
		
		// Manjear ataque cuerpo a cuerpo
		if (Input.IsActionJustPressed("bodyAttack") && IsOnFloor())
		{
			Attack();
		}
		
		if (Input.IsActionJustPressed("shoot") && IsOnFloor() )
		{
			SetState("Shoot");
		}
		
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	
	

	public void Attack()
	{
		
		_isAttacking = true;
		SetState("Attack");

		if (_animatedSprite.Frame == 10)
		{
			_isAttacking = false;
			GD.Print("dfdfd");
		}
		
		
		if (_enemy !=null)
		{
			float distanceToEnemy = GlobalPosition.DistanceTo(_enemy.GlobalPosition);

			if (distanceToEnemy <= AttackRange)
			{
				_enemy.TakeDamage(AttackDamage);
				GD.Print("atack");
			}
		}
		
		
		
	}
	
	private void SetState(string newState)
	{
		if (_currentState != newState)
		{
			_currentState = newState;

			switch (newState)
			{
				case "Idle":
					_animatedSprite.Play("idle");
					break;
				case "Run":
					_animatedSprite.Play("run");
					break;
				case "Jump":
					_animatedSprite.Play(
						"jump");
					break;
				case "Attack":
					_animatedSprite.Play("attack");
					break;
				case "Shoot":
					_animatedSprite.Play("shoot");
					break;
			}
		}
	}
	
	

	
	
}
