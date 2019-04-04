namespace NeoStatsPlugin.Stats
{
    public class MemPoolCountStat
    {
        /// <summary>
        /// UnVerified Count
        /// </summary>
        public int Total => UnVerified + Verified;

        /// <summary>
        /// UnVerified Count
        /// </summary>
        public int UnVerified { get; set; }

        /// <summary>
        /// Verified Count
        /// </summary>
        public int Verified { get; set; }
    }
}