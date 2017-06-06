using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PCTomatoTime
{
    public class TimeUnit
    {
        public TimeUnit(int limit)
        {
            CounterLimit = limit;
        }
        /// <summary>
        /// Counter limit to go next time
        /// </summary>
        public int CounterLimit
        {
            get;
            private set;
        }
    }

    public class Break : TimeUnit
    {
        public Break(int limit) : base(limit)
        {
        }
    }
    public class Pomodoro : TimeUnit
    {
        public Pomodoro(int limit) : base(limit)
        {
        }
    }

}
