using System.Numerics;

namespace Assets.Scripts.VISAB
{
    public class VISABPlayer
    {
        public int Health { get; set; }

        public int MagazineAmmunition { get; set; }
        public int TotalAmmunition { get; set; }

        public string Name { get; set; }

        public string Plan { get; set; }

        /// <summary>
        /// 2D Representation of the players position
        /// </summary>
        public Vector2 Position { get; set; }

        public float RelativeHealth { get; set; }

        public VISAB.VISABPlayerStatistics Statistics { get; set; }

        public string Weapon { get; set; }
    }
}