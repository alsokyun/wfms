using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using SHDocVw;

namespace GTI.WFMS.Models.FileUtils
{
    public class CompleteRecordDragDropEventArgsConverter : MarkupExtension, IEventArgsConverter {

        #region Struct

        [StructLayout(LayoutKind.Sequential)]
        struct Point {
            public int X;
            public int Y;
        }

        #endregion Struct

        #region Enums

        enum GetWindow_Cmd : uint {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        #endregion Enums

        #region Extern

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);


        #endregion Extern

        #region Methods

        public object Convert(object sender, object args) {
            var e = (CompleteRecordDragDropEventArgs)args;

            //get mouse cursor position that file is dropped
            GetCursorPos(out Point cursorPosition);

            if (IsMouseOverWindow(cursorPosition, 
                (int)(Application.Current.MainWindow.Top),
                (int)(Application.Current.MainWindow.Height),
                (int)(Application.Current.MainWindow.Left),
                (int)(Application.Current.MainWindow.Width)))
            {
                e.Handled = false;
                return null;
            }
            else
            {
                // set default directory
                string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                //get current windows and the window immediately below it
                IntPtr activeWindow = GetActiveWindow();
                IntPtr nextWindow = GetWindow(activeWindow, (uint)GetWindow_Cmd.GW_HWNDNEXT);

                //get windows z orders
                var pointerDictionary = new Dictionary<int, IntPtr>();
                int zIndex = 0;
                while(nextWindow != IntPtr.Zero) { 
                    pointerDictionary.Add(zIndex, nextWindow);
                    zIndex++;
                    nextWindow = GetWindow(nextWindow, (uint)GetWindow_Cmd.GW_HWNDNEXT);
                }

                // add reference: SHDocVw (Microsoft Internet Controls COM Object) - C:\Windows\system32\ShDocVw.dll
                var shellWindows = new ShellWindows();

                // get shell windows under cursor position
                var shellDictionary = new Dictionary<int, InternetExplorer>();
                if (shellWindows.Count > 0 && pointerDictionary.Count > 0) {
                    foreach (InternetExplorer window in shellWindows) {
                        IntPtr hwnd = (IntPtr)window.HWND;
                        int? order = pointerDictionary.FirstOrDefault(p => p.Value == hwnd).Key;
                        if (order != null && IsMouseOverWindow(cursorPosition, window)) {
                            shellDictionary.Add((int)order, window);
                        }
                    }

                    // get file name of the top most shell window
                    if (shellDictionary.Count > 0) {
                        var targetWindow = shellDictionary.OrderBy(d => d.Key).First().Value;
                        string appName = Path.GetFileNameWithoutExtension(targetWindow.FullName).ToLower();
                        switch (appName) {
                            case "iexplore":
                                directory = Path.GetTempPath();
                                break;
                            case "explorer":
                                directory = targetWindow.LocationURL;
                                break;
                        }
                    }
                }
                e.Handled = true;
                return directory;
            }
        }

        private bool IsMouseOverWindow(Point point, InternetExplorer window) {
            int top = window.Top;
            int height = window.Height;
            int left = window.Left;
            int width = window.Width;
            return IsMouseOverWindow(point, top, height, left, width);
        }

        private bool IsMouseOverWindow(Point point, int top, int height, int left, int width) {
            int x = point.X;
            int y = point.Y;
            int bottom = top + height;
            int right = left + width;
            if (x > left && x < right && y > top && y < bottom) {
                return true;
            }
            return false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }

        #endregion Methods 

    }
}
