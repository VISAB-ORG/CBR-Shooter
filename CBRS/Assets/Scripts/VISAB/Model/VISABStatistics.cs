using System.Collections.Generic;
using System.Numerics;
using VISABConnector;

namespace Assets.Scripts.VISAB
{
    /// <summary>
    /// TODO: Use relative vectors here?
    /// </summary>
    public class VISABStatistics : IVISABStatistics
    {
        /// <summary>
        /// The position of the ammunition item
        /// </summary>
        public Vector2 AmmunitionPosition { get; set; }

        public string Game => "CBRShooter";

        /// <summary>
        /// The position of the health item
        /// </summary>
        public Vector2 HealthPosition { get; set; }

        /// <summary>
        /// Information on all players
        /// </summary>
        public IList<PlayerInformation> Players { get; } = new List<PlayerInformation>();

        /// <summary>
        /// The current round
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// The time in seconds since the round has begun
        /// </summary>
        public float RoundTime { get; set; }

        /// <summary>
        /// The cumulated time in seconds over all rounds
        /// </summary>
        public float TotalTime { get; set; }

        /// <summary>
        /// The speed at which the game is played.
        /// 1 is normal, 2 is twice and so on.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The position of the weapon item
        /// </summary>
        public Vector2 WeaponPosition { get; set; }

        #region optional properties

        public bool IsAmmunitionCollected => AmmunitionPosition == default;

        public bool IsHealthCollected => HealthPosition == default;

        public bool IsWeaponCollected => WeaponPosition == default;

        #endregion optional properties
    }
}