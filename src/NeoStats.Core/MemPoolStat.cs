using NeoStats.Core.Extensions;

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

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.ToJson();
    }
}