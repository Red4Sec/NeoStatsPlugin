namespace NeoStats.Core
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
        public int UnVerified { get; set; } = 0;

        /// <summary>
        /// Verified Count
        /// </summary>
        public int Verified { get; set; } = 0;
    }
}