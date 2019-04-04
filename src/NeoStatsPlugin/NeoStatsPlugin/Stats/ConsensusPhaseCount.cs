namespace NeoStatsPlugin.Stats
{
    public class ConsensusPhaseCount
    {
        /// <summary>
        /// Received
        /// </summary>
        public int Received => Invalid + Valid;

        /// <summary>
        /// Invalid
        /// </summary>
        public int Invalid { get; set; } = 0;

        /// <summary>
        /// Invalid
        /// </summary>
        public int Valid { get; set; } = 0;

        /// <summary>
        /// Received
        /// </summary>
        public int Duplicated { get; set; } = 0;

        /// <summary>
        /// Total CN
        /// </summary>
        public int M { get; set; } = 0;
    }
}