using Godot;

namespace CGJ2025;

public enum SlippersStatus
{
	FLYING,
	STOPPED,
}

public partial class Slippers : CharacterBody2D
{
	[Export] float FlySpeed = 1000f;
	[Export] CollisionShape2D collisionShape;

	public SlippersStatus Status;

	public override void _Ready()
	{
		base._Ready();

		Status = SlippersStatus.FLYING;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (Velocity != Vector2.Zero)
		{
			MoveAndSlide();

			var collisionCount = GetSlideCollisionCount();
			if (collisionCount > 0)
			{
				Velocity = Vector2.Zero;

				for (var i = 0; i < collisionCount; i++)
				{
					var collision = GetSlideCollision(i);
					if (collision.GetCollider() is Periplaneta periplaneta)
					{
						periplaneta.OnDamage();
					}
					if (collision.GetCollider() is Furniture furniture)
					{
						furniture.OnDamage();
					}

					GD.Print(Name, "碰撞", (collision.GetCollider() as Node).Name);
				}
			}
		}
		else
		{
			Status = SlippersStatus.STOPPED;
		}
	}

	public Vector2 FlyDirection
	{
		get => flyDirection;
		set => Velocity = value * FlySpeed;
	}

	Vector2 flyDirection;
}
