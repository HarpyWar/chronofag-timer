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

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChronoFagTimer
{
    public class WinApi
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Get current running active process filename
        /// </summary>
        /// <returns></returns>
        public static string GetActiveProcessFileName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p.ProcessName;
        }


        private static string runPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static void SetStartup(string appName, string appPath, bool check)
        {
            using (var rk = Registry.CurrentUser.OpenSubKey(runPath, true))
            {
                if (check)
                    rk.SetValue(appName, appPath);
                else
                    rk.DeleteValue(appName, false);
            }
        }

        public static bool GetStartup(string appName, string appPath)
        {
            using (var rk = Registry.CurrentUser.OpenSubKey(runPath, false))
            {
                var val = rk.GetValue(appName);
                if (val != null)
                {
                    // if bad path then return false
                    if (val.ToString() != appPath)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// http://blogs.msdn.com/toub/archive/2006/05/03/589423.aspx
        /// </summary>
        public class InterceptKeys
        {
            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100;
            private static LowLevelKeyboardProc _proc = HookCallback;
            private static IntPtr _hookID = IntPtr.Zero;


            public static void LockKeyboard()
            {
                _hookID = SetHook(_proc);
            }
            public static void UnlockKeyboard()
            {
                UnhookWindowsHookEx(_hookID);
            }

            private static IntPtr SetHook(LowLevelKeyboardProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

            private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                // eat all keys
                return (IntPtr)1;
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);
        }


        public class WindowPos
        {
            private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            private const UInt32 SWP_NOSIZE = 0x0001;
            private const UInt32 SWP_NOMOVE = 0x0002;
            private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            public static void BringToFront(IntPtr handle)
            {
                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }


        /// <summary>
        /// Helps to find the idle time spent since the last user input
        /// </summary>
        public class IdleTimeFinder
        {
            [DllImport("User32.dll")]
            private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

            [DllImport("Kernel32.dll")]
            private static extern uint GetLastError();

            /// <summary>
            /// Return idle time in seconds
            /// </summary>
            /// <returns></returns>
            public static uint GetIdleTimeSec()
            {
                return GetIdleTime() / 1000;
            }

            /// <summary>
            /// Return idle time in milliseconds
            /// </summary>
            /// <returns></returns>
            public static uint GetIdleTime()
            {
                LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                GetLastInputInfo(ref lastInPut);

                return ((uint)Environment.TickCount - lastInPut.dwTime);
            }

            /// <summary>
            /// Get the Last input time in milliseconds
            /// </summary>
            /// <returns></returns>
            public static long GetLastInputTime()
            {
                LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                if (!GetLastInputInfo(ref lastInPut))
                {
                    throw new Exception(GetLastError().ToString());
                }
                return lastInPut.dwTime;
            }

            internal struct LASTINPUTINFO
            {
                public uint cbSize;

                public uint dwTime;
            }
        }
    }
}
