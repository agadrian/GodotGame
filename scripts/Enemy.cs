using Godot;

public partial class Enemy : CharacterBody2D
{
	
	public const float Speed = 50f;
	public const float DetectionRange = 200f;
	private const float AttackRange = 20f;
	private const int Damage = 10;
	public int Health = 90;
	public const float Gravity = 500f; 
	
	// Refernciar al jugador principal
	private Player player;
	// Referencia al sprite del enemy
	private AnimatedSprite2D animatedSprite;

	
	private bool isTakingDamage = false;
	private bool isAttackOnCooldown = false;
	private bool isAttacking = false;
	

	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent().GetParent().GetNode<Player>("Player");
		
		// Obtener referencia del AnimatedSprite2D del enemy
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		if (player == null || player.IsQueuedForDeletion()) return;

		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * (float)delta);
		}

		if (player != null && IsPlayerInRange() && !isTakingDamage)
		{
			if (GetDistanceToPlayer() < AttackRange && !isAttackOnCooldown && !isAttacking && !isTakingDamage)
			{
				Velocity = Vector2.Zero;
				Attack();
			}
			else if (!isAttacking)
			{
				Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
				
				Velocity = new Vector2(directionToPlayer.X * Speed, Velocity.Y);

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
			if (!isTakingDamage )
			{
				Velocity = Vector2.Zero;
				animatedSprite.Play("idle");
			}

			/* Si quiero que se quede quieto cuando reciba daño
			 if (isTakingDamage)
			{
				Velocity = Vector2.Zero;
			}
			*/
			
		}
	
		MoveAndSlide();
	}


	public void Attack()
	{

		if (player == null) return;
			
		isAttacking = true;
		animatedSprite.Play("attack");
		
		isAttackOnCooldown = true;
		
		
		var cooldownTimerEnemy = new Timer();
		cooldownTimerEnemy.WaitTime = 1f;
		cooldownTimerEnemy.OneShot = true;
		cooldownTimerEnemy.Connect("timeout", new Callable(this, nameof(OnAttackCooldownComplete)));
		AddChild(cooldownTimerEnemy);
		cooldownTimerEnemy.Start();
		
		GD.Print("despues de coldown");
		player.canAttack = false;
		player.TakeDamage(Damage);
	}
	
	private void OnAttackCooldownComplete()
	{
		GD.Print("Cooldown de ataque completado.");
		isAttackOnCooldown = false;
		isAttacking = false;
		player.canAttack = true;
	}
	
	
	

	public void TakeDamage(int damage)
	{
		Health -= damage;
		isTakingDamage = true;
		GD.Print("Enemy Taking damage");
		
		animatedSprite.Play("hurt");
		
		// Esperar para la animacion de daño
		var timer = new Timer();
		timer.WaitTime = 0.8f;  
		timer.OneShot = true;
		timer.Connect("timeout", new Callable(this, nameof(ResetHurtState)));
		AddChild(timer);
		timer.Start();
		

		if (Health <= 0)
		{
			Die();
		}
		
	}

	private void ResetHurtState()
	{
		isTakingDamage = false;
	}
	
	private void Die()
	{
		Velocity = Vector2.Zero;
		animatedSprite.Play("death");
		
		// Crear un temporizador para esperar al final de la animación
		var deathTimer = new Timer();
		deathTimer.WaitTime = 0.8f;  
		deathTimer.OneShot = true; 
		deathTimer.Connect("timeout", new Callable(this, nameof(OnDeathAnimationEnd)));
		AddChild(deathTimer);
		deathTimer.Start();
	}
	
	private void OnDeathAnimationEnd()
	{
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
