namespace Assets.Scripts.VISAB
{
    public class PlayerStatistics
    {
        /// <summary>
        /// How often the player has died
        /// </summary>
        public uint Deaths { get; set; }

        /// <summary>
        /// How many other players were killed by the player
        /// </summary>
        public uint Frags { get; set; }
    }
}