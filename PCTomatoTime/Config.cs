using Hjson;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PCTomatoTime
{
    public class Config
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Config(string fileName)
        {
            var jobj = HjsonValue.Load(fileName).Qo();

            Times = new List<TimeUnit>();
            foreach (dynamic item in jobj["rounds"])
            {
                TimeUnit unit;
                if (item["type"] == "work")
                {
                    unit = new Pomodoro(item["time"], item["title"]);
                }
                else // break
                {
                    unit = new Break(item["time"], item["title"]);
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
                PomodoroBackground = FromHex(jobj["pomodoro"]["background"]),
                PomodoroForeground = FromHex(jobj["pomodoro"]["foreground"]),
                BreakBackground = FromHex(jobj["break"]["background"]),
                BreakForeground = FromHex(jobj["break"]["foreground"]),
                UserTimerBackground = new List<Color>(),
                UserTimerForeground = FromHex(jobj["usertimer"]["foreground"])
            };
            foreach (dynamic item in jobj["usertimer"]["background"])
            {
                Face.UserTimerBackground.Add(FromHex(item));
            }


            // position
            Position = jobj["pomodoro"]["position"];
            MouseArea = jobj["pomodoro"]["mousearea"];

            // sounds
            PomodoroSound = jobj["pomodoro"]["sound"];
            BreakSound = jobj["break"]["sound"];
            UserTimerSound = jobj["usertimer"]["sound"];

            UserTimerShowFirstTime = jobj["usertimer"]["showfirsttime"];

            IdleTime = jobj["idletime"];
            IdleTitle = jobj["idle_title"];
            AlwaysOnTop = jobj["alwaysontop"];
        }

        public List<TimeUnit> Times { get; private set; }
        public List<Alert> Alerts { get; private set; }
        public List<string> SilenceApps { get; private set; }

        public Style Face { get; private set; }

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
        public string IdleTitle { get; private set; }
        public bool AlwaysOnTop{ get; private set; }

        public int UserTimerShowFirstTime { get; private set; }

        public struct Style
        {
            public Color PomodoroForeground;
            public Color PomodoroBackground;
            public Color BreakBackground;
            public Color BreakForeground;
            public List<Color> UserTimerBackground;
            public Color UserTimerForeground;
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
