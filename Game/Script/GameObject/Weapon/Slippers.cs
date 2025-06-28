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

			if (GetSlideCollisionCount() > 0)
			{
				Velocity = Vector2.Zero;

				GD.Print(Name, "碰撞", (GetLastSlideCollision().GetCollider() as Node).Name);
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
