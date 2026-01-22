using Godot;

public partial class MusicManager : Node
{
    static AudioStreamPlayer Level, BreakRoom;
    static AudioStreamPlayer ActiveSong;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Level = GetNode<AudioStreamPlayer>("Level");
        BreakRoom = GetNode<AudioStreamPlayer>("BreakRoom");
        ActiveSong = GetNode<AudioStreamPlayer>("Silence");
        SwitchTrack("Level");
    }

    public static void SwitchTrack(string trackName)
    {
        AudioStreamPlayer newSong = null;
        switch (trackName)
        {
            case "Level":
                newSong = Level;
                break;
            case "BreakRoom":
                newSong = BreakRoom;
                break;
        }

        if (newSong != ActiveSong)
        {
            ActiveSong.Playing = false;
            ActiveSong = newSong;
            ActiveSong.Play();
        }


    }
}
