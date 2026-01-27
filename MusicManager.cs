using System.Collections.Generic;
using Godot;

public partial class MusicManager : Node
{
    static AudioStreamPlayer Level, BreakRoom;
    static AudioStreamPlayer ActiveSong;
    private static Dictionary<string, double> SongTimers = new Dictionary<string, double>();
    private static string ActiveSongName;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SongTimers["Level"] = 0;
        SongTimers["Silence"] = 0;
        SongTimers["BreakRoom"] = 0;
        Level = GetNode<AudioStreamPlayer>("Level");
        BreakRoom = GetNode<AudioStreamPlayer>("BreakRoom");
        ActiveSong = GetNode<AudioStreamPlayer>("Silence");
        SwitchTrack("Level");
    }

    public override void _Process(double delta)
    {
        if (ActiveSongName != "BreakRoom")
        {
            SongTimers[ActiveSongName] += delta;
        }

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
            ActiveSongName = trackName;
            ActiveSong.Stop();
            ActiveSong = newSong;
            ActiveSong.Play((float)SongTimers[ActiveSongName]);
        }


    }
}
