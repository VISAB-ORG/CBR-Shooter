﻿using Assets.Scripts.AI;
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
        /// How many times to send statistics per ingame second.
        /// </summary>
        public const int SendPerSecond = 10;

        public static string HostAdress { get; set; }

        public static int Port { get; set; }

        public static int RequestTimeout { get; set; }

        public static VISABStatistics GetCurrentStatistics(GameInformation gameInformation)
        {
            if (gameInformation == null)
                return null;

            var statistics = new VISABStatistics
            {
                RoundTime = gameInformation.RoundTime,
                AmmunitionPosition = Vector3ToVector2(gameInformation.AmmunitionPosition),
                HealthPosition = Vector3ToVector2(gameInformation.HealthPosition),
                WeaponPosition = Vector3ToVector2(gameInformation.WeaponPosition),
                Round = gameInformation.RoundCounter,
                TotalTime = gameInformation.TotalTime,
                Speed = gameInformation.Speed
            };

            statistics.Players.Add(ExtractPlayerInformation(gameInformation.CBRPlayer));
            statistics.Players.Add(ExtractPlayerInformation(gameInformation.NonCBRPlayer));

            return statistics;
        }

        public static async Task<IVISABSession> InitiateSession()
        {
            Debug.Log($"Instantiating VISABApi with HostAdress: {HostAdress}, Port: {Port}, RequestTimeout: {RequestTimeout}");
            var visabApi = new VISABApi(HostAdress, Port, RequestTimeout);

            Debug.Log("Starting to initiate Session with VISAB WebApi.");
            // Initializes the VISAB transmission session
            var session = await TryInitiateSession(visabApi).ConfigureAwait(false);
            if (session == null)
            {
                // TODO: Start VISAB
                // VISABApi.StartVISAB("");
                session = await TryInitiateSession(visabApi).ConfigureAwait(false);
            }

            // If session is still null, return
            if (session == null)
                Debug.Log("Failed to initiate session twice, giving up on connecting to visab.");

            return session;
        }

        /// <summary>
        /// Initiates the infinite loop that sends information to the VISAB api. The loop is stopped
        /// if the given cancellationToken is canceled.
        /// </summary>
        /// <param name="session">The session from which to send data</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>An awaitable Task</returns>
        public static async Task StartVISABLoop(IVISABSession session, CancellationToken cancellationToken)
        {
            if (session == null)
                return;

            while (!cancellationToken.IsCancellationRequested)
            {
                var gameInformation = GameControllerScript.GameInformation;
                if (gameInformation?.GameState == GameState.RUNNING)
                {
                    var statistics = GetCurrentStatistics(gameInformation);
                    if (statistics != null)
                    {
                        var response = await session.SendStatistics(statistics);
                        if (response.IsSuccess)
                        {
                            Debug.Log($"Send statistics to VISAB! Round:{statistics.Round}, Time: {statistics.RoundTime}");
                        }
                        else
                        {
                            Debug.Log("Failed to send statistics to VISAB. Breaking out of loop!");
                            break;
                        }
                    }

                    var delay = Mathf.FloorToInt((1000 / SendPerSecond) / gameInformation.Speed);
                    await Task.Delay(delay);
                }

                await Task.Delay(20);
            }


            // Close the VISAB api session
            Debug.Log($"Closing VISAB WebApi session! SessionId:{session.SessionId}");
            await session.CloseSession();
            Debug.Log($"Closed session!");
        }

        private static async Task<IVISABSession> TryInitiateSession(VISABApi api)
        {
            var response = await api.InitiateSession("CBRShooter").ConfigureAwait(false);
            if (!response.IsSuccess)
            {
                Debug.Log("Couldent initialize VISAB api session! Reason:\n");
                Debug.Log(response.ErrorMessage);

                return null;
            }
            else
            {
                Debug.Log($"Initialized Session with VISAB WebApi! SessionId given:{response.Content.SessionId}");
                return response.Content;
            }
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
                IsCBR = player.mCBR,
                IsHumanController = player.mIsHumanControlled,
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