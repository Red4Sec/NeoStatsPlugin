namespace NeoStats.Core
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
        public int LowPriorty { get; set; } = 0;

        /// <summary>
        /// High priority count
        /// </summary>
        public int HighPriorty { get; set; } = 0;
    }
}