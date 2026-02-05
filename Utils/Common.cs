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
        Arrows
    }

    public readonly record struct NPCSpeaker(string Name, string Emotion)
    {
        public const string LIZ = "Liz";
        public readonly static NPCSpeaker Liz = new NPCSpeaker(LIZ, UNDEFINED);
        public const string VINCENT = "Vincent";
        public readonly static NPCSpeaker Vincent = new NPCSpeaker(VINCENT, UNDEFINED);
        public const string ELLIE = "Ellie";
        public readonly static NPCSpeaker Ellie = new NPCSpeaker(ELLIE, UNDEFINED);
        public const string ALEX = "Alex";
        public readonly static NPCSpeaker Alex = new NPCSpeaker(ALEX, UNDEFINED);
        public const string RB = "RB";
        public readonly static NPCSpeaker Rb = new NPCSpeaker(RB, UNDEFINED);
        public const string LEFTY = "Lefty";
        public readonly static NPCSpeaker Lefty = new NPCSpeaker(LEFTY, UNDEFINED);

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