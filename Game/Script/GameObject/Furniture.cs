using Godot;
using System.Collections.Generic;
using System.Linq;

namespace CGJ2025;

public enum Furniture_Status
{
	FREE,
	POSSESS,
}

public partial class Furniture : Node2D
{
	[Export] public Furniture_Status Status;
	[Export] Label holdHint;

	[Signal] public delegate void InteractiveEventHandler();
	[Signal] public delegate void HoldupEventHandler();
	[Signal] public delegate void HoldupMoveEventHandler();
	[Signal] public delegate void PeriMoveEventHandler();

	private bool IsHoldup;
	Dictionary<PlayerID, bool> canHoldLookups = new() { { PlayerID.Player1, false }, { PlayerID.Player2, false } };

	public override void _Ready()
	{
		holdHint.Visible = false;

		Interactive += OnInteractive;
		Holdup += OnHoldup;
		HoldupMove += OnMove;
		PeriMove += OnPeriMove;
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
		IsHoldup = !IsHoldup;
	}

	public void OnMove()
	{
	}

	public void OnPeriMove()
	{
		if (IsHoldup)
			return;
	}

	public void ToggleCanHold(PlayerID playerID)
	{
		canHoldLookups[playerID] = !canHoldLookups[playerID];
	}
}
