using NeoStatsPlugin.Extensions;

namespace NeoStatsPlugin.Core
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

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.ToJson();
    }
}