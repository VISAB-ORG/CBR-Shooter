using Assets.Scripts.AI;
using Assets.Scripts.Model;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VISABConnector;
using static GameControllerScript;

namespace Assets.Scripts.VISAB
{
    public static class VISABHelper
    {
        /// <summary>
        /// The delay inbetween sending statistics to VISAB in miliseconds
        /// </summary>
        public const int UpdateDelay = 100;

        public static VISABStatistics GetCurrentStatistics()
        {
            var gameInformation = GameControllerScript.GameInformation;

            if (gameInformation == null)
                return null;

            return new VISABStatistics
            {
                RoundTime = gameInformation.RoundTime,
                CBRPlayer = ExtractPlayerInformation(gameInformation.CBRPlayer),
                ScriptPlayer = ExtractPlayerInformation(gameInformation.NonCBRPlayer),
                AmmunitionPosition = Vector3ToVector2(gameInformation.AmmunitionPosition),
                HealthPosition = Vector3ToVector2(gameInformation.HealthPosition),
                WeaponPosition = Vector3ToVector2(gameInformation.WeaponPosition),
                Round = gameInformation.RoundCounter
            };
        }

        /// <summary>
        /// Initiates the infinite loop that sends information to the VISAB api. The loop is stopped
        /// if the given cancellationToken is canceled.
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>An awaitable Task</returns>
        public static async Task StartVISABLoop(CancellationToken cancellationToken)
        {
            // Initializes the VISAB transmission session
            Debug.Log("Starting to initalize Session with VISAB WebApi.");
            var session = await VISABConnector.VISABConnector.InitiateSession("CBRShooter");
            if (session == default)
            {
                while (session == default && !cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Couldent initialize VISAB api session!");
                    session = await VISABConnector.VISABConnector.InitiateSession("CBRShooter");
                }
            }
            Debug.Log($"Initialized Session with VISAB WebApi! SessionId:{session.SessionId}");

            while (!cancellationToken.IsCancellationRequested)
            {
                if (GameControllerScript.GameInformation.GameState == GameState.RUNNING)
                {
                    var statistics = GetCurrentStatistics();
                    if (statistics != null)
                    {
                        if (await session.SendStatistics(statistics))
                            Debug.Log($"Send statistics to VISAB! Round:{statistics.Round}, Time: {statistics.RoundTime}");
                        else
                            break;
                    }
                }

                await Task.Delay(UpdateDelay);
            }

            // Close the VISAB api session
            Debug.Log($"Closing VISAB WebApi session! SessionId:{session.SessionId}");
            await session.CloseSession();
            Debug.Log($"Closed session!");
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
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
                RelativeHealth = (float)player.mPlayerHealth / (float)Player.mMaxLife,
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
        /// Transforms a Unity Vector3 struct into a System.Numerics.Vector2 The Unity Vector3 y
        /// coordinate is thrown away, with its z property becoming the new Y.
        /// </summary>
        /// <param name="unityVector">The unity vector</param>
        /// <returns></returns>
        private static System.Numerics.Vector2 Vector3ToVector2(Vector3 unityVector)
        {
            return new System.Numerics.Vector2
            {
                X = unityVector.x,
                Y = unityVector.z
            };
        }
    }
}