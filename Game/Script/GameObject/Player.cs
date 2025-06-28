using Godot;
using System.Collections.Generic;

namespace CGJ2025;

public enum PlayerID
{
	Player1,
	Player2,
}

enum PlayerStatus
{
	Normal,
	Aiming,
	Holding,
}

public partial class Player : CharacterBody2D
{
	[Export] PlayerID PlayerID;
	[Export] Label playerHint;
	[Export] Label slippersCountHint;
	[Export] Sprite2D playerSprite;
	[Export] Sprite2D crosshair;
	[Export] Sprite2D meleeFlipFlop;

	[Export] Area2D furniturePicking;
	[Export] Area2D meleeAttackRange;

	[Export] PackedScene slippersScene;

	[Export] AudioStreamPlayer soundPlayer;
	[Export] AudioStream slippersHitSound;
	[Export] AudioStream slippersFlySound;

	[Export] Texture2D player1Texture;
	[Export] Texture2D player2Texture;

	private Furniture _faceFurniture;
	private Furniture _holdupFurniture;

	PlayerStatus status;

	int slippersCount;

	Vector2 moveDirection;
	float moveSpeed = 400f;

	Vector2 crosshairPosition;
	float crosshairAngle = 0f;

	float aimingStartTime;
	float AimingTime => Time.GetTicksMsec() - aimingStartTime;
	float aimingThreshold = 100f;

	float meleeAttackStartTime;
	float MeleeAttackTime => Time.GetTicksMsec() - meleeAttackStartTime;
	float meleeAttackDuration = 200f;

	Tween meleeAttackTween;

	Dictionary<PlayerID, List<Key>> keyMappings = new()
	{
		{ PlayerID.Player1, new List<Key> { Key.W, Key.S, Key.A, Key.D, Key.Space } },
		{ PlayerID.Player2, new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right, Key.Enter } }
	};

	const int upKey = 0;
	const int downKey = 1;
	const int leftKey = 2;
	const int rightKey = 3;
	const int interactKey = 4;

	public override void _Ready()
	{
		if (PlayerID == PlayerID.Player1)
		{
			playerHint.Text = "1P";
			playerHint.Modulate = Colors.Green;
			playerSprite.Texture = player1Texture;
			slippersCountHint.Modulate = Colors.Green;
		}
		else if (PlayerID == PlayerID.Player2)
		{
			playerHint.Text = "2P";
			playerHint.Modulate = Colors.Blue;
			playerSprite.Texture = player2Texture;
			slippersCountHint.Modulate = Colors.Blue;
		}

		slippersCountHint.Text = slippersCount.ToString();

		crosshair.Visible = false;
		crosshairPosition = new Vector2(0, -200f);
		aimingStartTime = 0f;

		meleeFlipFlop.Visible = false;

		furniturePicking.BodyEntered += (body) =>
		{
			if (body is Furniture furniture && furniture.Status == Furniture_Status.FREE && status == PlayerStatus.Normal)
			{
				if (_faceFurniture != furniture)
				{
					_faceFurniture?.UpdateCanHold(PlayerID, false);

					_faceFurniture = furniture;
					_faceFurniture.UpdateCanHold(PlayerID, true);
				}
			}
			else if (body is Slippers slippers && slippers.Status == SlippersStatus.STOPPED)
			{
				slippers.QueueFree();
				slippersCount += 1;
			}
		};

		furniturePicking.BodyExited += (body) =>
		{
			if (body is Furniture furniture)
			{
				if (_faceFurniture == furniture)
				{
					_faceFurniture.UpdateCanHold(PlayerID, false);
					_faceFurniture = null;
				}
			}
		};

		meleeAttackRange.Monitoring = false;
		meleeAttackRange.Monitorable = false;

		meleeAttackRange.BodyEntered += (body) =>
		{
			if (body is Periplaneta periplaneta)
			{
				periplaneta.OnDamage();
				GD.Print(Name, "命中", periplaneta.Name);
			}
		};

		slippersCount = 1;
		status = PlayerStatus.Normal;
		moveDirection = Vector2.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (moveDirection != Vector2.Zero)
		{
			Velocity = moveDirection;
			MoveAndSlide();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		slippersCountHint.Text = slippersCount.ToString();

		ProcessInput();

		if (status == PlayerStatus.Aiming && AimingTime >= aimingThreshold)
		{
			crosshair.Visible = true;
			crosshair.Position = crosshairPosition;
		}
		else
		{
			crosshair.Visible = false;
			crosshair.Position = Vector2.Zero;
		}

		if (MeleeAttackTime >= meleeAttackDuration)
		{
			meleeAttackRange.Monitoring = false;
			meleeFlipFlop.Visible = false;
			meleeFlipFlop.Modulate = new Color(meleeFlipFlop.Modulate, 0f);
			meleeAttackTween?.Kill();
		}
	}

	void ProcessInput()
	{
		var keys = keyMappings[PlayerID];

		switch (status)
		{
			case PlayerStatus.Normal:
				if (Input.IsKeyPressed(keys[interactKey]))
				{
					if (_faceFurniture != null)
					{
						_holdupFurniture = _faceFurniture;
						_faceFurniture = null;

						_holdupFurniture.OnHoldup();
						_holdupFurniture.UpdateCanHold(PlayerID, false);
						status = PlayerStatus.Holding;

						GD.Print(Name, "搬运", _holdupFurniture);
					}
					else
					{
						aimingStartTime = Time.GetTicksMsec();
						status = PlayerStatus.Aiming;
						moveDirection = Vector2.Zero;
					}
				}
				else
				{
					ProcessMove(keys);
				}

				break;
			case PlayerStatus.Aiming:
				if (!Input.IsKeyPressed(keys[interactKey]))
				{
					if (slippersCount > 0)
					{
						if (AimingTime >= aimingThreshold)
						{
							slippersCount -= 1;
							var slippers = slippersScene.Instantiate<Slippers>();
							GetTree().CurrentScene.AddChild(slippers);

							slippers.Position = Position;
							slippers.FlyDirection = crosshairPosition.Normalized();

							soundPlayer.Stream = slippersFlySound;
							soundPlayer.Play();

							GD.Print(Name, "投掷", slippers.Name);
						}
						else
						{
							if (!meleeAttackRange.Monitoring)
							{
								meleeAttackRange.Monitoring = true;
								meleeAttackStartTime = Time.GetTicksMsec();

								meleeFlipFlop.Visible = true;
								meleeAttackTween = CreateTween();
								meleeAttackTween.TweenProperty(meleeFlipFlop, "modulate:a", 255f, meleeAttackDuration);

								soundPlayer.Stream = slippersHitSound;
								soundPlayer.Play();

								GD.Print(Name, "挥舞拖鞋");
							}
						}
					}

					status = PlayerStatus.Normal;
				}
				else
				{
					ProcessAiming(keys);
				}

				break;
			case PlayerStatus.Holding:
				if (!Input.IsKeyPressed(keys[interactKey]))
				{
					GD.Print(Name, "放下", _holdupFurniture);

					_holdupFurniture.OnPutdown();
					_holdupFurniture.UpdateCanHold(PlayerID, true);
					_faceFurniture = _holdupFurniture;
					_holdupFurniture = null;
					status = PlayerStatus.Normal;
				}
				else
				{
					ProcessMove(keys);
					_holdupFurniture.Position = Position + new Vector2(0, -100f);
				}

				break;
		}
	}

	void ProcessMove(List<Key> keys)
	{
		moveDirection = Vector2.Zero;
		if (Input.IsKeyPressed(keys[upKey]))
		{
			moveDirection += Vector2.Up;
		}

		if (Input.IsKeyPressed(keys[downKey]))
		{
			moveDirection += Vector2.Down;
		}

		if (Input.IsKeyPressed(keys[leftKey]))
		{
			moveDirection += Vector2.Left;
		}

		if (Input.IsKeyPressed(keys[rightKey]))
		{
			moveDirection += Vector2.Right;
		}

		moveDirection = moveDirection.Normalized() * moveSpeed;
	}

	void ProcessAiming(List<Key> keys)
	{
		if (Input.IsKeyPressed(keys[upKey]))
		{
			if (Mathf.Sin(crosshairAngle) > 0)
			{
				crosshairAngle -= 0.001f;
			}
			else
			{
				crosshairAngle += 0.001f;
			}
		}

		if (Input.IsKeyPressed(keys[downKey]))
		{
			if (Mathf.Sin(crosshairAngle) > 0)
			{
				crosshairAngle += 0.001f;
			}
			else
			{
				crosshairAngle -= 0.001f;
			}
		}

		if (Input.IsKeyPressed(keys[leftKey]))
		{
			if (Mathf.Cos(crosshairAngle) > 0)
			{
				crosshairAngle -= 0.001f;
			}
			else
			{
				crosshairAngle += 0.001f;
			}
		}

		if (Input.IsKeyPressed(keys[rightKey]))
		{
			if (Mathf.Cos(crosshairAngle) > 0)
			{
				crosshairAngle += 0.001f;
			}
			else
			{
				crosshairAngle -= 0.001f;
			}
		}

		crosshairPosition = (Vector2.Up * 200f).Rotated(crosshairAngle);
	}

	public void Interactive()
	{
		_faceFurniture.EmitSignal(Furniture.SignalName.Interactive);
	}

	public void Holdup()
	{
		_faceFurniture.EmitSignal(Furniture.SignalName.Holdup);
	}
}
