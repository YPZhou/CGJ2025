using Godot;
using System.Collections.Generic;
using static Godot.TextServer;

namespace CGJ2025;

public enum PeriplanetaStates
{
	Outside,  // 家具外面
	Entering, // 进入家具
	Inside,   // 家具内
	ForcedOut,// 赶出家具
	Dead,     // 死亡
}

public partial class Periplaneta : CharacterBody2D
{
	[Export] public float AlertDistence = 2000f;
	[Export] public float DangerDistence = 500f;
	[Export] public float CD = 5f;
	[Export] public float BaseSpeed = 900f;
	[Export] public float DangerSpeed = 1800f;
	[Export] public float PossessSpeed = 500f;
	[Export] public int   MaxHealth = 15;
	[Export] public int   MaxHit = 3;

	[Signal] public delegate void DamageEventHandler();

	private Sprite2D _sprite;
	private CollisionShape2D _collisionShape;


	public bool _isInFurniture = false;
	public Player _player1;
	public Player _player2;
	public int currentDamage;
	public int currentHit;
	public float _remainCD;
	public Furniture _possessFurniture;
	public Furniture _targetFurniture;
	public PeriplanetaStates _periplanetaStates;

	public Vector2 moveDirection;
	public float _rotation;

	private bool isDead = false;
	
	public static int PeriplanetaCount;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		List<Player> players = FindAllPlayers(GetTree().Root);

		if (players.Count >= 2)
		{
			_player1 = players[0];
			_player2 = players[1];
			GD.Print("成功分配两个Player节点");
		}
		else if (players.Count == 1)
		{
			_player1 = players[0];
			GD.PrintErr("警告：仅找到1个Player节点，_player2未赋值");
		}
		else
		{
			GD.PrintErr("错误：未找到任何Player节点");
		}

		currentDamage = 0;
		currentHit = 0;

		_periplanetaStates = PeriplanetaStates.Outside;
		_isInFurniture = false;
		_remainCD = CD;
		
		moveDirection = Vector2.Zero;

		PeriplanetaCount += 1;

	}

	public override void _PhysicsProcess(double delta)
	{
		if (moveDirection != Vector2.Zero)
		{
			Velocity = moveDirection;
			Rotation = _rotation;
			MoveAndSlide();

			if (_isInFurniture)
			{
				_possessFurniture.OnPeriMove(Position, this);
			}
		}
	}


	public override void _Process(double delta)
	{
		if (_periplanetaStates == PeriplanetaStates.Outside)
		{
			_remainCD -= (float)delta;
			if (_remainCD <= 0)
			{
				_targetFurniture = FindNearestFreeFurniture(GetTree().Root);
				if (_targetFurniture != null)
				{
					_periplanetaStates = PeriplanetaStates.Entering;
					return;
				}
			}

			float distToPlayer1 = GlobalPosition.DistanceTo(_player1.GlobalPosition);
			float distToPlayer2 = GlobalPosition.DistanceTo(_player2.GlobalPosition);

			float distToPlayer = Mathf.Min(distToPlayer1, distToPlayer2);
			Vector2 fleeDir = (GlobalPosition - _player1.GlobalPosition).Normalized();

			if (distToPlayer2 < distToPlayer1)
			{
				fleeDir = (GlobalPosition - _player2.GlobalPosition).Normalized();
			}

			if (distToPlayer < DangerDistence)
			{
				moveDirection = fleeDir * DangerSpeed;
				SetRotationToDirection(fleeDir);
			}
			else if (distToPlayer < AlertDistence)
			{
				if (GD.Randf() < 0.01f)
				{
					Vector2 wanderDir = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1).Normalized();
					moveDirection = wanderDir * BaseSpeed;
					SetRotationToDirection(wanderDir);
				}
			}
			else
			{
				moveDirection = Vector2.Zero;
			}

			return;
		}

		if (_periplanetaStates == PeriplanetaStates.Entering)
		{
			Vector2 dir = (_targetFurniture.GlobalPosition - GlobalPosition).Normalized();
			SetRotationToDirection(dir);
			moveDirection = dir * DangerSpeed;

			float distToFurniture = GlobalPosition.DistanceTo(_targetFurniture.GlobalPosition);

			float stepDistance = (float)delta * moveDirection.Length();

			if (_targetFurniture.Status != Furniture_Status.FREE)
			{
				_periplanetaStates = PeriplanetaStates.Outside;
				_targetFurniture = null;
				return;
			}

			if (distToFurniture < stepDistance * 3)
			{
				if (_targetFurniture.Status == Furniture_Status.FREE)
				{
					_isInFurniture = true;
					_possessFurniture = _targetFurniture;
					_possessFurniture.UpdatePeriInside(this);
					_periplanetaStates = PeriplanetaStates.Inside;
					_sprite.Modulate = new Color(1, 1, 1, 0);
					CollisionLayer = 4;
					CollisionMask = 4;
					moveDirection = Vector2.Zero;
				}
				else
				{
					_periplanetaStates = PeriplanetaStates.Outside;
					_targetFurniture = null;
				}
			}
			return;
		}

		if (_periplanetaStates == PeriplanetaStates.Inside)
		{

			float distToPlayer1 = GlobalPosition.DistanceTo(_player1.GlobalPosition);
			float distToPlayer2 = GlobalPosition.DistanceTo(_player2.GlobalPosition);

			float distToPlayer = Mathf.Min(distToPlayer1, distToPlayer2);
			Vector2 moveDir = (GlobalPosition - _player1.GlobalPosition).Normalized();

			if (distToPlayer2 < distToPlayer1)
			{
				moveDir = (GlobalPosition - _player2.GlobalPosition).Normalized();
			}

			// 危险距离内：携带家具远离玩家
			if (distToPlayer < AlertDistence)
			{
				moveDirection = moveDir * PossessSpeed;
			}
			// 警戒距离外：随机移动家具
			else
			{
				if (GD.Randf() < 0.005f)
					moveDirection = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1) * PossessSpeed; // 水平移动
			}
			return;
		}

		if (_periplanetaStates == PeriplanetaStates.Dead)
		{
			if (isDead) return;
			
			isDead = true;
			// todo: 蟑螂死亡
			SceneTreeTimer timer = GetTree().CreateTimer(1.0f);
			timer.Timeout += () => QueueFree();

		}
	}


	public void OnDamage()
	{
		++currentDamage;

		if (currentDamage >= MaxHealth)
		{
			_periplanetaStates = PeriplanetaStates.Dead;
			PeriplanetaCount -= 1;

			if (_possessFurniture != null)
			{
				_possessFurniture.UpdatePeriInside(null);
				_possessFurniture = null;
			}

			_isInFurniture = false;
			_sprite.Modulate = new Color(1, 1, 1, 1);
			CollisionLayer = 52;
			CollisionMask = 52;
			moveDirection = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1) * DangerSpeed; // todo: 从箱子被打出的位移

			SetRotationToDirection(moveDirection);
			Rotation = _rotation;
			Velocity = moveDirection;

			MoveAndSlide();
			moveDirection = Vector2.Zero;
			Velocity = Vector2.Zero;
			return;
		}

		if (_periplanetaStates == PeriplanetaStates.Outside)
		{
			_remainCD = 0;
			_periplanetaStates = PeriplanetaStates.Entering;
			return;
		}

		if (_periplanetaStates == PeriplanetaStates.Entering)
		{
			return;
		}
		if (_periplanetaStates == PeriplanetaStates.Inside)
		{
			++currentHit;

			if (currentHit >= MaxHit)
			{
				_periplanetaStates = PeriplanetaStates.Outside;
				_possessFurniture.UpdatePeriInside(null);
				_possessFurniture = null;
				_sprite.Modulate = new Color(1, 1, 1, 1);
				_collisionShape.Disabled = false;
				_isInFurniture = false;
				moveDirection = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1) * DangerSpeed; // todo: 从箱子被打出的位移
				_remainCD = CD;
				currentHit = 0;
				SetRotationToDirection(moveDirection);
				Velocity = moveDirection;
				Rotation = _rotation;

				MoveAndSlide();
				moveDirection = Vector2.Zero;
				Velocity = Vector2.Zero;

			}
		}
	}

	#region tools

	private Furniture FindFurnitureInScene(Node node)
	{
		// 检查当前节点是否为 Furniture
		if (node is Furniture furnitureNode)
			return furnitureNode;

		// 递归搜索子节点
		foreach (Node child in node.GetChildren())
		{
			Furniture result = FindFurnitureInScene(child);
			if (result != null)
				return result;
		}
		return null;
	}

	private Furniture FindNearestFreeFurniture(Node rootNode)
	{
		// 获取当前节点的全局位置作为基准点
		Vector2 currentPos = GlobalPosition;
		Furniture nearestFurniture = null;
		float minDistanceSq = float.MaxValue; // 使用平方距离避免开方运算

		// 局部递归函数实现深度优先遍历
		void Search(Node node)
		{
			// 检查当前节点是否为有效家具
			if (node is Furniture furniture &&
				furniture.Status == Furniture_Status.FREE)
			{
				// 计算平方距离（性能优化）
				float distSq = currentPos.DistanceSquaredTo(furniture.GlobalPosition);

				// 更新最近家具记录
				if (distSq < minDistanceSq)
				{
					minDistanceSq = distSq;
					nearestFurniture = furniture;
				}
			}

			// 递归搜索子节点
			foreach (Node child in node.GetChildren())
			{
				Search(child);
			}
		}

		// 从根节点开始搜索
		Search(rootNode);
		return nearestFurniture;
	}

	private List<Player> FindAllPlayers(Node startNode)
	{
		List<Player> players = new List<Player>();
		RecursiveSearch(startNode, players);
		return players;
	}

	private void RecursiveSearch(Node currentNode, List<Player> playerList)
	{
		// 检查当前节点是否为Player
		if (currentNode is Player player)
		{
			playerList.Add(player);
			// 找到两个后提前终止搜索
			if (playerList.Count >= 2) return;
		}

		// 递归遍历子节点
		foreach (Node child in currentNode.GetChildren())
		{
			RecursiveSearch(child, playerList);
			// 找到两个后提前终止
			if (playerList.Count >= 2) return;
		}
	}

	private void SetRotationToDirection(Vector2 dir)
	{
		if (dir == Vector2.Zero) return;
		
		dir = -dir;

		_rotation = dir.Angle() - Mathf.Pi / 2f;
	}

	#endregion

}
