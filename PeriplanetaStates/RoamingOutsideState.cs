using Godot;

public partial class RoamingOutsideState : PeriplanetaStatus
{
    //public override void Enter() => owner._isInFurniture = false;

    //public override void PhysicsUpdate(double delta)
    //{
    //    float distToPlayer = owner.GlobalPosition.DistanceTo(owner._player1.GlobalPosition);

    //    // Σ�վ����ڣ���������
    //    if (distToPlayer < owner.DangerDistence)
    //    {
    //        Vector2 fleeDir = (owner.GlobalPosition - owner._player1.GlobalPosition).Normalized();
    //        owner.Velocity = fleeDir * owner.DangerSpeed;
    //    }
    //    // ��������ڣ�����ε����ܿ���ҷ���
    //    else if (distToPlayer < owner.AlertDistence)
    //    {
    //        // ��������ε���ʾ����ÿ2�뻻����
    //        if (GD.Randf() < 0.01f)
    //        {
    //            Vector2 wanderDir = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1).Normalized();
    //            owner.Velocity = wanderDir * owner.BaseSpeed;
    //        }
    //    }
    //    // ��������⣺��ֹ
    //    else
    //    {
    //        owner.Velocity = Vector2.Zero;
    //    }

    //    // ������Ҿߣ�����ȴ������
    //    if (owner._remainCD <= 0) // && DetectNearbyFurniture())
    //        owner.TransitionTo("Entering");
    //}
}