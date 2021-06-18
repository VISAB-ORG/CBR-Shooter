using Assets.Scripts.AI;
using Assets.Scripts.Model;
using Assets.Scripts.VISAB.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.VISAB
{
    public static class VISABHelper
    {
        /// <summary>
        /// How many times to send statistics per ingame second.
        /// </summary>
        public const int SendPerSecond = 10;

        public static string HostAdress { get; set; }

        public static int Port { get; set; }

        public static int RequestTimeout { get; set; }

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

            var playerInformation = new Dictionary<string, string>();
            foreach (var player in gameInformation.Players)
            {
                if (player.mIsHumanControlled)
                    playerInformation[player.mName] = ControlledBy.Human;
                else if (player.mCBR)
                    playerInformation[player.mName] = ControlledBy.CBR;
                else
                    playerInformation[player.mName] = ControlledBy.Script;
            }

            return new VISABMetaInformation
            {
                GameSpeed = gameInformation.Speed,
                PlayerCount = gameInformation.Players.Count,
                PlayerInformation = playerInformation,
                MapRectangle = gameInformation.MapRectangle
            };
        }

        private static PlayerInformation ExtractPlayerInformation(Player player)
        {
            var plan = "";
            if (!player.mCBR && BotBehaviourScript.ScriptBotPlan != null)
                plan = BotBehaviourScript.ScriptBotPlan.ToString();
            else if (player.mCBR && player.mPlan != null)
                plan = player.mPlan.actionsAsString;

            return new PlayerInformation
            {
                Health = (uint)player.mPlayerHealth,
                RelativeHealth = player.mPlayerHealth / (float)Player.mMaxLife,
                MagazineAmmunition = (uint)player.mEquippedWeapon.mCurrentMagazineAmmu,
                Name = player.mName,
                Plan = plan,
                Weapon = player.mEquippedWeapon.mName,
                Statistics = new PlayerStatistics
                {
                    Deaths = (uint)player.mStatistics.DeathCount(),
                    Frags = (uint)player.mStatistics.FragCount(),
                },
                Position = Vector3ToVector2(player.GetPlayerPosition())
            };
        }

        /// <summary>
        /// Transforms a Unity Vector3 into a System.Numerics.Vector2. The Unity Vector3 z
        /// coordinate is thrown away.
        /// </summary>
        /// <param name="unityVector">The unity vector</param>
        /// <returns></returns>
        private static System.Numerics.Vector2 Vector3ToVector2(Vector3 unityVector)
        {
            return new System.Numerics.Vector2
            {
                X = unityVector.x,
                Y = unityVector.y
            };
        }
    }
}