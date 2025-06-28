using Godot;

namespace CGJ2025;

public enum SlippersStatus
{
	FLYING,
	STOPPED,
}

public partial class Slippers : Node2D
{
	public SlippersStatus Status;

	public override void _Ready()
	{
		base._Ready();

		Status = SlippersStatus.FLYING;
	}
}
