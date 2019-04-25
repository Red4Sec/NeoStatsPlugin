using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Neo.IO.Caching;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using NeoStats.Core;

namespace NeoStatsPlugin.Extensions
{
    public static class NeoExtensions
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Sha256
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Data</returns>
        public static byte[] ToSha256(this byte[] data)
        {
            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(data);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="changes">Changes</param>
        /// <returns>Serialized bytes</returns>
        public static byte[] Serialize(this IEnumerable<DataCache<StorageKey, StorageItem>.Trackable> changes)
        {
            using (var stream = new MemoryStream())
            {
                var data = changes.ToArray();

                // count

                stream.Write(BitConverter.GetBytes(data.Length), 0, 4));

                foreach (var item in data)
                {
                    // State

                    stream.WriteByte((byte)item.State);

                    // Script hash

                    stream.Write(item.Key.ScriptHash.ToArray(), 0, item.Key.ScriptHash.Size);

                    // Key

                    stream.Write(item.Key.Key, 0, item.Key.Key.Length);

                    // Value

                    stream.WriteByte((byte)(item.Item.IsConstant ? 1 : 0));
                    stream.Write(item.Item.Value, 0, item.Item.Value.Length);
                }

                return stream.ToArray();
            }
        }

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

            if (!firstTime)
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