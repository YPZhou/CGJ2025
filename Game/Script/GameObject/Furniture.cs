using Godot;
using System;

public enum Furniture_Status
{
	FREE,
	POSSESS,
}

public partial class Furniture : Node2D
{
	[Export] public Furniture_Status Status;

	[Signal] public delegate void InteractiveEventHandler();
	[Signal] public delegate void HoldupEventHandler();
	[Signal] public delegate void HoldupMoveEventHandler();
	[Signal] public delegate void PeriMoveEventHandler();

	private bool IsHoldup;

	public override void _Ready()
	{
		Interactive += OnInteractive;
		Holdup += OnHoldup;
		HoldupMove += OnMove;
		PeriMove += OnPeriMove;
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
}
