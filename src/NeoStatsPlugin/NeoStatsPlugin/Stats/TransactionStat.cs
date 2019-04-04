using System.Linq;
using Neo.Network.P2P.Payloads;

namespace NeoStatsPlugin.Stats
{
    public class TransactionStat
    {
        /// <summary>
        /// Count
        /// </summary>
        public int Count => LowPriorty + HighPriorty;

        /// <summary>
        /// Low priority count
        /// </summary>
        public int LowPriorty { get; private set; }

        /// <summary>
        /// High priority count
        /// </summary>
        public int HighPriorty { get; private set; }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="transactions">Transactions</param>
        public void Update(Transaction[] transactions)
        {
            LowPriorty = transactions.Where(u => u.References == null ? true /* Prevent error if we don't have the chain */ : u.IsLowPriority).Count();
            HighPriorty = transactions.Length - LowPriorty;
        }
    }
}