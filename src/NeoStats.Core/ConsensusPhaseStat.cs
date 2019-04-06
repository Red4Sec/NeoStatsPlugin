using NeoStats.Core.Extensions;

namespace NeoStats.Core
{
    public class ConsensusPhaseStat
    {
        /// <summary>
        /// Time
        /// </summary>
        public TimeStat Time { get; set; } = new TimeStat();

        /// <summary>
        /// Messages
        /// </summary>
        public ConsensusPhaseCount Messages { get; set; } = new ConsensusPhaseCount();

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.ToJson();
    }
}