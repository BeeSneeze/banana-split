using Common;
using Godot;
using System;

public partial class DialogueTextBox : TextureRect
{
    public NPCName Speaker;
    public string Text;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<RichTextLabel>("RichTextLabel").Text = Text;
    }

}
