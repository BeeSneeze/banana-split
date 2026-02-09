
// Dear god, use this sparingly
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace GlobalVariables
{
    // These are the numbers which powerups etc. should modify
    class Game()
    {
        public const int PLAYER_DAMAGE = 1;
        public const float PLAYER_BONUS_SPEED = 100f;
        public const int ENEMY_BONUS_DAMAGE = 0;
    }

    // These are just for me. Because I'm cool like that
    class Cheat()
    {
        public const bool NEVER_DIE = false;
        public const bool DOORS_ALWAYS_OPEN = false;
    }
}