using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120.0f;
	public const float JumpVelocity = -300.0f;
	public const int AttackDamage = 10;
	public int Health = 120;
	
	// Referncia al sprite del Pj
	private AnimatedSprite2D _animatedSprite;
	// Referencia al nodo del enemy
	private Enemy _enemy;  
	// Rango de ataque cuerpo a cuerpo
	private const float AttackRange = 30f;
	private float AttackCooldown = 0.6f;
	public bool canAttack = true;
	private bool isTakingDamage = false;
	private bool isDead;
	
	private AnimationTree _animationTree;
	private AnimationNodeStateMachinePlayback _stateMachine;
	private AnimationPlayer _animationPlayer;
	
	// Referencia a la escena de la bala
	// Pre-cargar la escena manualmente
	private static PackedScene _bulletScene = (PackedScene)ResourceLoader.Load("res://scenes/bullet.tscn");

	
	

	
	public override void _Ready()
	{
		// Inicializar el nodo del sprite animado
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		// Obtener referencia del enemigo
		_enemy = GetParent().GetNode<Enemy>("Enemy");
		
		
		// Referenciar al AnimationTree
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		
		if (_animationTree != null)
		{
			GD.Print("AnimationTree encontrado y referenciado correctamente.");
			
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			
			
			_animationTree.Active = true;
			
			// Conversión explícita del nodo de playback
			_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

			if (_stateMachine != null)
			{
				GD.Print("StateMachinePlayback obtenido correctamente.");
			}
			else
			{
				GD.PrintErr("Error al obtener StateMachinePlayback. Verifica la configuración del AnimationTree.");
		 
			}
			
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
		
		
		// Salto
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			//_animationTree.Set("parameters/conditions/jump", true);
		}
		
		if (!IsOnFloor())
		{
			//GD.Print("aire");
			_animationTree.Set("parameters/conditions/jump", true);
			
		}
		
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
				Godot.GD.Print("ataaque");
				Attack();
			}
			else
			{
				GD.Print("Cooldown");
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
	
	private void OnAttackAnimationEnd()
	{
		GD.Print("Ataque Cuerpo a Cuerpo Terminado");
		_animationTree.Set("parameters/conditions/attack", false);
	}
	
	private void OnCooldownEnd()
	{
		GD.Print("Cooldown terminado, ahora puedes atacar nuevamente.");
		canAttack = true;  
	}

	public void TakeDamge(int damage)
	{
		if (!isTakingDamage)
		{
			Health -= damage;
			GD.Print("Player hurt! Health: " + Health);
			
			isTakingDamage = true;
			
			_animationTree.Set("parameters/conditions/hurt", true);

			// Timer para permitir que el jugador reciba daño nuevamente después de la animación
			var hurtTimer = new Timer();
			hurtTimer.WaitTime = 0.5f; // Duración de la animación de daño
			hurtTimer.OneShot = true;
			hurtTimer.Connect("timeout", new Callable(this, nameof(ResetDamageState)));
			AddChild(hurtTimer);
			hurtTimer.Start();
			
			

			// Comprobar si el jugador muere
			if (Health <= 0)
			{
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
		
		if (isDead) return;
		
		isDead = true;
		
		
		_animationTree.Set("parameters/conditions/dead", true);
		var deadTimer = new Timer();
		deadTimer.WaitTime = 1f; // Duración de la animación de daño
		deadTimer.OneShot = true;
		deadTimer.Connect("timeout", new Callable(this, nameof(OnDeathComplete)));
		AddChild(deadTimer);
		deadTimer.Start();
		
		
		// Aquí puedes añadir la animación de muerte y/o lógica de reinicio
	}
	
	private void OnDeathComplete()
	{
		GD.Print("Player has died!");
		QueueFree(); // Elimina al jugador de la escena
	}
	
}
