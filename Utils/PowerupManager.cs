using Common;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public partial class PowerupManager : Node
{
    private List<Powerup> ActivePowerups = new List<Powerup>();
    private Dictionary<Powerup, Action> PowerupFunctions = new Dictionary<Powerup, Action>();

    public override void _Ready()
    {
        DefinePowerups();
        CustomEvents.Instance.PowerupCollected += AssignPowerups;
    }

    private void DefinePowerups()
    {
        PowerupFunctions[Powerup.BANANA] = () => GlobalVariables.Game.PLAYER_BONUS_SPEED += 100f;
        PowerupFunctions[Powerup.STICK] = () => GlobalVariables.Game.PLAYER_DAMAGE += 1;
    }

    private void AssignPowerups(string powerupName)
    {
        var powerup = powerupName.ToPowerup();
        ActivePowerups.Add(powerup);
        PowerupFunctions[powerup]();
        GD.Print("Powerup Activated: " + powerup.Name);
    }
}
