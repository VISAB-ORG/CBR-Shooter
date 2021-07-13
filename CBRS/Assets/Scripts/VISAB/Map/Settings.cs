using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.VISAB.Map
{
    public class Settings
    {
        public string PrefabPath { get; set; }
        public string GameObjectID { get; set; }
        public float CamOffset { get; set; }
        public Vector3 Rotation { get; set; }

        public bool HasToBeInstantiated { get; set; }
        public int SizeWidth { get; set; }
        public int SizeHeight { get; set; }
        public Vector3 SpawnLocation { get; set; }
    }
}
