using Godot;
using System;

namespace CGJ2025;

public enum PlayerID
{
	Player1,
	Player2,
}

public partial class Player : CharacterBody2D
{
	[Export] public PlayerID PlayerID;

	public FlipFlop flipFlop;

	private Furniture _faceFurniture;
	private Furniture _holdupFurniture;


	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		// todo: move
		// todo: interactive
		// todo: holdup
		// todo: attack
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
