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
        /// <summary>
        /// Method that contains configurations for the game objects that need to be snapshotted
        /// </summary>
        /// <returns></returns>
        public static VISABImageContainer MakeSnapshots()
        {
            // default function for configuring game objects that need to be instantiated first
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
                    UseAbsoluteOffset = false,
                    CameraRotation = new Vector3(90, 0, 0)
                }
            };

            // default function for configuring game objects that already exist
            Func<string, SnapshotConfiguration> defaultExisting = (gameObj) => new SnapshotConfiguration
            {
                ImageHeight = 1024,
                ImageWidth = 1024,
                InstantiationSettings = new InstantiationConfiguration
                {
                    SpawnLocation = new Vector3(100, 100, 100),
                },
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 1.5f,
                    Orthographic = false,
                    UseAbsoluteOffset = false,
                    CameraRotation = new Vector3(90, 0, 0)
                },
                GameObjectId = gameObj
            };

            // default function for configuring game objects that need to be instantiated first and only one child object needs to be snapped
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
                    UseAbsoluteOffset = false,
                    CameraRotation = new Vector3(90, 0, 0)
                },
                ChildConfiguration = new ChildConfiguration
                {
                    ChildName = childName
                }
            };

            // dict for game objects that need to be instantiated first
            var spawnablePrefabPaths = new Dictionary<string, string>
            {
                { "WeaponCrate", "Prefabs/WeaponsCrate/WeaponsCrate" },
                { "Health", "Prefabs/Health" }
            };

            // Key contains child name, Value contains prefab path
            var spawnablePrefabPathsWithChild = new Dictionary<string, string>
            {
                { "Player", "Prefabs/Player" }
            };

            // image container that contains snapped images and will get sent to VISAB
            var images = new VISABImageContainer();

            // snapshot and add spawnable objects to image container
            foreach (var pair in spawnablePrefabPaths)
            {
                var config = defaultInstantiate(pair.Value);
                var bytes = ImageCreator.TakeSnapshot(config);
                var path = GameControllerScript.SnapshotName(234, 234, pair.Key);

#if UNITY_EDITOR
                File.WriteAllBytes(path, bytes);
#endif
                images.StaticObjects.Add(pair.Key, bytes);
            }

            // snapshot and add spawnable gameobj with children to image container
            foreach (var pair in spawnablePrefabPathsWithChild)
            {
                var config = defaultChildObjects(pair.Value, pair.Key);
                var bytes = ImageCreator.TakeSnapshot(config);
                var path = GameControllerScript.SnapshotName(234, 234, pair.Key);
#if UNITY_EDITOR
                File.WriteAllBytes(path, bytes);
#endif
                images.MoveableObjects.Add(pair.Key, bytes);

            }

            // seperate configuration for M4 GameObject 
            var M4Config = new SnapshotConfiguration
            {
                ImageHeight = 1024,
                ImageWidth = 1024,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = "Prefabs/M4A1_Collectable",
                    SpawnLocation = new Vector3(100, 100, 100),
                    // adjust rotation so it can be snapped from the side
                    SpawnRotation = new Vector3(0, 90, 90)
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
                    ChildName = "M4A1_Sopmod_Body",
                    // snap all children objects, not only the weapon body
                    SnapAllChilds = true

                }
            };

            // snapshot and add m4 to image container
            var m4snapshot = ImageCreator.TakeSnapshot(M4Config);
            // string has to be "M4a1" so that VISAB recognizes correct image
            images.StaticObjects.Add("M4a1", m4snapshot);

            var m4path = GameControllerScript.SnapshotName(1024, 1024, "M4a1");
#if UNITY_EDITOR
            File.WriteAllBytes(m4path, m4snapshot);
#endif
            // config for map
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
            // snapshot and add map to image container
            var snapshot = ImageCreator.TakeSnapshot(mapConfig);
            images.Map = snapshot;

            var savepath = GameControllerScript.SnapshotName(1024, 1024, "Map");
#if UNITY_EDITOR
            File.WriteAllBytes(savepath, snapshot);
#endif
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