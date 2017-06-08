using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PCTomatoTime
{
    public class TimeUnit
    {
        public TimeUnit(int limit, string title)
        {
            CounterLimit = limit;
            Title = title;
        }
        /// <summary>
        /// Counter limit to go next time
        /// </summary>
        public int CounterLimit
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }
    }

    public class Break : TimeUnit
    {
        public Break(int limit, string title) : base(limit, title)
        {
        }
    }
    public class Pomodoro : TimeUnit
    {
        public Pomodoro(int limit, string title) : base(limit, title)
        {
        }
    }

}
