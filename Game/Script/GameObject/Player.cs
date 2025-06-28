using Godot;
using System.Collections.Generic;

namespace CGJ2025;

public enum PlayerID
{
	Player1,
	Player2,
}

enum PlayerStatus
{
	Normal,
	Aiming,
	Holding,
}

public partial class Player : CharacterBody2D
{
	[Export] PlayerID PlayerID;
	[Export] Label playerHint;
	[Export] Sprite2D crosshair;

	public FlipFlop flipFlop;

	private Furniture _faceFurniture;
	private Furniture _holdupFurniture;

	PlayerStatus status;

	Vector2 moveDirection;
	float moveSpeed = 400f;

	Vector2 crosshairPosition;
	float crosshairAngle = 0f;

	float aimingStartTime;
	float AimingTime => Time.GetTicksMsec() - aimingStartTime;
	float aimingThreshold = 100f;

	Dictionary<PlayerID, List<Key>> keyMappings = new()
	{
		{ PlayerID.Player1, new List<Key> { Key.W, Key.S, Key.A, Key.D, Key.Space } },
		{ PlayerID.Player2, new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right, Key.Enter } }
	};

	const int upKey = 0;
	const int downKey = 1;
	const int leftKey = 2;
	const int rightKey = 3;
	const int interactKey = 4;

	public override void _Ready()
	{
		if (PlayerID == PlayerID.Player1)
		{
			playerHint.Text = "1P";
		}
		else if (PlayerID == PlayerID.Player2)
		{
			playerHint.Text = "2P";
		}

		crosshair.Visible = false;
		crosshairPosition = new Vector2(0, -200f);
		aimingStartTime = 0f;

		status = PlayerStatus.Normal;
		moveDirection = Vector2.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (moveDirection != Vector2.Zero)
		{
			Velocity = moveDirection;
			MoveAndSlide();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		ProcessInput();

		if (status == PlayerStatus.Aiming && AimingTime >= aimingThreshold)
		{
			crosshair.Visible = true;
			crosshair.Position = crosshairPosition;
		}
		else
		{
			crosshair.Visible = false;
			crosshair.Position = Vector2.Zero;
		}
	}

	void ProcessInput()
	{
		var keys = keyMappings[PlayerID];

		switch (status)
		{
			case PlayerStatus.Normal:
				if (Input.IsKeyPressed(keys[interactKey]))
				{
					aimingStartTime = Time.GetTicksMsec();
					status = PlayerStatus.Aiming;
					moveDirection = Vector2.Zero;
				}
				else
				{
					ProcessMove(keys);
				}

				break;
			case PlayerStatus.Aiming:
				if (!Input.IsKeyPressed(keys[interactKey]))
				{
					if (AimingTime >= aimingThreshold)
					{
					}
					else
					{
					}

					status = PlayerStatus.Normal;
				}
				else
				{
					ProcessAiming(keys);
				}

				break;
			case PlayerStatus.Holding:
				break;
		}
	}

	void ProcessMove(List<Key> keys)
	{
		moveDirection = Vector2.Zero;
		if (Input.IsKeyPressed(keys[upKey]))
		{
			moveDirection += Vector2.Up;
		}

		if (Input.IsKeyPressed(keys[downKey]))
		{
			moveDirection += Vector2.Down;
		}

		if (Input.IsKeyPressed(keys[leftKey]))
		{
			moveDirection += Vector2.Left;
		}

		if (Input.IsKeyPressed(keys[rightKey]))
		{
			moveDirection += Vector2.Right;
		}

		moveDirection = moveDirection.Normalized() * moveSpeed;
	}

	void ProcessAiming(List<Key> keys)
	{
		if (Input.IsKeyPressed(keys[upKey]))
		{
			if (Mathf.Sin(crosshairAngle) > 0)
			{
				crosshairAngle -= 0.001f;
			}
			else
			{
				crosshairAngle += 0.001f;
			}
		}

		if (Input.IsKeyPressed(keys[downKey]))
		{
			if (Mathf.Sin(crosshairAngle) > 0)
			{
				crosshairAngle += 0.001f;
			}
			else
			{
				crosshairAngle -= 0.001f;
			}
		}

		if (Input.IsKeyPressed(keys[leftKey]))
		{
			if (Mathf.Cos(crosshairAngle) > 0)
			{
				crosshairAngle -= 0.001f;
			}
			else
			{
				crosshairAngle += 0.001f;
			}
		}

		if (Input.IsKeyPressed(keys[rightKey]))
		{
			if (Mathf.Cos(crosshairAngle) > 0)
			{
				crosshairAngle += 0.001f;
			}
			else
			{
				crosshairAngle -= 0.001f;
			}
		}

		crosshairPosition = (Vector2.Up * 200f).Rotated(crosshairAngle);
	}

	public void Interactive()
	{
		_faceFurniture.EmitSignal(Furniture.SignalName.Interactive);
	}

	public void Holdup()
	{
		_faceFurniture.EmitSignal(Furniture.SignalName.Holdup);
	}

	public void FreeMove()
	{
		// todo
	}

	public void HoldupMove()
	{
		// todo
		_holdupFurniture.EmitSignal(Furniture.SignalName.HoldupMove);
	}
}
