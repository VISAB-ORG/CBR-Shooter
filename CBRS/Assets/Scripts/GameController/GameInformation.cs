using Assets.Scripts.Model;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static GameControllerScript;

/// <summary>
/// Class keeping information of the current state of the game
/// </summary>
public class GameInformation
{
    public IList<Player> Players { get; set; } = new List<Player>();

    public Assets.Scripts.VISAB.Model.Rectangle MapRectangle { get; set; }

    public float Speed { get; set; }

    /// <summary>
    /// Get the time since the round has started in seconds
    /// </summary>
    public float RoundTime { get; set; }

    /// <summary>
    /// Coordinates of spawned Ammunition Default if none on map
    /// </summary>
    public Vector3 AmmunitionPosition { get; set; }

    /// <summary>
    /// The state of the game
    /// </summary>
    public GameState GameState { get; set; }

    /// <summary>
    /// Coordinates of spawned Health items Default if none on map
    /// </summary>
    public Vector3 HealthPosition { get; set; }

    public bool IsAmmunitionCollected => AmmunitionPosition == default;

    public bool IsHealthCollected => HealthPosition == default;

    public bool IsWeaponCollected => WeaponPosition == default;

    /// <summary>
    /// Dont know yet
    /// </summary>
    public int RoundCounter { get; set; }

    /// <summary>
    /// Coordinates of spawned Weapon Default if none on map
    /// </summary>
    public Vector3 WeaponPosition { get; set; }

    public float TotalTime { get; set; }
}