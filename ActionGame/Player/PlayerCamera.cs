using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
    private bool skipFrame = false;
    private Vector2 OldZoom;

    public override void _Ready()
    {
        CustomEvents.Instance.DialogueStarted += StartDialogue;
        CustomEvents.Instance.DialogueEnded += EndDialogue;
    }

    private void StartDialogue(string _name)
    {
        OldZoom = new Vector2(Zoom.X, Zoom.Y);
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "zoom", new Vector2(1.5f, 1.5f), 0.3);
    }

    private void EndDialogue()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "zoom", OldZoom, 0.3);
    }
}
