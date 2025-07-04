using Godot;

namespace CGJ2025;

public partial class MainScene : Node
{
	[Export] Label remainingTimeLabel;
	[Export] Label furnitureCountLabel;
	[Export] Label periplanetaCountLabel;
	[Export] public Area2D targetArea;
	[Export] AudioStreamPlayer bgmPlayer;
	
	[Export] AudioStream bgm;

	ulong gameStartTime;
	int ElapsedSeconds => (int)((Time.GetTicksMsec() - gameStartTime) / 1000);
	int RemainingSeconds => Mathf.Max(0, 180 - ElapsedSeconds);
	int RemainingMinutes => RemainingSeconds / 60;

	int totalFurnitureCount;
	int furnitureInTargetAreaCount;

	public static bool IsGameWin;

	public override void _Ready()
	{
		base._Ready();

		totalFurnitureCount = Furniture.FurnitureCount;

		gameStartTime = Time.GetTicksMsec();
		remainingTimeLabel.Text = "剩余  3:00";
		furnitureCountLabel.Text = $"夺回{totalFurnitureCount}件家具";
		periplanetaCountLabel.Text = $"或 消灭{Periplaneta.PeriplanetaCount}只蟑螂";

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

		remainingTimeLabel.Text = $"剩余  {RemainingMinutes:D1}:{RemainingSeconds % 60:D2}";
		furnitureCountLabel.Text = $"夺回{totalFurnitureCount - furnitureInTargetAreaCount}件家具";
		periplanetaCountLabel.Text = $"或 消灭{Periplaneta.PeriplanetaCount}只蟑螂";

		if (RemainingSeconds > 0)
		{
			if (totalFurnitureCount - furnitureInTargetAreaCount == 0
				|| Periplaneta.PeriplanetaCount == 0)
			{
				IsGameWin = true;
				GetTree().ChangeSceneToFile("res://Scene/Result.tscn");
			}
		}
		else
		{
			IsGameWin = false;
			GetTree().ChangeSceneToFile("res://Scene/Result.tscn");
		}
	}
}
