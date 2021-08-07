using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VISABConnector;

namespace Assets.Scripts.VISAB.Model
{
    public class MapRectangle
    {
        public Vector2 TopLeftAnchorPoint { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class VISABMetaInformation : IMetaInformation
    {
        public string Game => "CBRShooter";

        public int PlayerCount { get; set; }

        public MapRectangle MapRectangle { get; set; }

        public IDictionary<string, string> PlayerInformation { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> PlayerColors { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The speed at which the game is played.
        /// 1 is normal, 2 is twice and so on.
        /// </summary>
        public float GameSpeed { get; set; }

        /// <summary>
        /// Information on all weapons.
        /// </summary>
        public IList<WeaponInformation> WeaponInformation { get; set; } = new List<WeaponInformation>();

    }

    public class WeaponInformation
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public float FireRate { get; set; }
        public int MagazineSize { get; set; }
        public int MaximumAmmunition { get; set; }
    }
}
