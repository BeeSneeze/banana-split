using System.Runtime.InteropServices.Marshalling;
using Godot;

public partial class Enemy : CharacterBody2D
{
    public Room Room;
    public ActionGame Level;
    public int Knockbackframes = 0;
    protected int HealthPoints;

    public override void _Ready()
    {
        // TODO: Fix references to room and level
        Room = GetParent().GetParent<Room>();
        Level = Room.GetParent<ActionGame>();
    }

    public void TakeHit(Vector2 incomingVelocity)
    {
        Velocity += incomingVelocity * 25;
        Knockbackframes = 10;
        HealthPoints--;
        if (HealthPoints < 1)
        {
            GD.Print("GRUNT DIED!");
            QueueFree();
        }
    }
}
