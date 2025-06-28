using Godot;

namespace CGJ2025;

public enum Periplaneta_Status
{
	FREE,
	POSSESS,
}


public partial class Periplaneta : Node2D
{
	[Export] public Periplaneta_Status Status;
	[Export] public float CheckRound;
	[Export] public float CD; // todo: global cd
	[Export] public float Speed;

	[Signal] public delegate void KillEventHandler();

	private float _remainCD;

	private Furniture _possessFurniture;

	public override void _PhysicsProcess(double delta)
	{
		if (Status == Periplaneta_Status.FREE)
		{
			FreeMove();
		}
		else if (Status == Periplaneta_Status.POSSESS)
		{
			PossessMove();
		}
	}

	public void FreeMove()
	{
		// todo
	}

	public void PossessMove()
	{
		// todo
		_possessFurniture.EmitSignal(Furniture.SignalName.PeriMove);
	}

}
