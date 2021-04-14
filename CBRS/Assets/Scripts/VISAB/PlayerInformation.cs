using System.Numerics;

namespace Assets.Scripts.VISAB
{
    public class PlayerInformation
    {
        public uint Health { get; set; }

        public uint MagazineAmmunition { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// TODO: Enum ggf. besser?
        /// </summary>
        public string Plan { get; set; }

        public Vector3 Position { get; set; }

        public float RelativeHealth { get; set; }

        public VISAB.PlayerStatistics Statistics { get; set; }

        public string Weapon { get; set; }
    }
}