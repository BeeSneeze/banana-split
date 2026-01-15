using Godot;
using System;
using Common;

public partial class TextBox : ColorRect
{
    public string TypeText = "UNINITIALIZED";
    private TextBoxType Type;
    private const double BOX_SHAKE_TIME = 0.3;
    private double BoxShakeCountdown;
    private Vector2 BasePosition;
    private PackedScene Particle;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Particle = GD.Load<PackedScene>("res://ActionGame/Particles/particle.tscn");
        GetNode<RichTextLabel>("Label").Text = TypeText;
    }

    public void SetHighlight(string highlight)
    {
        GetNode<RichTextLabel>("Label").Text = "[b]" + highlight + "[/b]" + TypeText.Right(highlight.Length);
    }

    public void WrongAnswer()
    {
        if (BoxShakeCountdown <= 0)
        {
            BasePosition = Position;
        }
        Modulate = new Color(1, 0.5f, 0.5f);
        BoxShakeCountdown = BOX_SHAKE_TIME;
    }

    public void RightAnswer()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(0.98f, 0.98f), 0.1);
        tween.Finished += () => Scale = new Vector2(1, 1);
    }

    public void CompleteAnswer()
    {
        GetNode<RichTextLabel>("Label").Text = "[b]" + TypeText + "[/b]";
        var finishParticle = Particle.Instantiate<Particle>();
        finishParticle.Animation = "Sparkle";
        finishParticle.Position = Position + new Vector2(Size.X, Size.Y) * 0.5f;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 0.2f, 0.0f), 0.1);
        var tween2 = GetTree().CreateTween();
        tween2.TweenProperty(this, "scale", new Vector2(1.05f, 1.05f), 0.1);
        tween.Finished += () => QueueFree();
        AddSibling(finishParticle);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (BoxShakeCountdown > 0)
        {
            Position = BasePosition + new Vector2(GD.RandRange(-7, 7), GD.RandRange(-7, 7));
            BoxShakeCountdown -= delta;
            if (BoxShakeCountdown <= 0)
            {
                Position = BasePosition;
                Modulate = new Color(1, 1, 1);
            }
        }
    }


    public void Initialize(string textName, TextBoxType type)
    {
        TypeText = textName.ToUpper();
        Type = type;
    }

    public void SetIntensity(float intensity)
    {
        Modulate = new Color(1, 1, 1, intensity);
        GetNode<RichTextLabel>("Label").Scale = new Vector2(intensity, intensity);
    }
}
