using System.Numerics;

namespace Assets.Scripts.VISAB
{
    public class PlayerInformation
    {
        public uint Health { get; set; }

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