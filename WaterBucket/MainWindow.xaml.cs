using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WaterBucket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimeSpan remind_time;
        private bool tracking;
        private CancellationTokenSource source;

        public MainWindow()
        {
            InitializeComponent();
            remind_time = new TimeSpan(0,0,30);
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CalculateRainTimer()
        {
            DateTime now = DateTime.Now;
            DateTime next = new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute >= 27 && now.Minute <= 57 ? 58 : 28, now.Second);
            TimeSpan delay = (next - remind_time) - now;
            //MessageBox.Show(delay.ToString());
            source = new CancellationTokenSource();
            tracking = true;
            Task task = Task.Delay(delay, source.Token).ContinueWith(_ =>
            {
                if (tracking)
                {
                    MediaPlayer player = new MediaPlayer();
                    player.Open(new Uri("chaching.mp3", UriKind.RelativeOrAbsolute));
                    player.Play();
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https:rustclash.com/cases",
                        UseShellExecute = true
                    });
                    tracking = false;
                    CalculateRainTimer();
                    return;
                }
            });
        }
        private void startButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tracking)
            {
                CalculateRainTimer();
                startButton_Text.Text = "Stop Collecting!";
            } else
            {
                source.Cancel();
                tracking = false;
                startButton_Text.Text = "Start Collecting!";
            }
        }
    }
}