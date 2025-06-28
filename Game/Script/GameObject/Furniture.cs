using Godot;
using System.Collections.Generic;
using System.Linq;

namespace CGJ2025;

public enum Furniture_Status
{
	FREE,
	POSSESS,
	HOLDUP,
}

public partial class Furniture : CharacterBody2D
{
	[Export] public Furniture_Status Status;
	[Export] Label holdHint;

	[Signal] public delegate void InteractiveEventHandler();
	[Signal] public delegate void HoldupEventHandler();
	[Signal] public delegate void HoldupMoveEventHandler();
	[Signal] public delegate void PeriMoveEventHandler(Vector2 periPos, Periplaneta peri);

	private bool IsHoldup;
	Dictionary<PlayerID, bool> canHoldLookups = new() { { PlayerID.Player1, false }, { PlayerID.Player2, false } };

	public static int FurnitureCount;

	public override void _Ready()
	{
		holdHint.Visible = false;

		Interactive += OnInteractive;
		Holdup += OnHoldup;
		HoldupMove += OnMove;
		PeriMove += OnPeriMove;

		FurnitureCount += 1;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (canHoldLookups.Any(canHold => canHold.Value))
		{
			holdHint.Visible = true;
		}
		else
		{
			holdHint.Visible = false;
		}
	}

	public override void _ExitTree()
	{
		Interactive -= OnInteractive;
		Holdup -= OnHoldup;
		HoldupMove -= OnMove;
		PeriMove -= OnPeriMove;
	}

	public void OnInteractive()
	{
		if (Status == Furniture_Status.FREE)
		{
			// todo 可以拿出物品
		}
		else if (Status == Furniture_Status.POSSESS)
		{
			// todo 会敲出蟑螂
		}
	}

	public void OnHoldup()
	{
		IsHoldup = true;
		Status = Furniture_Status.HOLDUP;
		CollisionLayer = 2;
		CollisionMask = 2;
	}

	public void OnPutdown()
	{
		IsHoldup = false;
		Status = Furniture_Status.FREE;
		CollisionLayer = 3;
		CollisionMask = 3;
	}

	public void OnMove()
	{
	}

	public void OnPeriMove(Vector2 periPos, Periplaneta peri)
	{
		if (IsHoldup)
			return;

		GlobalPosition = periPos;
		peri.GlobalPosition = periPos;

	}

	public void UpdateCanHold(PlayerID playerID, bool canHold)
	{
		canHoldLookups[playerID] = canHold;
	}
}
