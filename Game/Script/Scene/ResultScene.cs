using Godot;

namespace CGJ2025;

public partial class ResultScene : Control
{
	[Export] Label winLabel;
	[Export] Label loseLable;

	public override void _Ready()
	{
		base._Ready();

		if (MainScene.IsGameWin)
		{
			winLabel.Visible = true;
			loseLable.Visible = false;
		}
		else
		{
			winLabel.Visible = false;
			loseLable.Visible = true;
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);


	}
}
