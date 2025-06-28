using Godot;

namespace CGJ2025;

public partial class MainScene : Node
{
	[Export] Label remainingTimeLabel;
	[Export] public Area2D targetArea;
	[Export] AudioStreamPlayer bgmPlayer;
	
	[Export] AudioStream bgm;

	ulong gameStartTime;
	int ElapsedSeconds => (int)((Time.GetTicksMsec() - gameStartTime) / 1000);
	int RemainingSeconds => Mathf.Max(0, 180 - ElapsedSeconds);
	int RemainingMinutes => RemainingSeconds / 60;

	int furnitureInTargetAreaCount;

	public override void _Ready()
	{
		base._Ready();

		gameStartTime = Time.GetTicksMsec();
		remainingTimeLabel.Text = "3:00";

		furnitureInTargetAreaCount = 0;
		targetArea.BodyEntered += (body) =>
		{
			GD.Print(body.Name, "进入目标区域");
			if (body is Furniture)
			{
				furnitureInTargetAreaCount += 1;
				GD.Print("已搬运家具", furnitureInTargetAreaCount);
			}
		};

		targetArea.BodyExited += (body) =>
		{
			GD.Print(body.Name, "离开目标区域");
			if (body is Furniture)
			{
				furnitureInTargetAreaCount -= 1;
				GD.Print("已搬运家具", furnitureInTargetAreaCount);
			}
		};

		bgmPlayer.Stream = bgm;
		bgmPlayer.Finished += () => bgmPlayer.Play();
		bgmPlayer.Play();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		remainingTimeLabel.Text = $"{RemainingMinutes:D1}:{RemainingSeconds % 60:D2}";
	}
}
