using Godot;

namespace CGJ2025;

public partial class TitleScene : Node
{
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
		{
			if (keyEvent.Keycode == Key.Escape)
			{
				GetTree().Quit();
			}
			else if (keyEvent.Keycode == Key.Enter || keyEvent.Keycode == Key.Space)
			{
				GetTree().ChangeSceneToFile("res://Scene/Main.tscn");
			}
		}
	}
}
