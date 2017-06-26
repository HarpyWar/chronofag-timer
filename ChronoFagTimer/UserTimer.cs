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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChronoFagTimer
{
    class UserTimer
    {
        // Declare the delegate (if using non-generic pattern).
        public delegate void StoppedEventHandler(object sender, EventArgs e);

        // Declare the event.
        public event StoppedEventHandler StoppedEvent;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Form form;
        private TomatoForm parentForm;
        private Label lblTitle, lblDownTitle, lblTime;
        private Timer timer;

        public string Key
        {
            get
            {
                return _key;
            }
            private set
            {
                _key = value;
            }
        }
        private string _key;


        public int Seconds
        {
            get
            {
                return _seconds;
            }
            private set{
                _seconds = value;
            }
        }
        private int _seconds;

        public int Counter
        {
            get
            {
                return _counter;
            }
            private set
            {
                _counter = value;
            }
        }
        private int _counter;


        public bool Elapsed
        {
            get
            {
                return Counter >= Seconds;
            }
        }


        private Config config;

        /// <summary>
        /// Identifier (starting from 1)
        /// It used to calculate form position on the screen
        /// </summary>
        public int ID;

        private bool visibility = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds">When timer should be stopped</param>
        public UserTimer(string key, string title, int seconds, Config config, TomatoForm parentForm)
        {
            this.parentForm = parentForm;
            this.config = config;
            this.Seconds = seconds;
            this.Key = key;

            var size = Helper.GetFormSize();
            // init form
            form = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                Width = size.X,
                Height = size.Y,
                Cursor = Cursors.Hand,
                TopMost = true,
                ShowInTaskbar = false
            };


            lblTitle = new Label()
            {
                Text = title,
                AutoSize = true
            };
            lblDownTitle = new Label()
            {
                AutoSize = true
            };
            lblTime = new Label()
            {
                AutoSize = true
            };

            form.Click += Form_Click;
            lblTitle.Click += Form_Click;
            lblDownTitle.Click += Form_Click;
            lblTime.Click += Form_Click;


            // set font size
            lblTime.Font = new Font(FontFamily.GenericSerif, Helper.GetTimerFontSize());

            lblTitle.Font = new Font(FontFamily.GenericSerif, Helper.GetTitleFontSize());
            lblDownTitle.Font = new Font(FontFamily.GenericSerif, Helper.GetDownTitleFontSize());

            // set colors
            lblTime.ForeColor = lblTitle.ForeColor = lblDownTitle.ForeColor = config.Face.UserTimerForeground;
            // set random background color
            form.BackColor = config.Face.UserTimerBackground[Helper.GetRandom(0, config.Face.UserTimerBackground.Count - 1)];

            // add labels to the form
            form.Controls.AddRange(new Control[] {
                lblTitle, lblDownTitle, lblTime
            });


            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            updateElementsPosition();
            Start();
        }

        private void Form_Click(object sender, EventArgs e)
        {
            if (StoppedEvent != null)
            {
                StoppedEvent(this, null);
            }
            form.Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Counter == config.UserTimerShowFirstTime)
            {
                // do not hide if parent tomato form is now visible
                if (!parentForm.Visibility)
                {
                    Hide();
                }
            }

            Counter++;

            Logger.Trace("User timer counter {0}: {1}", Key, Counter);

            // when elapsed then stop timer and show form
            if (Elapsed)
            {
                Stop();

                lblDownTitle.Text = string.Format("{0} {1}:{2}:{3}", config.GetPhrase("timerstoptime"), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                Helper.PlaySound(config.UserTimerSound);
                Show();
            }
            updateElementsPosition();
        }

        private void updateElementsPosition()
        {
            // form
            if (!visibility)
            {
                return;
            }

            // set always on top 
            if (!form.TopMost)
            {
                form.TopMost = true;
            }

            // timer
            lblTime.Text = Helper.GetTimeElapsedString(Seconds, Counter);
            lblTime.Left = form.Width / 2 - lblTime.Width / 2;
            lblTime.Top = form.Height / 2 - lblTime.Height / 2;

            // title
            lblTitle.Left = form.Width / 2 - lblTitle.Width / 2;
            lblTitle.Top = lblTime.Top / 4;
            lblDownTitle.Left = form.Width / 2 - lblDownTitle.Width / 2;
            lblDownTitle.Top = lblTime.Top + lblTime.Height + (lblTime.Top / 4);
        }


        public void Show()
        {
            if (!visibility)
            {
                // if first time not visible then show form
                if (!form.Visible)
                {
                    form.Show();
                }
                visibility = true;

                var pos = getFormPosition(ID, form.Height);
                form.Left = pos.X;
                form.Top = pos.Y;
                if (!form.TopMost)
                {
                    form.TopMost = true;
                }


                updateElementsPosition();
                Logger.Trace("Show user timer " + lblTitle.Text);
            }
        }

        public void Hide()
        {
            if (visibility && !Elapsed)
            {
                visibility = false;

                // make form invisible
                form.Left = -form.Width;
                form.Top = -form.Height;

                Logger.Trace("Hide user timer " + lblTitle.Text);
            }
        }

        public void Start()
        {
            timer.Start();
            Logger.Info("User timer started: {0}", Key);
        }

        public void Stop()
        {
            timer.Stop();
            Logger.Info("User timer elapsed: {0}", Key);
        }

        public void Destroy()
        {
            Stop();
            form.Dispose();
            Logger.Debug("Destroy user timer " + lblTitle.Text);
        }




        private Point getFormPosition(int id, int height)
        {
            // TODO: handle each possible form position like in getPomodoroPosition()
            return new Point(0, id * height);
        }
    }
}
