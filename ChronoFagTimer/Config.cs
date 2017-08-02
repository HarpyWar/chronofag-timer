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

using Hjson;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace ChronoFagTimer
{
    public class Config
    {
        public string ApplicationName
        {
            get
            {
                var attributes = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), 
                    typeof(AssemblyTitleAttribute), false);
                return attributes?.Title;
            }
        }
        public string ApplicationDescription
        {
            get
            {
                var attributes = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),
                    typeof(AssemblyDescriptionAttribute), false);
                return attributes?.Description;
            }
        }
        public string ApplicationCopyright
        {
            get
            {
                var attributes = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),
                    typeof(AssemblyCopyrightAttribute), false);
                return attributes?.Copyright;
            }
        }
        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();   
            }
        }

        public string ApplicationHomePage = "https://github.com/HarpyWar/chronofag-timer";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Config(string fileName)
        {
            var jobj = HjsonValue.Load(fileName).Qo();

            MaxExtraTimes = jobj["breaktimer"]["max_extratimes"];
            ExtraTime = jobj["breaktimer"]["extratime"];

            Times = new List<TimeUnit>();
            foreach (dynamic item in jobj["rounds"])
            {
                TimeUnit unit;
                if (item["type"] == "work")
                {
                    unit = new Pomodoro(item["time"], ExtraTime, item["title"]);
                }
                else // break
                {
                    unit = new Break(item["time"], ExtraTime, item["title"]);
                }
                Times.Add(unit);
            }

            if (Times.Where(t => t is Pomodoro).FirstOrDefault() == null || Times.Where(t => t is Break).FirstOrDefault() == null)
            {
                Logger.Fatal("rounds must contain at least one pomodoro and one break");
                Environment.Exit(1);
            }


            // alerts
            Alerts = new List<Alert>();
            if (jobj["alerts"] != null)
            {
                foreach (dynamic item in jobj["alerts"])
                {
                    Alert alert = new Alert() {
                        Remain = item["remain"],
                        Duration = item["duration"],
                        Sound = item["sound"]
                    };
                    Alerts.Add(alert);
                }
            }

            // silence apps
            SilenceApps = new List<string>();
            foreach (dynamic item in jobj["silence_apps"])
            {
                SilenceApps.Add(item);
            }


            // colors
            Face = new Style()
            {
                PomodoroBackground = FromHex(jobj["worktimer"]["background"]),
                PomodoroForeground = FromHex(jobj["worktimer"]["foreground"]),
                BreakBackground = FromHex(jobj["breaktimer"]["background"]),
                BreakForeground = FromHex(jobj["breaktimer"]["foreground"]),
                UserTimerBackground = new List<Color>(),
                UserTimerForeground = FromHex(jobj["usertimer"]["foreground"])
            };
            foreach (dynamic item in jobj["usertimer"]["background"])
            {
                Face.UserTimerBackground.Add(FromHex(item));
            }

            // phrases
            Phrases = new Dictionary<string, string>();
            foreach (dynamic item in jobj["phrases"])
            {
                Phrases.Add(item.Key, item.Value);
            }

            // position
            Position = jobj["worktimer"]["position"];
            MouseArea = jobj["worktimer"]["mousearea"];

            // sounds
            PomodoroSound = jobj["worktimer"]["sound"];
            BreakSound = jobj["breaktimer"]["sound"];
            UserTimerSound = jobj["usertimer"]["sound"];

            UserTimerShowFirstTime = jobj["usertimer"]["showfirsttime"];
            WorkTimerShowFirstTime = jobj["worktimer"]["showfirsttime"];

            IdleTime = jobj["idletime"];
            LockKeyboard = jobj["breaktimer"]["lockkeyboard"];

        }

        public List<TimeUnit> Times { get; private set; }
        public List<Alert> Alerts { get; private set; }
        public List<string> SilenceApps { get; private set; }

        public Style Face { get; private set; }
        public Dictionary<string, string> Phrases { get; private set; }

        public string Position { get; private set; }
        public int MouseArea { get; private set; }

        /// <summary>
        /// File to play when pomodoro start
        /// </summary>
        public string PomodoroSound { get; private set; }
        /// <summary>
        /// File to play when break start
        /// </summary>
        public string BreakSound { get; private set; }
        public string UserTimerSound { get; private set; }
        
        public int IdleTime { get; private set; }
        public bool LockKeyboard { get; private set; }

        public int UserTimerShowFirstTime { get; private set; }
        public int WorkTimerShowFirstTime { get; private set; }

        public int ExtraTime { get; private set; }
        public int MaxExtraTimes { get; private set; }


        public struct Style
        {
            public Color PomodoroForeground;
            public Color PomodoroBackground;
            public Color BreakBackground;
            public Color BreakForeground;
            public List<Color> UserTimerBackground;
            public Color UserTimerForeground;
        }


        public string GetPhrase(string key)
        {
            if (Phrases.ContainsKey(key))
            {
                return Phrases[key];
            }
            return string.Format("Phrase '{0}' is not defined", key);
        }


        private Color FromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception(string.Format("Color {0} is not valid", hex));

            return Color.FromArgb(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }



    }
}
