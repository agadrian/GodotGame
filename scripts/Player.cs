using Godot;


public partial class Player : CharacterBody2D
{
	public float Speed = 120.0f;
	public const float JumpVelocity = -320.0f;
	public const int AttackDamage = 15;
	public int Health = 180;
	
	// Referncia al sprite del Pj
	private AnimatedSprite2D _animatedSprite;
	private Enemy _enemy;  
	private const float AttackRange = 30f;
	private float AttackCooldown = 0.6f;
	public bool canAttack = true;
	private bool isTakingDamage = false;
	private bool isDead;
	
	private AnimationTree _animationTree;
	private AnimationPlayer _animationPlayer;
	
	
	
	public override void _Ready()
	{
		// Inicializar el nodo del sprite animado
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		// Obtener referencia del enemigo
		_enemy = GetParent().GetNode<Enemy>("Enemies/Enemy");
		
		
		
		// Referenciar al AnimationTree
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		
		if (_animationTree != null)
		{
			GD.Print("AnimationTree encontrado y referenciado correctamente.");
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			_animationTree.Active = true;
		}
		else
		{
			GD.PrintErr("Error: No se encontró el nodo AnimationTree. Revisa la ruta.");
		}
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		
		// Agregar gravedad
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
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

		velocity = update_animation_parameters(velocity);
		
		
		// Aplicar aniamciones y movimientos segun el estado del Pj
		
		if (direction != 0)
		{
			velocity.X = direction * Speed;
		}
		else
		{
			velocity.X = 0;
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}


	private Vector2 update_animation_parameters(Vector2 velocity)
	{
		// Idle y run
		_animationTree.Set("parameters/conditions/idle", velocity.X == 0 && IsOnFloor());
		_animationTree.Set("parameters/conditions/run", velocity.X != 0 && IsOnFloor());
		
		
		// Velocidad Y
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		
		// Salto on
		if (!IsOnFloor())
		{
			//GD.Print("aire");
			_animationTree.Set("parameters/conditions/jump", true);
			
		}
		
		// Salto off
		if (IsOnFloor())
		{
			//GD.Print("suelo");
			_animationTree.Set("parameters/conditions/jump", false);
		}
		
		
		
		// Manjear ataque cuerpo a cuerpo
		if (Input.IsActionJustPressed("bodyAttack") && IsOnFloor())
		{
			if (canAttack)
			{
				//Godot.GD.Print("ataque");
				Attack();
			}
		}
		
		return velocity;
	}
	

	
	public void Attack()
	{
		if (!canAttack) return;
		
		Godot.GD.Print("attack");
		_animationTree.Set("parameters/conditions/attack", true);
		
		float attackDuration = _animationPlayer.GetAnimation("attack").Length;
		
		// Temp para esperar la animacion
		var attackTimer = new Timer();
		attackTimer.WaitTime = attackDuration;
		attackTimer.OneShot = true;
		AddChild(attackTimer);
		attackTimer.Connect("timeout", new Callable(this, nameof(OnAttackAnimationEnd)));
		attackTimer.Start();
		
		// Desactivar ataque hasta finalizar el cooldown
		canAttack = false;
		
		var cooldownTimer = new Timer();
		cooldownTimer.WaitTime = AttackCooldown;
		cooldownTimer.OneShot = true;
		cooldownTimer.Connect("timeout", new Callable(this, nameof(OnCooldownEnd)));
		AddChild(cooldownTimer);
		cooldownTimer.Start();

		Enemy closestEnemy = null;
		float closestDistance = AttackRange;

		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
		{
			if (node is Enemy enemy)
			{
				float distanceToEnemy = GlobalPosition.DistanceTo(enemy.GlobalPosition);

				if (distanceToEnemy <= closestDistance)
				{
					closestDistance = distanceToEnemy;
					closestEnemy = enemy;
					//GD.Print("atack");
				}
			}

		}
		
		// Atacar si encuentra enemigo cercano
		if (closestEnemy != null)
		{
			closestEnemy.TakeDamage(AttackDamage);
			GD.Print("Attacking the closest enemy!");
		}

	}
	
	private void OnAttackAnimationEnd()
	{
		//GD.Print("Ataque Cuerpo a Cuerpo Terminado");
		_animationTree.Set("parameters/conditions/attack", false);
	}
	
	private void OnCooldownEnd()
	{
		//GD.Print("Cooldown terminado, ahora puedes atacar nuevamente.");
		canAttack = true;  
	}

	public void TakeDamage(int damage)
	{
		if (!isTakingDamage)
		{
			Health -= damage;
			GD.Print("Player herido! Vida: " + Health);
			
			isTakingDamage = true;
			
			_animationTree.Set("parameters/conditions/hurt", true);

			// Timer para permitir que el jugador reciba daño nuevamente después de la animación
			var hurtTimer = new Timer();
			hurtTimer.WaitTime = 0.5f; 
			hurtTimer.OneShot = true;
			hurtTimer.Connect("timeout", new Callable(this, nameof(ResetDamageState)));
			AddChild(hurtTimer);
			hurtTimer.Start();
			
			
			// Comprobar si el jugador muere
			if (Health <= 0)
			{
				Speed = 0; // Para que no se mueva mientras hace la animacion de muerte
				Die();
			}
		}
	}
	
	private void ResetDamageState()
	{
		isTakingDamage = false;
		_animationTree.Set("parameters/conditions/hurt", false);
	}
	
	private void Die()
	{
		if (isDead)
		{
			//GD.Print("Is already dead");
			return;
		}
		
		isDead = true;
		
		
		_animationTree.Set("parameters/conditions/dead", true);
		var deadTimer = new Timer();
		deadTimer.WaitTime = 1f; 
		deadTimer.OneShot = true;
		deadTimer.Connect("timeout", new Callable(this, nameof(OnDeathComplete)));
		AddChild(deadTimer);
		deadTimer.Start();
		
		
	}
	
	private void OnDeathComplete()
	{
		GD.Print("Player has died!");
		QueueFree();
		GetTree().CallDeferred("change_scene_to_file", "res://scenes/GameOver.tscn");
	}
	
	
}
