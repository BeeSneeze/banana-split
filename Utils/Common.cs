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

    public enum DamageType
    {
        Text,
        Scramble,
        Numbers
    }

    public readonly record struct NPCSpeaker(string Name, string Emotion)
    {
        public const string LIZ = "Liz";
        public static NPCSpeaker Liz = new NPCSpeaker(LIZ, UNDEFINED);
        public const string RB = "RB";
        public static NPCSpeaker Rb = new NPCSpeaker(RB, UNDEFINED);
        public const string LEFTY = "Lefty";
        public static NPCSpeaker Lefty = new NPCSpeaker(LEFTY, UNDEFINED);

        public const string HAPPY = "Happy";
        public const string SAD = "Sad";
        public const string ANGRY = "Angry";
        public const string CONFUSED = "Confused";
        public const string UNDEFINED = "Undefined";
    };

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