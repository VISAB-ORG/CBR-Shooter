using System.Numerics;

namespace Assets.Scripts.VISAB
{
    /// <summary>
    /// TODO: Use relative vectors here?
    /// </summary>
    public class VISABStatistics
    {
        /// <summary>
        /// The position of the ammunition item
        /// </summary>
        public Vector3 AmmunitionPosition { get; set; }

        /// <summary>
        /// Information on the player object controlled by the CBR-system
        /// </summary>
        public VISAB.PlayerInformation CBRPlayer { get; set; }

        /// <summary>
        /// The position of the health item
        /// </summary>
        public Vector3 HealthPosition { get; set; }

        /// <summary>
        /// The current round
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// Information on the player object controlled by the script bot
        /// </summary>
        public VISAB.PlayerInformation ScriptPlayer { get; set; }

        /// <summary>
        /// The position of the weapon item
        /// </summary>
        public Vector3 WeaponPosition { get; set; }

        #region optional properties

        public bool IsAmmunitionCollected => AmmunitionPosition == default;

        public bool IsHealthCollected => HealthPosition == default;

        public bool IsWeaponCollected => WeaponPosition == default;

        #endregion optional properties
    }
}