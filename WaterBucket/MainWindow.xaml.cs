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
using System.Windows.Threading;

namespace WaterBucket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimeSpan remind_time;
        private bool tracking;
        private CancellationTokenSource? source;
        private bool playAudio = true;
        private bool visit = true;
        DateTime next;
        TimeSpan delay;

        public MainWindow()
        {
            InitializeComponent();
            remind_time = new TimeSpan(0,0,30);
            DateTime now = DateTime.Now;
            next = new DateTime(now.Year, now.Month, now.Day, (now.Minute < 57 ? now.Hour : now.Hour + 1), now.Minute >= 27 && now.Minute < 57 ? 58 : 28, now.Second);
            delay = (next - remind_time) - now;
            countdown_Tick(null, new EventArgs());
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(countdown_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private async void CalculateRainTimer()
        {
            DateTime now = DateTime.Now;
            next = new DateTime(now.Year,now.Month,now.Day, (now.Minute < 57 ? now.Hour:now.Hour+1),now.Minute >= 27 && now.Minute < 57 ? 58 : 28, now.Second);
            //MessageBox.Show(next.ToString());
            delay = (next - remind_time) - now;
            TimeSpan wd = (next - remind_time) - now;
            //MessageBox.Show(delay.ToString());
            source = new CancellationTokenSource();
            tracking = true;
            Task task = Task.Delay(wd, source.Token).ContinueWith(delegate
            {
                if (tracking)
                {
                    if (playAudio)
                    {
                        MediaPlayer player = new MediaPlayer();
                        player.Open(new Uri("chaching.mp3", UriKind.RelativeOrAbsolute));
                        player.Play();
                    }
                    if (visit)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://rustclash.com/cases",
                            UseShellExecute = true
                        });
                    }
                    return;
                }
            });
            await task;
            if (tracking) CalculateRainTimer();
        }
        private void startButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tracking)
            {
                CalculateRainTimer();
                startButton_Text.Text = "Stop Collecting!";
            } else
            {
                tracking = false;
                if (source != null) source.Cancel();
                startButton_Text.Text = "Start Collecting!";
            }
        }

        private void webpageToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            visit ^= true;
            webpageToggle_Text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(visit ? "#474747" : "#f78c6c"));
            return;
        }

        private void noiseToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            playAudio ^= true;
            noiseToggle_Text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(playAudio ? "#474747" : "#f78c6c"));
            return;
        }

        private void countdown_Tick(object sender, EventArgs args)
        {
            countdown.Content = "00:" + (delay.Minutes>=10?delay.Minutes:"0"+delay.Minutes) + ":" + (delay.Seconds>=10?delay.Seconds:"0"+delay.Seconds);
            delay = delay.Subtract(new TimeSpan(0, 0, 1));
        }
    }
}