using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

// Notes
// Prefabs are only availible in the unity editor, not at runtime

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

        /// <summary>
        /// Takes a snapshot of the given GameObject
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="obj"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D GetSnapshotCenter(GameObjectType objectType, GameObject obj, int width, int height)
        {
            Assert.IsNotNull(obj);
            Assert.IsTrue(width >= 0 && height >= 0);
            
            var layer = obj.layer;
            var camera = SnapshotCamera.MakeSnapshotCamera(layer);
            var objBounds = GetBounds(obj);

            // TODO: Use different camera settings depending on the objectType.
            switch (objectType)
            {
                case GameObjectType.DyanmicMoveable:
                    break;
            }

            return camera.TakeObjectSnapshot(obj, width: width, height: height);
        }

        /// <summary>
        /// Saves a Texture2D at a given path
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task SaveAsync(this Texture2D texture, string filePath, ImageEncoding encoding)
        {
            Assert.IsNotNull(texture);

            byte[] bytes;
            if (encoding == ImageEncoding.PNG)
            {
                bytes = texture.EncodeToPNG();
                if (!filePath.EndsWith(".png"))
                    filePath += ".png";
            }
            else
            {
                bytes = texture.EncodeToJPG();
                if (!filePath.EndsWith(".jpg"))
                    filePath += ".jpg";
            }

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory("path");

            await Task.Run(() => File.WriteAllBytes(filePath, bytes)).ConfigureAwait(false);
        }
    }
}