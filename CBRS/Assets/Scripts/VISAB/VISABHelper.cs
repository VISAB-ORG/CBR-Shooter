using Assets.Scripts.AI;
using Assets.Scripts.Model;
using Assets.Scripts.VISAB.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VISABConnector.Unity;

namespace Assets.Scripts.VISAB
{
    public static class VISABHelper
    {
        /// <summary>
        /// How many times to send statistics per ingame second.
        /// </summary>
        public const int SendPerSecond = 10;

        public static string HostAdress { get; set; } = "http://25.44.85.33";

        public static int Port { get; set; } = 2673;

        public static int RequestTimeout { get; set; } = 15;

        public static VISABStatistics GetCurrentStatistics()
        {
            var gameInformation = GameControllerScript.GameInformation;
            if (gameInformation == null)
            {
                Debug.Log("Gameinformation was null when trying to get current statistics.");
                return null;
            }

            var statistics = new VISABStatistics
            {
                RoundTime = gameInformation.RoundTime,
                AmmunitionPosition = Vector3ToVector2(gameInformation.AmmunitionPosition),
                HealthPosition = Vector3ToVector2(gameInformation.HealthPosition),
                WeaponPosition = Vector3ToVector2(gameInformation.WeaponPosition),
                Round = gameInformation.RoundCounter,
                TotalTime = gameInformation.TotalTime
            };

            foreach (var player in gameInformation.Players)
                statistics.Players.Add(ExtractPlayerInformation(player));

            return statistics;
        }

        public static VISABMetaInformation GetMetaInformation()
        {
            var gameInformation = GameControllerScript.GameInformation;
            if (gameInformation == null)
            {
                Debug.Log("Gameinformation was null when trying to get meta information.");
                return null;
            }


            var metaInformation = new VISABMetaInformation
            {
                GameSpeed = gameInformation.Speed,
                PlayerCount = gameInformation.Players.Count,
                MapRectangle = gameInformation.MapRectangle,
            };

            var playerColors = new Dictionary<string, string>();
            var playerInformation = new Dictionary<string, string>();
            foreach (var player in gameInformation.Players)
            {
                if (player.mIsHumanControlled)
                    playerInformation[player.mName] = ControlledBy.Human;
                else if (player.mCBR)
                    playerInformation[player.mName] = ControlledBy.CBR;
                else
                    playerInformation[player.mName] = ControlledBy.Script;

                if (player.mName == "Jane Doe")
                    playerColors[player.mName] = "#B10DC9";
                else
                    playerColors[player.mName] = "#FF851B";
            }

            metaInformation.PlayerInformation = playerInformation;
            metaInformation.PlayerColors = playerColors;

            var machineGunInfo = GetWeaponInformation(new MachineGun(null));
            var pistolInfo = GetWeaponInformation(new Pistol(null));
            metaInformation.WeaponInformation.Add(machineGunInfo);
            metaInformation.WeaponInformation.Add(pistolInfo);

            return metaInformation;
        }

        private static WeaponInformation GetWeaponInformation(Weapon weapon)
        {
            return new WeaponInformation
            {
                Name = weapon.mName,
                Damage = weapon.mDamage,
                FireRate = weapon.mFireRate,
                MagazineSize = weapon.mMagazineSize,
                MaximumAmmunition = weapon.mMaxAmmu
            };
        }

        private static VISABPlayer ExtractPlayerInformation(Player player)
        {
            var plan = "";
            if (!player.mCBR && BotBehaviourScript.ScriptBotPlan != null)
                plan = BotBehaviourScript.ScriptBotPlan.ToString();
            else if (player.mCBR && player.mPlan != null)
                plan = player.mPlan.actionsAsString;

            return new VISABPlayer
            {
                Health = player.mPlayerHealth,
                RelativeHealth = player.mPlayerHealth / (float)Player.mMaxLife,
                MagazineAmmunition = player.mEquippedWeapon.mCurrentMagazineAmmu,
                TotalAmmunition = player.mEquippedWeapon.mCurrentTotalAmmu,
                Name = player.mName,
                Plan = plan,
                Weapon = player.mEquippedWeapon.mName,
                Statistics = new VISABPlayerStatistics
                {
                    Deaths = player.mStatistics.DeathCount(),
                    Frags = player.mStatistics.FragCount(),
                },
                Position = Vector3ToVector2(player.GetPlayerPosition())
            };
        }

        /// <summary>
        /// Transforms a Unity Vector3 into a System.Numerics.Vector2.
        /// </summary>
        /// <param name="in">The unity vector</param>
        /// <returns>A Vector2 with Vector2.X = in.z, Vector2.Y = in.x</returns>
        private static System.Numerics.Vector2 Vector3ToVector2(Vector3 @in)
        {
            var rotated = Quaternion.Euler(new Vector3(0, 45, 0)) * @in;

            return new System.Numerics.Vector2
            {
                X = rotated.x,
                Y = rotated.z
            };
        }

        public static VISABImageContainer MakeSnapshots()
        {
            Func<string, SnapshotConfiguration> defaultInstantiate = (prefabPath) => new SnapshotConfiguration
            {
                ImageHeight = 1024,
                ImageWidth = 1024,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = prefabPath,
                    SpawnLocation = new Vector3(100, 100, 100),
                },
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 1.5f,
                    Orthographic = false,
                    UseAbsoluteOffset = true,
                    CameraRotation = new Vector3(90, 0, 0)
                }
            };

            Func<string, string, SnapshotConfiguration> defaultChildObjects = (prefabPath, childName) => new SnapshotConfiguration
            {
                ImageHeight = 1024,
                ImageWidth = 1024,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = prefabPath,
                    SpawnLocation = new Vector3(100, 100, 100),
                },
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 1.5f,
                    Orthographic = false,
                    UseAbsoluteOffset = true,
                    CameraRotation = new Vector3(90, 0, 0)
                },
                ChildConfiguration = new ChildConfiguration
                {
                    ChildName = childName
                }

            };

            Func<string, SnapshotConfiguration> defaultExisting = (gameId) => new SnapshotConfiguration
            {
                ImageHeight = 1024,
                ImageWidth = 1024,
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 1f,
                    Orthographic = false,
                    UseAbsoluteOffset = false,
                    CameraRotation = new Vector3(90, 0, 0)
                },
                GameObjectId = gameId
            };

            var spawnablePrefabPaths = new Dictionary<string, string>
            {
                { "WeaponCrate", "Prefabs/WeaponsCrate/WeaponsCrate" },
                { "M4a1", "Prefabs/M4A1_Collectable" },
                { "Health", "Prefabs/Health" }
            };

            // Key contains child name, Value contains prefab path
            var spawnablePrefabPathsWithChild = new Dictionary<string, string>
            {
                { "M4A1_Sopmod_Body", "Prefabs/M4A1_Collectable"},
                { "Player", "Prefabs/Player" }
            };

            var images = new VISABImageContainer();

            foreach (var pair in spawnablePrefabPaths)
            {
                var config = defaultInstantiate(pair.Value);
                var bytes = ImageCreator.TakeSnapshot(config);
                var path = GameControllerScript.SnapshotName(234, 234, pair.Key);

                File.WriteAllBytes(path, bytes);

                images.StaticObjects.Add(pair.Key, bytes);
            }

            foreach (var pair in spawnablePrefabPathsWithChild)
            {
                var config = defaultChildObjects(pair.Value, pair.Key);
                var bytes = ImageCreator.TakeSnapshot(config);
                var path = GameControllerScript.SnapshotName(234, 234, pair.Key);

                File.WriteAllBytes(path, bytes);

                images.StaticObjects.Add(pair.Key, bytes);
            }

            var mapConfig = new SnapshotConfiguration
            {
                ImageHeight = 550,
                ImageWidth = 550,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = "Prefabs/Environment45",
                    SpawnLocation = new Vector3(500, 500, 500)
                },
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 2f,
                    Orthographic = true,
                    CameraRotation = new Vector3(90, 0, 45),
                    OrthographicSize = 75f
                }
            };
            var snapshot = ImageCreator.TakeSnapshot(mapConfig);
            images.Map = snapshot;


            var savepath = GameControllerScript.SnapshotName(1024, 1024, "Map");
            File.WriteAllBytes(savepath, snapshot);

            Debug.Log(JsonConvert.SerializeObject(images));

            return images;
        }

        public static MapRectangle GetMapRectangle()
        {
            // Create a new prefab instance that is pre rotated.
            // We have to do this, since rotating the game object dynamically does not rotate the renderes/colliders.
            // Infact, colliders and renderers can not be rotated at execution time in general.
            var clone = GameObject.Instantiate(Resources.Load("Prefabs/Environment45")) as GameObject;

            var bounds = clone.GetBoundsWithChildren();
            var anchorPoint = new System.Numerics.Vector2 { X = bounds.min.x, Y = bounds.max.z };
            GameObject.Destroy(clone);

            return new MapRectangle
            {
                Height = (int)bounds.size.z,
                Width = (int)bounds.size.x,
                TopLeftAnchorPoint = anchorPoint
            };
        }

        public static Bounds GetBoundsWithChildren(this GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();

            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i])
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }

            return bounds;
        }
    }
}