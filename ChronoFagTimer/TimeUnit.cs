/*
 *  Copyright (C) 2017 HarpyWar <harpywar@gmail.com>
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using NLog;

namespace ChronoFagTimer
{
    public class TimeUnit
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TimeUnit(int limit, int extraTime, string title)
        {
            CounterLimit = limit;
            ExtraTime = extraTime; // value from config
            Title = title;
        }
        /// <summary>
        /// Counter limit to go next time
        /// </summary>
        public int CounterLimit
        {
            get
            {
                return ExtraMode 
                    ? ExtraTime
                    : _counterLimit;
            }
            private set
            {
                _counterLimit = value;
            }
        }
        int _counterLimit;

        public int CounterLimitOriginal
        {
            get
            {
                return _counterLimit;
            }
        }

        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Set value from config, actully used only for pomodoro
        /// </summary>
        private int ExtraTime
        {
            get;
            set;
        }

        /// <summary>
        /// Actually extramode can be only set for pomodoro
        /// </summary>
        public bool ExtraMode
        {
            get
            {
                return _extraMode;
            }
            set
            {
                _extraMode = value;
                Logger.Info("Set extramode = {0}", _extraMode);
            }
        }
        bool _extraMode = false;



    }

    public class Break : TimeUnit
    {
        public Break(int limit, int extraTime, string title) : base(limit, extraTime, title)
        {
        }
    }
    public class Pomodoro : TimeUnit
    {
        public Pomodoro(int limit, int extraTime, string title) : base(limit, extraTime, title)
        {
        }
    }

}
