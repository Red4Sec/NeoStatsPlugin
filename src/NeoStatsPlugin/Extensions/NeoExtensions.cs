using System;
using System.Linq;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using NeoStats.Core;

namespace NeoStatsPlugin.Extensions
{
    public static class NeoExtensions
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Update MemPool
        /// </summary>
        /// <param name="memPool">Mem pool</param>
        public static void UpdateMemPool(this MemPoolStat memPool)
        {
            memPool.Capacity = Blockchain.Singleton.MemPool.Capacity;
            memPool.Count.UnVerified = Blockchain.Singleton.MemPool.UnVerifiedCount;
            memPool.Count.Verified = Blockchain.Singleton.MemPool.VerifiedCount;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="stat">Stat</param>
        /// <param name="transactions">Transactions</param>
        public static void Update(this TransactionStat stat, Transaction[] transactions)
        {
            stat.LowPriorty = transactions.Where(u => u.References == null ? true /* Prevent error if we don't have the chain */ : u.IsLowPriority).Count();
            stat.HighPriorty = transactions.Length - stat.LowPriorty;
        }

        /// <summary>
        /// Update block information
        /// </summary>
        /// <param name="block">Block</param>
        /// <param name="previousBlock">Previous block</param>
        public static void UpdateBlockInfo(this BlockStat block, Block currentBlock, BlockStat previousBlock)
        {
            bool firstTime = block.Size == 0;
            var time = unixEpoch.AddSeconds(currentBlock.Timestamp);

            if (firstTime)
            {
                if (block.Hash.ToString() != currentBlock.Hash.ToString() || block.Size != currentBlock.Size || block.Timestamp != time)
                {
                    throw new ArgumentException($"Fork on {block.Index}");
                }
            }

            block.Size = currentBlock.Size;
            block.Hash = currentBlock.Hash.ToString();
            block.Timestamp = time;
            block.Transactions.Update(currentBlock.Transactions);

            block.ElapsedTime = (previousBlock == null ? TimeSpan.Zero : time - previousBlock.Timestamp);

            if (block.Index != 0 && !firstTime)
            {
                // We can't access to Blockchain on GenesisBlock

                block.MemPool.UpdateMemPool();
            }
        }
    }
}