using System;

namespace NeoStatsPlugin.Core
{
    public class TimeStat
    {
        private TimeSpan _total = TimeSpan.Zero;

        /// <summary>
        /// Min time
        /// </summary>
        public TimeSpan Min { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Max time
        /// </summary>
        public TimeSpan Max { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Avg time
        /// </summary>
        public TimeSpan Avg => TimeSpan.FromMilliseconds(_total.TotalMilliseconds / Count);

        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; set; } = 0;

        /// <summary>
        /// Add time
        /// </summary>
        /// <param name="time">Time</param>
        public void Add(TimeSpan time)
        {
            _total = _total.Add(time);
            Count++;

            Min = Min.TotalMilliseconds == 0 ? time : (Min.TotalMilliseconds < time.TotalMilliseconds ? Min : time);
            Max = Max.TotalMilliseconds == 0 ? time : (Max.TotalMilliseconds > time.TotalMilliseconds ? Max : time);
        }
    }
}