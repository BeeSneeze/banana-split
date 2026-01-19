using Godot;
using System;

public partial class PersonalSpace : Area2D
{
    Enemy ParentEnemy;

    public override void _Ready()
    {
        ParentEnemy = GetParent<Enemy>();
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (var body in GetOverlappingBodies())
        {
            if (body is Enemy otherEnemy)
            {
                if (otherEnemy != ParentEnemy)
                {
                    ParentEnemy.CreatePersonalSpace(otherEnemy);
                }
            }
        }
    }
}
