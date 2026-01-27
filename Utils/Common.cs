using System.Reflection.Metadata;

namespace Common
{
    public enum Team
    {
        PLAYER,
        ENEMY,
        NEUTRAL
    }

    public enum ParticleNames
    {
        Explosion,
        Dust,
        Splash,
        Sparkle
    }

    public enum MinigameBoxType
    {
        Text,
        Numbers
    }

    public readonly record struct NPCName(string Name, string Emotion)
    {
        public const string JENNY = "Jenny";
        public static NPCName Jenny = new NPCName(JENNY, UNDEFINED);
        public const string RB = "RB";
        public static NPCName Rb = new NPCName(RB, UNDEFINED);
        public const string LEFTY = "Lefty";
        public static NPCName Lefty = new NPCName(LEFTY, UNDEFINED);

        public const string HAPPY = "Happy";
        public const string SAD = "Sad";
        public const string ANGRY = "Angry";
        public const string CONFUSED = "Confused";
        public const string UNDEFINED = "Undefined";
    };

    public readonly record struct Emotion(string Name)
    {

    }

    public enum CollisionLayerDefs : uint
    {
        DEFAULT = 1,
        PLAYER = 2,
        PLAYER_BULLETS = 4,
        ENEMY = 8,
        ENEMY_BULLETS = 16,
        WALLS = 32,
        OBSTACLES = 64,
        ROOM_TELEPORTS = 128,
        PICKUPS = 256,
        DODGEABLE = 512,
    }
}