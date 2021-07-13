using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Scripts.VISAB.Map;
using UnityEngine;

namespace Assets.Scripts.VISAB.Map
{
    static class SnapshotAPI
    {
        public static List<Settings> SnapList { get; set; }

        private static SnapshotMethods API = new SnapshotMethods();

        public static Dictionary<Settings, byte[]> TakeSnapshot()
        {
            Dictionary<Settings, byte[]> imageContainer = new Dictionary<Settings, byte[]>();

            foreach (var element in SnapList)
            {
                if (element.HasToBeInstantiated)
                {
                    API.SnapInstantiatedObj(element);
                } else
                {
                    API.SnapExistingObj(element);
                }
            }

            return imageContainer; 
        }


    }
}
