using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AltTabHud
{
    /// <summary>
    /// Interaction logic for AcronymExpansionWindow.xaml
    /// </summary>
    public partial class AcronymExpansionWindow : Window
    {
        private Timer timer = new Timer();
        private static readonly List<Key> InterestingKeys = new List<Key> {
            Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H,
            Key.I, Key.J, Key.K, Key.L, Key.M, Key.N, Key.O, Key.P,
            Key.Q, Key.R, Key.S, Key.T, Key.U, Key.V, Key.X, Key.Y,
            Key.Z, Key.Back, Key.Escape
        };
        private static readonly Dictionary<string, Func<string>> Acronyms = new Dictionary<string, Func<string>> {
            { "DATE", () => DateTime.Now.ToString("yyyy-MM-dd") }
        };
        private static readonly List<string> ChromeOptions = new List<string>
        {
            "--windows10-custom-titlebar",
            "--profile-directory=Default",
            "--new-window",
            "--window-size=1090,600",
            "--window-position=-10,-30",
            "--window-workspace=2",
            "--utility"
        };

        private StringBuilder acronym = new StringBuilder();

        public AcronymExpansionWindow()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Interval = 40;
            //timer.Start();

            Input.Focus();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            timer.Elapsed -= Timer_Elapsed;
            timer.AutoReset = false;
            timer.Interval = 40;
            //timer.Stop();
            base.OnDeactivated(e);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var key = InterestingKeys.FirstOrDefault(Keyboard.IsKeyDown);
            if (key != default(Key)) 
                acronym.Append(key.ToString());
        }

        public string Complete()
        {
            var content = Content as RichTextBox;
            var acronym = new TextRange(content.Document.ContentStart, content.Document.ContentEnd).Text.Trim();
            var expansion = Acronyms.ContainsKey(acronym) ? Acronyms[acronym]() : acronym;
            content.Document.Blocks.Clear();
            Hide();
            return expansion;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        private void WhenFollowLinkButtonIsClicked(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            CapsLockControl.PushButton();

            var text = Clipboard.GetText();
            if (Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out Uri result))
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    WorkingDirectory = @"C:\Program Files (x86)\Google\Chrome\Application",
                    Arguments = String.Join(" ", ChromeOptions.Append(result.OriginalString))
                };

                Process.Start(startInfo);
            }
        }
    }
}
