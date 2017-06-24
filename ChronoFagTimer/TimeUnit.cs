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

namespace ChronoFagTimer
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
