using Hjson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PCTomatoTime
{
    public class Config
    {
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

            // colors
            Face.PomodoroBackground = FromHex(jobj["pomodoro"]["background"]);
            Face.PomodoroForeground = FromHex(jobj["pomodoro"]["foreground"]);
            Face.BreakBackground = FromHex(jobj["break"]["background"]);
            Face.BreakForeground = FromHex(jobj["break"]["foreground"]);

            // position
            Position = jobj["pomodoro"]["position"];
            MouseArea = jobj["pomodoro"]["mousearea"];

            // sounds
            PomodoroSound = jobj["pomodoro"]["sound"];
            BreakSound = jobj["break"]["sound"];
        }

        public List<TimeUnit> Times { get; private set; }
        public List<Alert> Alerts { get; private set; }

        public Style Face;

        public string Position;
        public int MouseArea;

        /// <summary>
        /// File to play when pomodoro start
        /// </summary>
        public string PomodoroSound;
        /// <summary>
        /// File to play when break start
        /// </summary>
        public string BreakSound;


        public struct Style
        {
            public Color PomodoroForeground;
            public Color PomodoroBackground;
            public Color BreakBackground;
            public Color BreakForeground;
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
