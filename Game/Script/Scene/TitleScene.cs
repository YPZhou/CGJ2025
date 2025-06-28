using Godot;

namespace CGJ2025;

public partial class TitleScene : Node
{
	public override void _Ready()
	{
		base._Ready();

		currentSelection = 0;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		switch (currentSelection)
		{
			case 0:
				startRect.Scale = new Vector2(1.1f, 1.1f);
				startLabel.Visible = true;
				quitRect.Scale = new Vector2(1.0f, 1.0f);
				quitLabel.Visible = false;
				break;
			case 1:
				startRect.Scale = new Vector2(1.0f, 1.0f);
				startLabel.Visible = false;
				quitRect.Scale = new Vector2(1.1f, 1.1f);
				quitLabel.Visible = true;
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
					GetTree().ChangeSceneToFile("res://Scene/Main.tscn");
				}
				else if (currentSelection == 1)
				{
					GetTree().Quit();
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

	int currentSelection;

	[Export] TextureRect startRect;
	[Export] Label startLabel;
	[Export] TextureRect quitRect;
	[Export] Label quitLabel;
}
