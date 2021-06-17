using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.VISAB.Map
{
    public static class MapExtractionHelper
    {
        /// <summary>
        /// Gets the combined bounds of all GameObjects decorated with the given tag
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The combined bounds</returns>
        public static Bounds GetBounds(String tag)
        {
            var bounds = new Bounds();
            foreach (var obj in GameObject.FindGameObjectsWithTag(tag))
                bounds.Encapsulate(GetBounds(obj));

            return bounds;
        }

        /// <summary>
        /// Gets the bounds for the given GameObject
        /// </summary>
        /// <param name="obj">The GameObject</param>
        /// <returns>The bounds of the GameObject</returns>
        public static Bounds GetBounds(GameObject obj)
        {
            var bounds = new Bounds();
            var allRenderes = obj.GetComponentsInChildren<Renderer>();

            foreach (var renderer in allRenderes)
            {
                if (renderer.enabled)
                    bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }
    }
}