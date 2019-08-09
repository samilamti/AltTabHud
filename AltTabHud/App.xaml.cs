using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using static AltTabHud.WinApi;

namespace AltTabHud
{
    public partial class App : Application
    {
        private Timer timer;
        private AcronymExpansionWindow AcronymExpansionWindow;
        bool windowWasClicked = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += (o, O) => Dispatcher.Invoke(OnTimerElapsed);
            timer.Start();

            MainWindow mainWindow = new MainWindow() { Visibility = Visibility.Visible };
            BindEventListeners(mainWindow);

            Current.MainWindow = mainWindow;

            AcronymExpansionWindow = new AcronymExpansionWindow { Visibility = Visibility.Hidden };

            base.OnStartup(e);
        }

        private void BindEventListeners (MainWindow window)
        {
            if (window.WindowStyle == WindowStyle.ToolWindow)
            {
                window.PreviewMouseLeftButtonUp -= Window_PreviewMouseLeftButtonUp;
            }
            else
            {
                window.PreviewMouseLeftButtonUp += Window_PreviewMouseLeftButtonUp;
            }

            window.MouseDoubleClick += WhenWindowIsDoubleClicked;
        }

        private void Window_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            windowWasClicked = true;
        }

        private IntPtr PreviouslyActiveWindow;

        private void OnTimerElapsed()
        {
            var window = Current.MainWindow as MainWindow;
            if (window == null) return;

            var capsLockIsActive = System.Windows.Input.Keyboard.IsKeyToggled(System.Windows.Input.Key.CapsLock);

            if (capsLockIsActive)
            {
                CapsLockControl.PushButton(); // toggle it back off

                if (!AcronymExpansionWindow.IsVisible)
                {
                    PreviouslyActiveWindow = GetForegroundWindow();
                    POINT p = new POINT();
                    GetCursorPos(ref p);
                    RECT rect = new RECT {
                        Top = p.y,
                        Left = p.x
                    };

                    if (GetWindowRect(PreviouslyActiveWindow, ref rect))
                    {
                        AcronymExpansionWindow.Left = rect.Left;
                        AcronymExpansionWindow.Top = rect.Top;
                    }
                    AcronymExpansionWindow.ShowInTaskbar = false;
                    AcronymExpansionWindow.Topmost = true;
                    AcronymExpansionWindow.ShowActivated = true;
                    AcronymExpansionWindow.Show();
                    AcronymExpansionWindow.Focus();
                }
            }
            //else if (AcronymExpansionWindow.IsVisible)
            //{
            //    var expansion = AcronymExpansionWindow.Complete();
            //    SetActiveWindow(PreviouslyActiveWindow);
            //    System.Windows.Forms.SendKeys.SendWait(expansion);
            //}

            if (windowWasClicked)
            {
                Clipboard.SetText(DateTime.Now.ToString("yyyy-MM-dd"));
                window.Notify("OK, copied date to clipboard!");
                windowWasClicked = false; // We've handled this situation now
            }
            else
            {
                window.Invalidate();
            }
        }

        private void WhenWindowIsDoubleClicked(object sender, EventArgs args)
        {
            windowWasClicked = false; // Not clicked, but *double*-clicked!
            var currentWindow = Current.MainWindow;
            MainWindow newWindow;

            if (currentWindow.WindowStyle == WindowStyle.ToolWindow)
            {
                newWindow = new MainWindow() {
                    WindowStyle = WindowStyle.None,
                    Topmost = true,
                    AllowsTransparency = true,
                    Top = currentWindow.Top,
                    Left = currentWindow.Left,
                    Width = currentWindow.Width,
                    Height = currentWindow.Height,
                    Padding = new Thickness(
                        currentWindow.Padding.Left, 
                        currentWindow.Padding.Top + SystemParameters.CaptionHeight, 
                        currentWindow.Padding.Right, 
                        currentWindow.Padding.Bottom + SystemParameters.CaptionHeight)
                };
            }
            else
            {
                newWindow = new MainWindow()
                {
                    WindowStyle = WindowStyle.ToolWindow,
                    Topmost = false,
                    AllowsTransparency = false,
                    Top = currentWindow.Top,
                    Left = currentWindow.Left,
                    Width = currentWindow.Width,
                    Height = currentWindow.Height
                };

            }

            currentWindow.MouseDoubleClick -= WhenWindowIsDoubleClicked;
            currentWindow.Close();

            BindEventListeners(newWindow);
            Current.MainWindow = newWindow;
            newWindow.Show();
        }
    }
}
