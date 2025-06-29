using Godot;

namespace CGJ2025;

public partial class ResultScene : Control
{
	[Export] Label winLabel;
	[Export] Label loseLable;

	[Export] TextureRect restartRect;
	[Export] TextureRect quitRect;

	int currentSelection;

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

		currentSelection = 0;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		switch (currentSelection)
		{
			case 0:
				restartRect.Scale = new Vector2(1.1f, 1.1f);
				quitRect.Scale = new Vector2(1.0f, 1.0f);
				break;
			case 1:
				restartRect.Scale = new Vector2(1.0f, 1.0f);
				quitRect.Scale = new Vector2(1.1f, 1.1f);
				break;
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
		{
			if (keyEvent.Keycode == Key.Enter || keyEvent.Keycode == Key.Space)
			{
				if (currentSelection == 0)
				{
					Furniture.FurnitureCount = 0;
					Periplaneta.PeriplanetaCount = 0;
					MainScene.IsGameWin = false;

					GetTree().ChangeSceneToFile("res://Scene/Main.tscn");
				}
				else if (currentSelection == 1)
				{
					GetTree().ChangeSceneToFile("res://Scene/Title.tscn");
				}
			}
			else if (keyEvent.Keycode == Key.W || keyEvent.Keycode == Key.Up)
			{
				currentSelection = (currentSelection - 1 + 2) % 2; // Wrap around
			}
			else if (keyEvent.Keycode == Key.S || keyEvent.Keycode == Key.Down)
			{
				currentSelection = (currentSelection + 1) % 2; // Wrap around
			}
		}
	}
}
