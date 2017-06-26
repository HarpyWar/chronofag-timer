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
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ChronoFagTimer
{
    public partial class TomatoForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Timer timeUnitTimer, tomatoShowTimer;

        /// <summary>
        /// Custom user timers
        /// </summary>
        private Dictionary<string, UserTimer> customTimers = new Dictionary<string, UserTimer>();

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
                // increase delta only for next pomodoro but not first
                if (CurrentRound <= 0)
                {
                    return;
                }

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

        /// <summary>
        /// Visible property sometimes true inside Timer.Tick event when actually it's false
        /// so use own robust property
        /// </summary>
        public bool Visibility = true;

        public TomatoForm()
        {
            InitializeComponent();

            var configFile = "config.hjson";
            try
            {
                config = new Config(configFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error loading " + configFile);
                Environment.Exit(1);
            }


            this.Width = this.Height = 0;

            // fill tray context menutimerstoptime
            var menuQuit = new MenuItem() { Text = config.GetPhrase("quit") };
            menuQuit.Click += MenuQuit_Click;

            var menuAutostart = new MenuItem()
            {
                Text = config.GetPhrase("autostart"),
                Name = "menuAutostart",
                Checked = WinApi.GetStartup(config.ApplicationName, Application.ExecutablePath)
            };
            menuAutostart.Click += MenuAutostart_Click;

            var menuAddTimer = new MenuItem() { Text = config.GetPhrase("addtimer") };
            menuAddTimer.Click += MenuAddTimer_Click;

            var contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuAddTimer, menuAutostart, menuQuit });
            this.notifyIcon1.Text = this.Text = config.ApplicationName;
            this.notifyIcon1.ContextMenu = contextMenu;

            lblDownTitle.Text = config.LockExit
                ? config.GetPhrase("lockmode")
                : config.GetPhrase("freemode");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

        private void MenuAutostart_Click(object sender, EventArgs e)
        {
            // toggle startup
            try
            {
                var check = false;
                if (WinApi.GetStartup(config.ApplicationName, Application.ExecutablePath))
                {
                    WinApi.SetStartup(config.ApplicationName, Application.ExecutablePath, false);
                    Logger.Debug("Unet startup");
                }
                else
                {
                    WinApi.SetStartup(config.ApplicationName, Application.ExecutablePath, true);
                    check = true;
                    Logger.Debug("Set startup");
                }
                notifyIcon1.ContextMenu.MenuItems.Find("menuAutostart", false).First().Checked = check;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MenuQuit_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }
        private void MenuAddTimer_Click(object sender, EventArgs e)
        {
            if (customTimers.Count >= 7)
            {
                MessageBox.Show(config.GetPhrase("maxtimerswarn"), config.GetPhrase("maxtimerswarntitle"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var frm = new NewUserTimerForm(config);
            frm.ShowDialog(this);
        }
        private void MenuRemoveTimer_Click(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            RemoveCustomTimer(item.Name);
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
                var idleTime = WinApi.IdleTimeFinder.GetIdleTimeSec();

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
            handleSleepLeap();

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
                        Counter--;
                        // update pomodoro timer
                        goto updatePositions;
                    }
                    else
                    {
                        IdleDeltaCounter++;
                    }
                    // do nothing if counter <= 0
                    return;
                }
            }

            Counter++;

            // handle alerts
            foreach (var alert in config.Alerts)
            {
                if (alert.Remain == CurrentTimeUnit.CounterLimit - Counter)
                {
                    Helper.PlaySound(alert.Sound);
                    // show alert only for all apps except silence 
                    if (AllowMouseEventForCurrentProcess())
                    {
                        // do not show custom timers when alert
                        this.FadeIn(true, false);
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
                updateBreakPosition();
            }
            if (CurrentTimeUnit is Pomodoro)
            {
                updateTomatoPosition();
            }
            updateElementsPosition();
        }

        /// <summary>
        /// Last timer active time
        /// </summary>
        DateTime? lastActiveTime = null;
        private void handleSleepLeap()
        {
            if (lastActiveTime == null)
            {
                // first assign
                lastActiveTime = DateTime.Now;
            }
            var diff = (DateTime.Now - (DateTime)lastActiveTime).TotalSeconds;

            // if was leap
            if (diff > config.IdleTime)
            {
                Logger.Info("Time leap detected for {0} seconds", diff);
                // iterate diff untill null
                while (diff > 0)
                {
                    diff--;
                    if (Counter > 0)
                    {
                        Counter--;
                    }
                    else
                    {
                        IdleDeltaCounter++;
                    }
                }
            }
            lastActiveTime = DateTime.Now;
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
            lblBreakTime.Text = Helper.GetTimeElapsedString(CurrentTimeUnit.CounterLimit, Counter);

            lblBreakTime.Left = this.Width / 2 - lblBreakTime.Width / 2;
            lblBreakTime.Top = this.Height / 2 - lblBreakTime.Height / 2;

            // focus form
            this.Activate();
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
            lblPomodoroTime.Text = Helper.GetTimeElapsedString(CurrentTimeUnit.CounterLimit, Counter);

            lblPomodoroTime.Left = this.Width / 2 - lblPomodoroTime.Width / 2;
            lblPomodoroTime.Top = this.Height / 2 - lblPomodoroTime.Height / 2;
        }

        private void updateElementsPosition()
        {
            // set always on top 
            if (!this.TopMost)
            {
                this.TopMost = true; 
            }

            // title
            var fontSizeTitle = (IsPomodoro ? lblPomodoroTime.Font.Size : lblBreakTime.Font.Size) / 4;
            lblTitle.Font = lblDownTitle.Font = new Font(FontFamily.GenericSerif, fontSizeTitle);

            lblTitle.Text = IsIdle
                ? config.IdleTitle
                : CurrentTimeUnit.Title;
            lblTitle.Left = this.Width / 2 - lblTitle.Width / 2;
            lblTitle.Top = (IsPomodoro ? lblPomodoroTime.Top : lblBreakTime.Top) / 4;
            lblTitle.ForeColor = lblDownTitle.ForeColor = IsPomodoro ? config.Face.PomodoroForeground : config.Face.BreakForeground;

            if (!IsPomodoro)
            {
                lblDownTitle.Show();
                lblDownTitle.Left = this.Width / 2 - lblDownTitle.Width / 2;
                lblDownTitle.Top = this.Height - (this.Height - lblBreakTime.Top) / 4;
            }
            else
            {
                lblDownTitle.Hide();
            }
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
        public void FadeIn(bool immediate = false, bool customTimers = true)
        {
            // do nothing if already visible
            if (this.Visibility)
            {
                return;
            }
            this.Opacity = 0;
            this.Left = this.Top = 0; //instead this.Show(); which toggle focus
            Logger.Trace("Show");

            if (customTimers)
            {
                toggleUserTimers(true);
            }


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
            toggleUserTimers(false);
            // if customtimers not null then also immediate hide
            if (immediate || customTimers.Count > 0)
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

        #region User Timers


        /// <summary>
        /// 
        /// </summary>
        /// <param name="show">show or hide</param>
        private void toggleUserTimers(bool show)
        {
            // show only if not alert
            if (show)
            {
                updateUserTimerIDs();
                customTimers.OrderBy(t => t.Value.Seconds).ToList().ForEach(t => t.Value.Show());
            }
            else
            {
                customTimers.OrderBy(t => t.Value.Seconds).ToList().ForEach(t => t.Value.Hide());
            }
        }

        private void updateUserTimerIDs()
        {
            var i = 0;
            // update timers ID to update positioon when old timers deleted
            customTimers.OrderBy(t => t.Value.Seconds).ToList().ForEach(t => { t.Value.ID = ++i; });
        }


        /// <summary>
        /// Add custom timer into 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="seconds">when timer should be elapsed</param>
        /// <returns></returns>
        public string AddCustomTimer(string title, int seconds)
        {
            var key = Guid.NewGuid().ToString();

            MenuItem menuRemoveTimer;
            // add "Remove Timer" menu item if not exist
            var find = notifyIcon1.ContextMenu.MenuItems.Find("menuRemoveItem", false);
            if (find.Count() == 0)
            {
                menuRemoveTimer = new MenuItem() { Text = config.GetPhrase("removetimer"), Name = "menuRemoveItem" };
                notifyIcon1.ContextMenu.MenuItems.Add(0, menuRemoveTimer);
            }
            else
            {
                menuRemoveTimer = find.First();
            }
            // add timer menu item 
            var menuTimerItem = new MenuItem() {
                Text = title,
                Name = key
            };
            menuTimerItem.Click += MenuRemoveTimer_Click;
            menuRemoveTimer.MenuItems.Add(menuTimerItem);

            // add timer
            var utimer = new UserTimer(key, title, seconds, config);
            utimer.StoppedEvent += Utimer_StoppedEvent;
            customTimers.Add(key, utimer);

            // show first time
            updateUserTimerIDs();
            utimer.Show();

            return key;

        }

        /// <summary>
        /// When user timer stopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Utimer_StoppedEvent(object sender, EventArgs e)
        {
            var utimer = (UserTimer)sender;
            RemoveCustomTimer(utimer.Key);
        }

        public void RemoveCustomTimer(string key)
        {
            notifyIcon1.ContextMenu.MenuItems.RemoveByKey(key);

            var find = notifyIcon1.ContextMenu.MenuItems.Find("menuRemoveItem", false);
            if (find.Count() > 0)
            {
                var menuItem = find.First();
                // remove item
                menuItem.MenuItems.RemoveByKey(key);

                // if there are no more timers then remove menu "Remove Timer"
                if (menuItem.MenuItems.Count == 0)
                {
                    notifyIcon1.ContextMenu.MenuItems.Remove(menuItem);
                }
            }

            // remove timer
            customTimers[key].Destroy();
            customTimers.Remove(key);
        }


        #endregion
    }
}
