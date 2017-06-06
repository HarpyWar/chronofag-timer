using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCTomatoTime
{
    public class Alert
    {
        /// <summary>
        /// Current elapsed time (do not set from config)
        /// </summary>
        public int Elapsed;
        public void Reset()
        {
            Elapsed = 0;
        }

        /// <summary>
        /// Time remaining in pomodoro (seconds)
        /// </summary>
        public int Remain;
        /// <summary>
        /// Duration of alert show
        /// </summary>
        public int Duration;

        /// <summary>
        /// File to play
        /// </summary>
        public string Sound;
    }
}
