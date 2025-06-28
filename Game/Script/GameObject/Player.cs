using Godot;

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

	Vector2 moveDirection;

	public FlipFlop flipFlop;

	private Furniture _faceFurniture;
	private Furniture _holdupFurniture;


	public override void _Ready()
	{
		moveDirection = Vector2.Zero;

		if (PlayerID == PlayerID.Player1)
		{
			playerHint.Text = "1P";
		}
		else if (PlayerID == PlayerID.Player2)
		{
			playerHint.Text = "2P";
		}
	}

	public override void _Input(InputEvent @event)
	{
		// todo: move
		// todo: interactive
		// todo: holdup
		// todo: attack

		if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
		{
			if (PlayerID == PlayerID.Player1)
			{
				ProcessInputForPlayer1(keyEvent);
			}
			else if (PlayerID == PlayerID.Player2)
			{
				ProcessInputForPlayer2(keyEvent);
			}
		}
	}

	void ProcessInputForPlayer1(InputEventKey keyEvent)
	{
		moveDirection = Vector2.Zero;
		if (keyEvent.Keycode == Key.W)
		{
			moveDirection += Vector2.Up;
		}

		moveDirection = moveDirection.Normalized();
	}

	void ProcessInputForPlayer2(InputEventKey keyEvent)
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
