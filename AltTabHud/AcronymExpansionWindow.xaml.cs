using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace AltTabHud
{
    public partial class AcronymExpansionWindow : Window
    {
        public event Action<string> Completed = _ => { };

        private static readonly Dictionary<string, Func<string>> Acronyms = new Dictionary<string, Func<string>> {
            { "date", () => DateTime.Now.ToString("yyyy-MM-dd") }
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

        public AcronymExpansionWindow()
        {
            InitializeComponent();
        }

        public string Complete()
        {
            var acronym = new TextRange(Input.Document.ContentStart, Input.Document.ContentEnd).Text.Trim();
            var expansion = Acronyms.ContainsKey(acronym) ? Acronyms[acronym]() : acronym;
            Input.Document.Blocks.Clear();
            Visibility = Visibility.Hidden;
            return expansion;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Visibility = Visibility.Hidden;
                return;
            }

            if (e.Key == Key.Enter)
            {
                Completed(Complete());
                return;
            }
        }

        private void WhenFollowLinkButtonIsClicked(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;

            var text = Clipboard.GetText();
            if (Uri.TryCreate(text, UriKind.Absolute, out Uri result))
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
