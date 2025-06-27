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
	[Signal] public delegate void MoveEventHandler();

	public override void _Ready()
	{
		Interactive += OnInteractive;
		Holdup += OnHoldup;
		Move += OnMove;
	}

	public override void _ExitTree()
	{
		Interactive -= OnInteractive;
		Holdup -= OnHoldup;
		Move -= OnMove;
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

	public void OnMove()
	{

	}

	public void OnHoldup()
	{

	}
}
