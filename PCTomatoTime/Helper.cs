using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;
using System.IO;

namespace PCTomatoTime
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
            return time.ToString(@"mm\:ss");
        }

        public static int GetRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }
    }
}
