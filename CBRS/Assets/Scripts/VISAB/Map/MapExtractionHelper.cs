using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.VISAB.Map
{
    public static class MapExtractionHelper
    {
        public static Texture2D GetScreenshotCenter(GameObjectType objectType, GameObject obj, int width, int height, int layer = 0)
        {
            // TODO: Use different camera settings depending on the objectType.

            var objBounds = GetBounds(obj);
            var camera = SnapshotCamera.MakeSnapshotCamera(layer);
            // Set camera options
            
            return camera.TakeObjectSnapshot(obj, width: width, height: height);
        }

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
            var enabledRenderer = allRenderes.FirstOrDefault(x => x.enabled);

            foreach (var renderer in allRenderes) { 
                if (renderer.enabled)
                    bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }
    }
}