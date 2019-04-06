using System;
using System.Collections.Generic;
using System.Diagnostics;
using Neo.Consensus;
using Neo.Network.P2P.Payloads;
using Newtonsoft.Json;

namespace NeoStatsPlugin.Stats
{
    public class BlockStat : IDisposable
    {
        Stopwatch _watch;
        bool _updatedBlock;

        /// <summary>
        /// Block index
        /// </summary>
        public uint Index { get; private set; } = 0;

        /// <summary>
        /// Block hash
        /// </summary>
        public string Hash { get; private set; } = "";

        /// <summary>
        /// Timestamp
        /// </summary>
        public uint Timestamp { get; private set; } = 0;

        /// <summary>
        /// Size
        /// </summary>
        public long Size { get; private set; } = 0;

        /// <summary>
        /// Time between blocks
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Mem pool stats
        /// </summary>
        public MemPoolStat MemPool { get; }

        /// <summary>
        /// Transactions stats
        /// </summary>
        public TransactionStat Transactions { get; }

        /// <summary>
        /// Consensus phases
        /// </summary>
        [JsonIgnore]
        public Dictionary<ConsensusMessageType, ConsensusPhaseStat> Consensus { get; private set; } = new Dictionary<ConsensusMessageType, ConsensusPhaseStat>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        public BlockStat(uint index)
        {
            _updatedBlock = false;
            _watch = new Stopwatch();
            _watch.Start();

            MemPool = new MemPoolStat();
            Transactions = new TransactionStat();

            Index = index;
        }

        /// <summary>
        /// Update block information
        /// </summary>
        /// <param name="block">Block</param>
        /// <param name="previousBlock">Previous block</param>
        public void UpdateBlockInfo(Block block, BlockStat previousBlock)
        {
            if (_updatedBlock)
            {
                if (block.Hash.ToString() != Hash || Size != block.Size || Timestamp != block.Timestamp)
                {
                    throw new ArgumentException($"Fork on {block.Index}");
                }
            }

            Size = block.Size;
            Hash = block.Hash.ToString();
            Timestamp = block.Timestamp;
            Transactions.Update(block.Transactions);

            ElapsedTime = TimeSpan.FromSeconds((previousBlock == null ? 0 : block.Timestamp - previousBlock.Timestamp));

            if (Index != 0 && !_updatedBlock)
            {
                // We can't access to Blockchain on GenesisBlock

                MemPool.UpdateMemPool();
            }

            _updatedBlock = true;
        }

        #region Consensus

        /// <summary>
        /// On ChangeView
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <param name="message">Message</param>
        public void OnChangeViewReceived(ConsensusPayload payload, ChangeView message) { }

        /// <summary>
        /// On Prepare request
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <param name="message">Message</param>
        public void OnPrepareRequestReceived(ConsensusPayload payload, PrepareRequest message) { }

        /// <summary>
        /// On Prepare response
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <param name="message">Message</param>
        public void OnPrepareResponseReceived(ConsensusPayload payload, PrepareResponse message) { }

        /// <summary>
        /// On Commit
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <param name="message">Message</param>
        public void OnCommitReceived(ConsensusPayload payload, Commit message) { }

        /// <summary>
        /// On Recovery Message
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <param name="message">Message</param>
        public void OnRecoveryMessageReceived(ConsensusPayload payload, RecoveryMessage message) { }

        #endregion

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