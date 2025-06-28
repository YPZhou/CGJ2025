using Godot;

public partial class RoamingOutsideState : PeriplanetaStatus
{
    //public override void Enter() => owner._isInFurniture = false;

    //public override void PhysicsUpdate(double delta)
    //{
    //    float distToPlayer = owner.GlobalPosition.DistanceTo(owner._player1.GlobalPosition);

    //    // 危险距离内：加速逃离
    //    if (distToPlayer < owner.DangerDistence)
    //    {
    //        Vector2 fleeDir = (owner.GlobalPosition - owner._player1.GlobalPosition).Normalized();
    //        owner.Velocity = fleeDir * owner.DangerSpeed;
    //    }
    //    // 警戒距离内：随机游荡（避开玩家方向）
    //    else if (distToPlayer < owner.AlertDistence)
    //    {
    //        // 随机方向游荡（示例：每2秒换方向）
    //        if (GD.Randf() < 0.01f)
    //        {
    //            Vector2 wanderDir = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1).Normalized();
    //            owner.Velocity = wanderDir * owner.BaseSpeed;
    //        }
    //    }
    //    // 警戒距离外：静止
    //    else
    //    {
    //        owner.Velocity = Vector2.Zero;
    //    }

    //    // 检测进入家具（需冷却结束）
    //    if (owner._remainCD <= 0) // && DetectNearbyFurniture())
    //        owner.TransitionTo("Entering");
    //}
}