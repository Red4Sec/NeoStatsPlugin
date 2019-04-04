using Neo.Ledger;

namespace NeoStatsPlugin.Stats
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
        public int Capacity { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MemPoolStat() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="update">Update</param>
        public MemPoolStat(bool update)
        {
            if (update) UpdateMemPool();
        }

        /// <summary>
        /// Update MemPool
        /// </summary>
        public void UpdateMemPool()
        {
            Capacity = Blockchain.Singleton.MemPool.Capacity;
            Count.UnVerified = Blockchain.Singleton.MemPool.UnVerifiedCount;
            Count.Verified = Blockchain.Singleton.MemPool.VerifiedCount;
        }
    }
}