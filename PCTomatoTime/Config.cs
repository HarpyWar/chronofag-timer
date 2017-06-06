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
                int limit = item["time"] /** 60*/; // make seconds as minutes
                if (item["type"] == "pomodoro")
                {
                    unit = new Pomodoro(limit);
                }
                else // break
                {
                    unit = new Break(limit);
                }
                Times.Add(unit);
            }

            // style
            Face.PomodoroBackground = FromHex(jobj["style"]["pomodoro"]["background"]);
            Face.PomodoroForeground = FromHex(jobj["style"]["pomodoro"]["foreground"]);
            Face.BreakBackground = FromHex(jobj["style"]["break"]["background"]);
            Face.BreakForeground = FromHex(jobj["style"]["break"]["foreground"]);
            Face.CounterForeground = FromHex(jobj["style"]["counter"]["foreground"]);

            // position
            Position = jobj["style"]["pomodoro"]["position"];
            MouseArea = jobj["style"]["pomodoro"]["mousearea"];
        }

        public List<TimeUnit> Times { get; private set; }

        public Style Face;

        public string Position;
        public int MouseArea;

        public struct Style
        {
            public Color PomodoroForeground;
            public Color PomodoroBackground;
            public Color BreakBackground;
            public Color BreakForeground;
            public Color CounterForeground;
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
