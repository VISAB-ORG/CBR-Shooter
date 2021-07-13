using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Scripts.VISAB.Map;
using UnityEngine;

namespace Assets.Scripts.VISAB.Map
{
    class TakeSnapshot
    {
        public List<Settings> SnapList { get; set; }

        public TakeSnapshot()
        {
            foreach(var element in SnapList)
            {
                if (element.HasToBeInstantiated)
                {

                } else
                {
                   
                }
            }
        }


    }
}
