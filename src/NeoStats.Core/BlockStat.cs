using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace NeoStats.Core
{
    public class BlockStat : IDisposable
    {
        Stopwatch _watch;

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
        public uint Timestamp { get; set; } = 0;

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
            _watch = new Stopwatch();
            _watch.Start();

            Index = index;
        }

        /// <summary>
        /// Convert to json
        /// </summary>
        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.None);

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            _watch?.Stop();
            _watch = null;
        }
    }
}