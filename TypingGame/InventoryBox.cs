using Common;
using Godot;
using System;
using System.Threading.Tasks;

public partial class InventoryBox : TextureRect
{
    public int Damage;
    public DamageType damageType;
    private bool Initiated = false;

    public override void _Ready()
    {
        // Delay by one frame due to being a control node
        GetTree().ProcessFrame += Initialize;
    }

    public void OneLessDamage()
    {
        Damage--;
        GetNode<AnimatedSprite2D>("Number").Animation = Damage.ToString();
    }

    private void Initialize()
    {
        if (!Initiated)
        {
            Initiated = true;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(this, "rotation", 0.2f, 0.1);
            tween.TweenProperty(this, "rotation", 0f, 0.1);
            var tween2 = GetTree().CreateTween();
            Scale = new Vector2(0.1f, 0.1f);
            tween2.TweenProperty(this, "scale", new Vector2(1, 1), 0.11);
            tween2.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.03);
            tween2.TweenProperty(this, "scale", new Vector2(1, 1), 0.02);
            GetNode<AnimatedSprite2D>("Number").Animation = Damage.ToString();

            switch (damageType)
            {
                case DamageType.Scramble:
                    Texture = GD.Load<Texture2D>("res://TypingGame/ScrambleBox.png");
                    break;
                case DamageType.Arrows:
                    Texture = GD.Load<Texture2D>("res://TypingGame/ArrowBox.png");
                    break;
            }
        }

    }

}
