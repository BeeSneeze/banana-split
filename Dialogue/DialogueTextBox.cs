using Common;
using Godot;
using System;

public partial class DialogueTextBox : TextureRect
{

    public string Speaker;
    public string Text;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<RichTextLabel>("RichTextLabel").Text = Text;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
