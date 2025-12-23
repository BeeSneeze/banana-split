using Godot;
using System;

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
        Dust
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
        OBSTACLES = 64
    }
}