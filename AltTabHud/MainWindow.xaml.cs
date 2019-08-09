using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace AltTabHud
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            //SelectedBackground = Application.Current.FindResource("NormalBackground") as LinearGradientBrush;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string DateField { get; set; }
        public string TimeField { get; set; }
        public string WeekField { get; set; }
        public DateTime ChillUntil { get; private set; }
        public Brush SelectedBackground { get; private set; }

        public void Invalidate()
        {
            var utcnow = DateTime.UtcNow;
            if (utcnow < ChillUntil)
                return;

            var now = DateTime.Now;
            var weekday = CultureInfo.CurrentUICulture.Calendar
                .GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            DateField = $"{now:dddd}en den {now:dd} {now:MMMM}";
            TimeField = $"kl. {now:HH:mm}";
            WeekField = $"vecka {weekday.ToString()}";

            SelectedBackground = Application.Current.FindResource("NormalBackground") as Brush;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(DateField)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeField)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(WeekField)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedBackground)));
        }

        public void Notify(string message)
        {
            WeekField = "";
            TimeField = "";

            DateField = message;

            SelectedBackground = Application.Current.FindResource("HighlightedBackground") as LinearGradientBrush;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(DateField)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeField)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(WeekField)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedBackground)));

            ChillUntil = DateTime.UtcNow + TimeSpan.FromSeconds(1);
        }
    }
}
