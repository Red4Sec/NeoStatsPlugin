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
    }
}