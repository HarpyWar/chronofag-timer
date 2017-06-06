using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCTomatoTime
{
    public partial class Form1 : Form
    {

        private Timer timeUnitTimer, tomatoShowTimer;
        int counter = 0;
        int currentRound = 0;
        /// <summary>
        /// Overall pomodoros for the current day
        /// </summary>
        int pomodoroCounter = 0;

        /// <summary>
        /// Current alert, show if not null
        /// </summary>
        Alert Alert = null;

        Config config;

        public Form1()
        {
            InitializeComponent();
            this.Width = this.Height = 0;
        }

        /// <summary>
        /// Visible property sometimes true inside Timer.Tick event when actually it's false
        /// so use own robust property
        /// </summary>
        public bool Visibility = true;

        private void Form1_Load(object sender, EventArgs e)
        {
            var configFile = "config.hjson";
            try
            {
                config = new Config(configFile);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error loading " + configFile);
                Environment.Exit(1);
            }

            timeUnitTimer = new Timer()
            {
                Interval = 1000,
                Enabled = true
            };
            tomatoShowTimer = new Timer()
            {
                Interval = 10,
                Enabled = true
            };
    

            timeUnitTimer.Tick += _timeUnitTimer_Elapsed;
            tomatoShowTimer.Tick += TomatoShowTimer_Tick;

            var fontSizeBreak = Screen.PrimaryScreen.Bounds.Width / 20;
            lblBreakTime.Font = new Font(FontFamily.GenericSerif, fontSizeBreak);
            lblBreakTime.ForeColor = config.Face.BreakForeground;

            var fontSizeTomato = Screen.PrimaryScreen.Bounds.Width / 50;
            lblPomodoroTime.Font = new Font(FontFamily.GenericSerif, fontSizeTomato);
            lblPomodoroTime.ForeColor = config.Face.PomodoroForeground;

            lblPomodoroCounter.ForeColor = config.Face.CounterForeground;

            startPomodoro();
        }

        private void TomatoShowTimer_Tick(object sender, EventArgs e)
        {
            if (IsPomodoro)
            {
                // if mouse cursor in hot area
                if (mouseShowPomodoro())
                {
                    // show form
                    this.FadeIn(true);
                }
                else
                {
                    if (Alert == null)
                    {
                        this.FadeOut();
                    }
                }
            }
        }

        public bool IsPomodoro
        {
            get
            {
                return CurrentTimeUnit is Pomodoro;
            }
        }

        /// <summary>
        /// Current time unit (pomodoro or break)
        /// </summary>
        public TimeUnit CurrentTimeUnit
        {
            get
            {
                return config.Times[currentRound];
            }
        }

        private void _timeUnitTimer_Elapsed(object sender, EventArgs e)
        {
            // check for round finish
            if (counter >= CurrentTimeUnit.CounterLimit)
            {
                // reset counter
                counter = 0;
                // reset alert
                Alert = null;

                // increase round
                currentRound++;
                if (currentRound >= config.Times.Count)
                {
                    currentRound = 0;
                }

                if (CurrentTimeUnit is Break)
                {
                    startBreak();
                }
                if (CurrentTimeUnit is Pomodoro)
                {
                    startPomodoro();
                }
                return;
            }

            counter++;

            // alerts
            foreach(var alert in config.Alerts)
            {
                if (alert.Remain == CurrentTimeUnit.CounterLimit - counter)
                {
                    this.FadeIn();
                    alert.Reset();
                    Alert = alert;
                    Helper.PlaySound(alert.Sound);
                }
            }
            if (Alert != null)
            {
                Alert.Elapsed++;
                if (Alert.Elapsed > Alert.Duration)
                {
                    Alert = null;
                }
            }

            // set label position
            if (CurrentTimeUnit is Break)
            {
                updateBreakPosition();
            }
            if (CurrentTimeUnit is Pomodoro)
            {
                updateTomatoPosition();
            }
            updatePomodoroCounterPosition();
        }

        private void startBreak()
        {
            Helper.PlaySound(config.BreakSound);

            // update form size equal to screen size
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Top = this.Left = 0;
            this.BackColor = config.Face.BreakBackground;

            lblBreakTime.Show();
            lblPomodoroTime.Hide();

            updateBreakPosition();
            updatePomodoroCounterPosition();

            this.FadeIn();
        }
        private void updateBreakPosition()
        {
            lblBreakTime.Text = getTimeElapsedString(); 

            lblBreakTime.Left = this.Width / 2 - lblBreakTime.Width / 2;
            lblBreakTime.Top = this.Height / 2 - lblBreakTime.Height / 2;
        }

        private void startPomodoro()
        {
            Helper.PlaySound(config.PomodoroSound);

            this.FadeOut();

            this.Width = Screen.PrimaryScreen.Bounds.Width / 8;
            this.Height = Screen.PrimaryScreen.Bounds.Height / 8;
            var pos = getPomodoroPosition();
            this.Top = pos.X;
            this.Left = pos.Y;

            this.BackColor = config.Face.PomodoroBackground;

            lblPomodoroTime.Show();
            lblBreakTime.Hide();

            updateTomatoPosition();
            updatePomodoroCounterPosition();

            pomodoroCounter++;
        }
        private void updateTomatoPosition()
        {
            lblPomodoroTime.Text = getTimeElapsedString();

            lblPomodoroTime.Left = this.Width / 2 - lblPomodoroTime.Width / 2;
            lblPomodoroTime.Top = this.Height / 2 - lblPomodoroTime.Height / 2;
        }

        private void updatePomodoroCounterPosition()
        {
            lblPomodoroCounter.Text = string.Format("{0} pomodoro", pomodoroCounter);

            switch (config.Position)
            {
                case "top-right":
                case "bottom-right":
                case "right":
                    lblPomodoroCounter.Left = 10;
                    lblPomodoroCounter.Top = 10;
                    break;
                case "top-left":
                case "bottom-left":
                case "bottom":
                case "top":
                case "left":
                    lblPomodoroCounter.Left = this.Width - lblPomodoroCounter.Width - 10;
                    lblPomodoroCounter.Top = 10;
                    break;
                default:
                    goto case "top-left";
            }
        }



        private string getTimeElapsedString()
        {
            var time = TimeSpan.FromSeconds(CurrentTimeUnit.CounterLimit - counter);
            return time.ToString(@"mm\:ss");
        }


        private Point getPomodoroPosition()
        {
            var sw = Screen.PrimaryScreen.Bounds.Width;
            var sh = Screen.PrimaryScreen.Bounds.Height;
            var fw = this.Width;
            var fh = this.Height;

            // (top, left)
            Point point;
            switch (config.Position)
            {
                case "top-right":
                    point = new Point(0, sw - fw);
                    break;
                case "top-left":
                    point = new Point(0, 0);
                    break;
                case "bottom-left":
                    point = new Point(sh - fh, 0);
                    break;
                case "bottom-right":
                    point = new Point(sh - fh, sw - fw);
                    break;
                case "bottom":
                    point = new Point(sh - fh, (sw / 2) - (fw / 2));
                    break;
                case "top":
                    point = new Point(0, (sw / 2) - (fw / 2));
                    break;
                case "left":
                    point = new Point((sh / 2) - (fh / 2), 0);
                    break;
                case "right":
                    point = new Point((sh / 2) - (fh / 2), sw - fw);
                    break;
                default:
                    goto case "top-left";
            }
            return point;
        }

        private bool mouseShowPomodoro()
        {
            var area = config.MouseArea;
            var w = Screen.PrimaryScreen.Bounds.Width;
            var h = Screen.PrimaryScreen.Bounds.Height;
            var x = Cursor.Position.X;
            var y = Cursor.Position.Y;

            switch (config.Position)
            {
                case "top-right":
                    return x > w - area && y < area;
                case "top-left":
                    return x < area && y < area;
                case "bottom-left":
                    return x < area && y > h - area;
                case "bottom-right":
                    return x > w - area && y > h - area;
                case "bottom":
                    return y > h - area;
                case "top":
                    return y < area;
                case "left":
                    return x < area;
                case "right":
                    return x > w - area;
                default:
                    goto case "top-left";
            }
        }


        #region show/hide animation

        public void FadeIn(bool immediate = false)
        {
            // do nothing if already visible
            if (this.Visibility)
            {
                return;
            }
            this.Opacity = 0;
            this.Show();
            this.Visibility = true;
            if (immediate)
            {
                this.Opacity = 100;
                return;
            }
            for (int i = 1; i < 100; i+=2)
            {
                var opacity = (double)i / 100;
                this.Opacity = opacity;
                this.Refresh();
            }
          
        }

        public void FadeOut()
        {
            // do nothing if already hidden
            if (!this.Visibility)
            {
                return;
            }
            for (int i = 100; i > 0; i--)
            {
                var opacity = (double)i / 100;
                this.Opacity = opacity;
                this.Refresh();
            }
            this.Hide();
            this.Visibility = false;
        }



        #endregion

    }
}
