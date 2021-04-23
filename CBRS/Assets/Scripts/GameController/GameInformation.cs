using Assets.Scripts.Model;
using UnityEngine;
using static GameControllerScript;

/// <summary>
/// Class keeping information of the current state of the game
/// TODO: Add relative Coordinate properties?
/// There can only be one of each Item on the map
/// </summary>
public class GameInformation
{
    /// <summary>
    /// Get the time since the round has started in seconds
    /// </summary>
    public float RoundTime { get; set; }

    /// <summary>
    /// Coordinates of spawned Ammunition Default if none on map
    /// </summary>
    public Vector3 AmmunitionPosition { get; set; }

    /// <summary>
    /// The player object controlled by the CBR System
    /// </summary>
    public Player CBRPlayer { get; set; }

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
    /// The player object controlled by script or human
    /// </summary>
    public Player NonCBRPlayer { get; set; }

    /// <summary>
    /// Dont know yet
    /// </summary>
    public int RoundCounter { get; set; }

    /// <summary>
    /// Coordinates of spawned Weapon Default if none on map
    /// </summary>
    public Vector3 WeaponPosition { get; set; }
}