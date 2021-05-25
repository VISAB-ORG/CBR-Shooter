using System.Numerics;

namespace Assets.Scripts.VISAB
{
    public class PlayerInformation
    {
        public uint Health { get; set; }

        /// <summary>
        /// If the player is controlled by a CBR system
        /// </summary>
        public bool IsCBR { get; set; }

        /// <summary>
        /// If the player is controlled by a human
        /// </summary>
        public bool IsHumanController { get; internal set; }

        public uint MagazineAmmunition { get; set; }

        public string Name { get; set; }

        public string Plan { get; set; }

        /// <summary>
        /// 2D Representation of the players position
        /// </summary>
        public Vector2 Position { get; set; }

        public float RelativeHealth { get; set; }

        public VISAB.PlayerStatistics Statistics { get; set; }

        public string Weapon { get; set; }
    }
}