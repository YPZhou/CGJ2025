using Godot;
using System;

public abstract partial class PeriplanetaStatus : Node
{
	protected Periplaneta owner;
	public void Init(Periplaneta ai) => owner = ai;

	public virtual void Enter() { }
	public virtual void Exit() { }
	public virtual void PhysicsUpdate(double delta) { }
}
