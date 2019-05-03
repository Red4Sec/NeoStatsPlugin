using System;
using System.Collections.Generic;
using NeoStatsPlugin.Extensions;
using Newtonsoft.Json;

namespace NeoStatsPlugin.Core
{
    public class BlockStat
    {
        /// <summary>
        /// Block index
        /// </summary>
        public uint Index { get; set; } = 0;

        /// <summary>
        /// Block hash
        /// </summary>
        public string Hash { get; set; } = "";

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Size
        /// </summary>
        public long Size { get; set; } = 0;

        /// <summary>
        /// Time between blocks
        /// </summary>
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Mem pool stats
        /// </summary>
        public MemPoolStat MemPool { get; } = new MemPoolStat();

        /// <summary>
        /// Transactions stats
        /// </summary>
        public TransactionStat Transactions { get; } = new TransactionStat();

        /// <summary>
        /// P2P stats
        /// </summary>
        public P2PStat P2P { get; } = new P2PStat();

        /// <summary>
        /// Storage Hash
        /// </summary>
        public string StorageHash { get; set; } = "";

        /// <summary>
        /// Consensus phases
        /// </summary>
        [JsonIgnore]
        public Dictionary<byte, ConsensusPhaseStat> Consensus { get; set; } = new Dictionary<byte, ConsensusPhaseStat>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        public BlockStat(uint index)
        {
            Index = index;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.ToJson();
    }
}