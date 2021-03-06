﻿/*
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
using System;
using NAudio.Wave;
using System.Drawing;
using System.Windows.Forms;

namespace ChronoFagTimer
{
    class Helper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Random rnd = new Random(DateTime.Now.Millisecond);

        public static void PlaySound(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            try
            {
                var reader = new Mp3FileReader(fileName);
                var waveOut = new WaveOut();
                waveOut.Init(reader);
                waveOut.Play();
            }
            catch (Exception e)
            {
                Logger.Error("Could not play sound " + e.Message);
            }
        }

        public static string GetTimeElapsedString(int counter, int limit)
        {
            var time = TimeSpan.FromSeconds(limit - counter);
            var hours = (double)(limit - counter) / 60 / 60;
            var timeStr = hours >= 1
                ? time.ToString(@"hh\:mm\:ss") // show hours if exist
                : time.ToString(@"mm\:ss");
            return timeStr;
        }

        public static int GetRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static Point GetFormSize()
        {
            return new Point
            (
                SystemInformation.VirtualScreen.Width / GetTimersOnScreen(),
                SystemInformation.VirtualScreen.Height / GetTimersOnScreen()
            );
        }
        public static float GetTitleFontSize()
        {
            return GetTimerFontSize() / 3;
        }
        public static float GetDownTitleFontSize()
        {
            return GetTimerFontSize() / 4;
        }
        public static float GetBreakTitleFontSize()
        {
            return GetTitleFontSize() * 2;
        }
        public static float GetTimerFontSize()
        {
            return (float)(Screen.PrimaryScreen.Bounds.Width / GetTimersOnScreen() / 6.4);
        }
        public static float GetBreakTimerFontSize()
        {
            return (float)(Screen.PrimaryScreen.Bounds.Width / GetTimersOnScreen() / 2.5);
        }

        /// <summary>
        /// Max timers on screen
        /// </summary>
        /// <returns></returns>
        public static int GetTimersOnScreen()
        {
            var screenSize = Screen.PrimaryScreen.Bounds;
            var k = (int)Math.Ceiling((double)screenSize.Height / screenDivider);
            return k;
        }

        /// <summary>
        /// 200 px is good vertical size of timer
        /// </summary>
        const int screenDivider = 200;
    }
}
