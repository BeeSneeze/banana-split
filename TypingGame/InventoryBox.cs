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
        // Delay by one frame
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
            var tween2 = GetTree().CreateTween();
            Scale = new Vector2(0.1f, 0.1f);
            PivotOffset = new Vector2(100, 100);
            tween.TweenProperty(this, "pivot_offset", new Vector2(0, 0), 0.11);
            tween2.TweenProperty(this, "scale", new Vector2(1, 1), 0.11);
            tween2.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.03);
            tween2.TweenProperty(this, "scale", new Vector2(1, 1), 0.02);
            GetNode<AnimatedSprite2D>("Number").Animation = Damage.ToString();

            if (damageType == DamageType.Scramble)
            {
                Texture = GD.Load<Texture2D>("res://TypingGame/ScrambleBox.png");
            }
        }

    }

}
