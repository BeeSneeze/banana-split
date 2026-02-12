
// Dear god, use this sparingly
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace GlobalVariables
{
    // These are the numbers which powerups etc. should modify
    class Game()
    {
        public static int PLAYER_DAMAGE = 1;
        public static float PLAYER_BONUS_SPEED = 400f;
        public static int ENEMY_BONUS_DAMAGE = 0;
    }

    // These are the values that the settings menu should adjust
    class Settings()
    {
        public static bool MUSIC_MUTED = false;
    }

    // These are just for me. Because I'm cool like that
    class Cheat()
    {
        public const bool NEVER_DIE = true;
        public const bool DOORS_ALWAYS_OPEN = true;
    }


}