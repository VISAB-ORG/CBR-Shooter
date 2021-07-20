using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VISABConnector;

namespace Assets.Scripts.VISAB.Model
{
    public class VISABImageContainer : IImageContainer
    {
        public IDictionary<string, byte[]> StaticObjects { get; set; } = new Dictionary<string, byte[]>();

        public IDictionary<string, byte[]> MoveableObjects { get; set; } = new Dictionary<string, byte[]>();

        public byte[] Map { get; set; }
    }
}
