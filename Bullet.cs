using Godot;
using System;
using Common;

public partial class Bullet : RigidBody2D
{
    private Team team;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (CollisionLayer == 0)
        {
            throw new Exception("Bullet was created without a team!");
        }
    }

    public void SetTeam(Team setAllegiance)
    {

        team = setAllegiance;
        if (team == Team.ENEMY)
        {
            CollisionLayer = 16;
            CollisionMask = 2;
        }
        else if (team == Team.PLAYER)
        {
            CollisionLayer = 4;
            CollisionMask = 8;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
