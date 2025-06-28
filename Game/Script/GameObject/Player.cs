using Godot;
using System.Collections.Generic;

namespace CGJ2025;

public enum PlayerID
{
	Player1,
	Player2,
}

public partial class Player : CharacterBody2D
{
	[Export] PlayerID PlayerID;
	[Export] Label playerHint;
	[Export] Sprite2D cursor;

	public FlipFlop flipFlop;

	private Furniture _faceFurniture;
	private Furniture _holdupFurniture;

	Vector2 moveDirection;
	float moveSpeed = 400f;

	bool isAiming;

	Dictionary<PlayerID, List<Key>> keyMappings = new()
	{
		{ PlayerID.Player1, new List<Key> { Key.W, Key.S, Key.A, Key.D } },
		{ PlayerID.Player2, new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right } }
	};

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

		cursor.Visible = false;

		moveDirection = Vector2.Zero;
		isAiming = false;
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
	}

	void ProcessInput()
	{
		var keys = keyMappings[PlayerID];

		ProcessAiming(keys);
		ProcessMove(keys);
	}

	void ProcessMove(List<Key> keys)
	{
		moveDirection = Vector2.Zero;
		if (Input.IsKeyPressed(keys[0]))
		{
			moveDirection += Vector2.Up;
		}

		if (Input.IsKeyPressed(keys[1]))
		{
			moveDirection += Vector2.Down;
		}

		if (Input.IsKeyPressed(keys[2]))
		{
			moveDirection += Vector2.Left;
		}

		if (Input.IsKeyPressed(keys[3]))
		{
			moveDirection += Vector2.Right;
		}

		moveDirection = moveDirection.Normalized() * moveSpeed;
	}

	void ProcessAiming(List<Key> keys)
	{
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
		_holdupFurniture.EmitSignal(Furniture.SignalName.Move);
	}
}
