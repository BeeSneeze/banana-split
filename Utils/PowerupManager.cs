using Common;
using Godot;
using System;
using System.Collections.Generic;
using Powerups;
using System.Linq;

namespace Powerups
{
    public static class Extensions
    {
        public static Powerup ToPowerup(this string str)
        {
            return Powerup.AllPowerups.First(x => x.Name == str);
        }
    }

    // This only defines the names of the powerups. PowerupManager contains the functionality
    public record struct Powerup(string Name, string Description, string FlavorText)
    {
        public override string ToString()
        {
            return Name;
        }

        // Left brain
        public readonly static Powerup PENCIL = new("Pencil", "Word games get slightly easier", "Better than a pen");
        public readonly static Powerup PEN = new("Pen", "Word games get much easier, everything else gets harder", "Better than a pencil");
        public readonly static Powerup MEMO = new("Memo", "Some boxes turn into arrows", "New directives from corporate");
        public readonly static Powerup AGILE = new("Agile dev.", "All work gets easier, take more damage", "If we keep on splitting into smaller tasks, surely it'll get easier!");
        public readonly static Powerup FORM22B = new("Form 22B", "Turn arrow boxes into letters", "Wow this makes a lot less sense now");
        public readonly static Powerup UNPAIDOVERTIME = new("Unpaid overtime", "All work gets slightly harder", "Boss makes a dollar, and I don't even make a dime");

        // Right brain
        public readonly static Powerup BANANA = new("Banana", "Speeds you up", "This item gives you [b]UNLIMITED[/b] potassium!");
        public readonly static Powerup STICK = new("Stick", "Deal extra damage", "It's a really cool looking stick");
        public readonly static Powerup HAIRDRYER = new("Hair Dryer", "[i]Everything[/i] hurts more", "Turn up the heat!");
        public readonly static Powerup FINGERGUN = new("Finger Gun", "Enemies miss more often", "+2 charisma");
        public readonly static Powerup PVCPIPE = new("PVC Pipe", "Way worse aim, but you take less damage", "It's like a giant macaroni!");
        public readonly static Powerup UPPERCASEL = new("Uppercase L", "Increased firing speed, decreased accuracy", "wait why is my keyboard missing two keys");

        public readonly static Powerup[] AllPowerups =
        [PENCIL, PEN, MEMO, AGILE, FORM22B, UNPAIDOVERTIME,
        BANANA, STICK, HAIRDRYER, FINGERGUN, UPPERCASEL, PVCPIPE];
    }
}

public partial class PowerupManager : Node
{
    private List<Powerup> ActivePowerups = new List<Powerup>();
    private Dictionary<Powerup, Action> PowerupFunctions = new Dictionary<Powerup, Action>();

    public override void _Ready()
    {
        CustomEvents.Instance.PowerupCollected += AssignPowerups;

        foreach (var pow in Powerup.AllPowerups)
        {
            PowerupFunctions[pow] = () => GD.Print("Powerup Activated: " + pow.ToString());
        }

        // Define what the powerups should actually *do*
        PowerupFunctions[Powerup.BANANA] += () => GlobalVariables.Player.PLAYER_BONUS_SPEED += 100f;
        PowerupFunctions[Powerup.STICK] += () => GlobalVariables.Player.PLAYER_DAMAGE += 1;
        PowerupFunctions[Powerup.HAIRDRYER] += () => GlobalVariables.Player.PLAYER_DAMAGE += 1;
        PowerupFunctions[Powerup.HAIRDRYER] += () => GlobalVariables.Enemy.ENEMY_BONUS_DAMAGE += 1;
        PowerupFunctions[Powerup.PVCPIPE] += () => GlobalVariables.Enemy.ENEMY_BONUS_DAMAGE -= 1;
        PowerupFunctions[Powerup.PVCPIPE] += () => GlobalVariables.Player.BONUS_BULLET_SPREAD += 0.1f;
    }

    // Activates a given powerup for the player
    private void AssignPowerups(string powerupName)
    {
        var powerup = powerupName.ToPowerup();
        ActivePowerups.Add(powerup);
        PowerupFunctions[powerup]();
    }
}
