using System;
using System.Linq;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using NeoStats.Core;

namespace NeoStatsPlugin.Extensions
{
    public static class NeoExtensions
    {
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

            if (firstTime)
            {
                if (block.Hash.ToString() != block.Hash || block.Size != block.Size || block.Timestamp != block.Timestamp)
                {
                    throw new ArgumentException($"Fork on {block.Index}");
                }
            }

            block.Size = block.Size;
            block.Hash = block.Hash.ToString();
            block.Timestamp = block.Timestamp;
            block.Transactions.Update(currentBlock.Transactions);

            block.ElapsedTime = TimeSpan.FromSeconds((previousBlock == null ? 0 : block.Timestamp - previousBlock.Timestamp));

            if (block.Index != 0 && !firstTime)
            {
                // We can't access to Blockchain on GenesisBlock

                block.MemPool.UpdateMemPool();
            }
        }
    }
}