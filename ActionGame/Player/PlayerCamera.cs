using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
    private bool skipFrame = false;

    // This is needed to skip the one frame of the game where there is no camera
    // Otherwise the camera will jerk across the whole stage (from a room's origin)
    public void TurnOnPositionSmoothing()
    {
        skipFrame = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (skipFrame)
        {
            skipFrame = false;
            PositionSmoothingEnabled = true;
        }
    }
}
