using NLog;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Timer timeUnitTimer, tomatoShowTimer;


        int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                _counter = value;
                Logger.Trace("Counter = {0}", _counter);
            }
        }
        int _counter;


        /// <summary>
        /// Overall pomodoros for the current day
        /// </summary>
        int pomodoroCounter = 0;

        /// <summary>
        /// Current alert, show if not null
        /// </summary>
        Alert Alert = null;

        Config config;

        /// <summary>
        /// Current time index (to config.Times)
        /// </summary>
        int CurrentRound
        {
            get
            {
                return _currentRound;
            }
            set
            {
                _currentRound = value;
                Logger.Debug("Change round to {0} {1} ({2})", _currentRound, CurrentTimeUnit.GetType(), CurrentTimeUnit.Title);
            }
        }
        int _currentRound = 0;

        /// <summary>
        /// Additional time 
        /// </summary>
        int IdleDeltaCounter
        {
            get
            {
                return _idleDeltaCounter;
            }
            set
            {
                _idleDeltaCounter = value;
                Logger.Trace("Set IdleDeltaCounter = {0}", _idleDeltaCounter);
                if (_idleDeltaCounter > getPrevTime(typeof(Break)).CounterLimit)
                {
                    CurrentRound = getPrevTimeIndex(typeof(Pomodoro));
                    _idleDeltaCounter = 0; // reset
                }
            }
        }
        int _idleDeltaCounter;


        public Form1()
        {
            InitializeComponent();
            this.Width = this.Height = 0;

            var menuQuitItem = new MenuItem() { Text = "Quit" };
            menuQuitItem.Click += MenuQuitItem_Click;
            var contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuQuitItem });
            this.notifyIcon1.Text = this.Text = "Tomato Time";
            this.notifyIcon1.ContextMenu = contextMenu;
        }

        private void MenuQuitItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(0);
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

            var fontSizeTomato  = Screen.PrimaryScreen.Bounds.Width / 50;
            lblPomodoroTime.Font = new Font(FontFamily.GenericSerif, fontSizeTomato);
            lblPomodoroTime.ForeColor = config.Face.PomodoroForeground;


            startPomodoro();
        }


        public bool IsIdle
        {
            get
            {
                // if zero then disable idle mode
                if (config.IdleTime == 0)
                {
                    return false;
                }
                // handle idle only for pomodoro
                if (!IsPomodoro)
                {
                    return false;
                }


                // update idle time
                var idleTime = IdleTimeFinder.GetIdleTimeSec();

                var idle = idleTime >= config.IdleTime;
                if (!_idle && idle)
                {
                    _idle = true;
                    Logger.Info("Start idle");

                    // change interval for timer when idle
                    int newInterval = (int)(decimal.Divide(getPrevTime(typeof(Break)).CounterLimit, CurrentTimeUnit.CounterLimit) * 1000);
                    timeUnitTimer.Interval = newInterval;
                    Logger.Debug("Set timer interval = {0}", timeUnitTimer.Interval);
                    updateElementsPosition();
                }
                if (_idle && !idle)
                {
                    _idle = false;
                    Logger.Info("End idle");

                    // reset timer interval
                    timeUnitTimer.Interval = 1000;
                    Logger.Debug("Set timer interval = {0}", timeUnitTimer.Interval);
                    updateElementsPosition();
                }
                return idle;
            }
        }
        private bool _idle = false;

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
                return config.Times[CurrentRound];
            }
        }


        /// <summary>
        /// Timer to handle mouse move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TomatoShowTimer_Tick(object sender, EventArgs e)
        {
            if (IsPomodoro)
            {
                // if mouse cursor in hot area
                if (mouseShowPomodoro() || IsIdle)
                {
                    if (AllowMouseEventForCurrentProcess())
                    {
                        // show form
                        this.FadeIn(true);
                    }
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

        /// <summary>
        /// Main timer loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timeUnitTimer_Elapsed(object sender, EventArgs e)
        {

            // check for round finish
            if (Counter >= CurrentTimeUnit.CounterLimit)
            {
                // reset counter
                Counter = 0;
                // reset alert
                Alert = null;

                // increase round
                CurrentRound++;
                if (CurrentRound >= config.Times.Count)
                {
                    CurrentRound = 0;
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

            // handle idle
            if (IsPomodoro)
            {
                // pause pomodoro if idle
                if (IsIdle)
                {
                    if (Counter > 0)
                    {
                        // decrease counter up to init pomodoro time (while counter > 0)
                        Counter--;
                        // update pomodoro timer
                        goto updatePositions;
                    }
                    else
                    {
                        // increase delta only for next pomodoro but not first
                        if (CurrentRound > 0)
                        {
                            IdleDeltaCounter++;
                        }
                    }
                    // do nothing if counter <= 0
                    return;
                }
            }

            Counter++;


            // handle alerts
            foreach(var alert in config.Alerts)
            {
                if (alert.Remain == CurrentTimeUnit.CounterLimit - Counter)
                {
                    Helper.PlaySound(alert.Sound);
                    // show alert only for all apps except silence 
                    if (AllowMouseEventForCurrentProcess())
                    {
                        this.FadeIn();
                    }

                    alert.Reset();
                    Alert = alert;
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

        updatePositions:

            // set label position
            if (CurrentTimeUnit is Break)
            {
                // always on top for break window only if set in config
                if (config.AlwaysOnTop)
                {
                    this.TopMost = true; 
                }
                // if window "on top" but config value not "on top"
                else if (this.TopMost && !config.AlwaysOnTop)
                {
                    this.TopMost = false;
                }
                updateBreakPosition();
            }
            if (CurrentTimeUnit is Pomodoro)
            {
                if (!this.TopMost)
                {
                    this.TopMost = true; // always on top for pomodoro window
                }
                updateTomatoPosition();
            }
            updateElementsPosition();
        }

        private void startBreak()
        {
            this.FadeOut(true);

            Logger.Info("Start break[{0}] ({1})", CurrentRound, CurrentTimeUnit.Title);
            Helper.PlaySound(config.BreakSound);

            // update form size equal to screen size
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Top = this.Left = 0;
            this.BackColor = config.Face.BreakBackground;

            lblBreakTime.Show();
            lblPomodoroTime.Hide();

            updateBreakPosition();
            updateElementsPosition();


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
            Logger.Info("Start pomodoro[{0}|{1}] ({2})", CurrentRound, pomodoroCounter, CurrentTimeUnit.Title);
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
            updateElementsPosition();

            pomodoroCounter++;
        }
        private void updateTomatoPosition()
        {
            lblPomodoroTime.Text = getTimeElapsedString();

            lblPomodoroTime.Left = this.Width / 2 - lblPomodoroTime.Width / 2;
            lblPomodoroTime.Top = this.Height / 2 - lblPomodoroTime.Height / 2;
        }

        private void updateElementsPosition()
        {
            // title
            var fontSizeTitle = (IsPomodoro ? lblPomodoroTime.Font.Size : lblBreakTime.Font.Size) / 4;
            lblTitle.Font = new Font(FontFamily.GenericSerif, fontSizeTitle);

            lblTitle.Text = IsIdle
                ? config.IdleTitle
                : CurrentTimeUnit.Title;
            lblTitle.Left = this.Width / 2 - lblTitle.Width / 2;
            lblTitle.Top = (IsPomodoro ? lblPomodoroTime.Top : lblBreakTime.Top) / 4;
            lblTitle.ForeColor = IsPomodoro ? config.Face.PomodoroForeground : config.Face.BreakForeground;
        }



        private string getTimeElapsedString()
        {
            var time = TimeSpan.FromSeconds(CurrentTimeUnit.CounterLimit - Counter);
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

        /// <summary>
        /// Show form with fade animation
        /// </summary>
        /// <param name="immediate">without animation</param>
        public void FadeIn(bool immediate = false)
        {
            // do nothing if already visible
            if (this.Visibility)
            {
                return;
            }
            this.Opacity = 0;
            this.Left = this.Top = 0; //instead this.Show(); which toggle focus
            Logger.Trace("Show");
 
            this.Visibility = true;
            if (immediate)
            {
                this.Opacity = 100;
                return;
            }
            // fade in up to 95% (for half-visible break window)
            for (int i = 1; i < 95; i+=3)
            {
                System.Threading.Thread.Sleep(1);
                var opacity = (double)i / 100;
                this.Opacity = opacity;
                this.Refresh();
            }
          
        }

        /// <summary>
        /// Hide form with fade animation
        /// </summary>
        /// <param name="immediate">without animation</param>
        public void FadeOut(bool immediate = false)
        {
            // do nothing if already hidden
            if (!this.Visibility)
            {
                return;
            }
            Logger.Trace("Hide");
            if (immediate)
            {
                goto complete;
            }
            for (int i = 100; i > 0; i-=3)
            {
                System.Threading.Thread.Sleep(1);
                var opacity = (double)i / 100;
                this.Opacity = opacity;
                this.Refresh();
            }
            complete:
            
            this.Left = this.Top = -this.Width; //instead this.Hide(); which toggle focus
            this.Visibility = false;
        }



        #endregion


        /// <summary>
        /// Return previous time index from current (break or pomodoro)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int getPrevTimeIndex(Type type)
        {
            var i = CurrentRound;
            while (true)
            {
                i--;
                if (i < 0)
                {
                    i = config.Times.Count - 1;
                }
                if (config.Times[i].GetType() == type)
                {
                    return i;
                }
            }
        }
        private TimeUnit getPrevTime(Type type)
        {
            var i = getPrevTimeIndex(type);
            return config.Times[i];
        }


        /// <summary>
        /// Check if current active app is in silence apps
        /// </summary>
        /// <returns></returns>
        private bool AllowMouseEventForCurrentProcess()
        {
            var pname = WinApi.GetActiveProcessFileName();
            foreach(var app in config.SilenceApps)
            {
                if (pname.ToLower() == app.ToLower())
                {
                    return false;
                }
            }
            return true;
        }




    }
}
