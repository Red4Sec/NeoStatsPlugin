namespace NeoStats.Core
{
    public class MemPoolStat
    {
        /// <summary>
        /// Count
        /// </summary>
        public MemPoolCountStat Count { get; } = new MemPoolCountStat();

        /// <summary>
        /// Capacity
        /// </summary>
        public int Capacity { get; set; } = 0;
    }
}