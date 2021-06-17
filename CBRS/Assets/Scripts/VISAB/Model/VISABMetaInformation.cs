using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VISABConnector;

namespace Assets.Scripts.VISAB.Model
{
    public class VISABMetaInformation : IMetaInformation
    {
        public string Game => "CBRShooter";

        public int PlayerCount { get; set; }

        public Rectangle MapRectangle { get; set; }

        public IDictionary<string, string> PlayerInformation { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The speed at which the game is played.
        /// 1 is normal, 2 is twice and so on.
        /// </summary>
        public float GameSpeed { get; set; }

    }
}
