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

    public enum TextBoxType
    {
        Normal,
        Numbers
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