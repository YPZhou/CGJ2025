using Godot;

namespace CGJ2025;

public partial class MainScene : Node
{
	[Export] Label remainingTimeLabel;

	ulong gameStartTime;
	int ElapsedSeconds => (int)((Time.GetTicksMsec() - gameStartTime) / 1000);
	int RemainingSeconds => Mathf.Max(0, 180 - ElapsedSeconds);
	int RemainingMinutes => RemainingSeconds / 60;

	public override void _Ready()
	{
		base._Ready();

		gameStartTime = Time.GetTicksMsec();
		remainingTimeLabel.Text = "3:00";
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		remainingTimeLabel.Text = $"{RemainingMinutes:D1}:{RemainingSeconds % 60:D2}";
	}
}
