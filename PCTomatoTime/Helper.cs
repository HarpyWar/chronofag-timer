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

        public static void PlaySound(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            try
            {
                // play in different thread to except sound lagging
                System.Threading.Tasks.Task.Factory.StartNew(() => {
                    var reader = new Mp3FileReader(fileName);
                    var waveOut = new WaveOut();
                    waveOut.Init(reader);
                    waveOut.Play();
                });
            }
            catch (Exception e)
            {
                Logger.Error("Could not play sound " + e.Message);
            }
        }
    }
}
