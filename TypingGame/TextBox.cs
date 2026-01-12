using Godot;
using System;
using Common;

public partial class TextBox : ColorRect
{
    public string TypeText = "UNINITIALIZED";
    private TextBoxType Type;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<RichTextLabel>("Label").Text = TypeText;
    }

    public void SetHighlight(string highlight)
    {
        GetNode<RichTextLabel>("Label").Text = "[b]" + highlight + "[/b]" + TypeText.Right(highlight.Length);
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
