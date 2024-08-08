using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Clock
{
   
   
    public partial class MainWindow : Window
    {
       

        private TaskbarIcon taskbarIcon; // Add this field

        private int _currentColorIndex = 0;
        private int _totalColors = 256;
        private List<Color> _colorList = new List<Color>();
        public MainWindow()
        {
   
            InitializeComponent();
            StartGradientAnimation();

            taskbarIcon = new TaskbarIcon();
            taskbarIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/splash.ico"));
            taskbarIcon.Visibility = Visibility.Hidden;
            taskbarIcon.TrayLeftMouseUp += TaskbarIcon_Click;

            minbut.Click += MinimizeToTray_Click;
            winclick.Click += Win_Click;
            closeclick.Click += Close_Click;
            this.KeyDown += Window_KeyDown;
            GenerateColorList();
 
            DateTime date = DateTime.Now;
            CultureInfo csCZ = new CultureInfo("cs-CZ");
            string formattedDate = date.ToString("dddd d.M.yyyy", csCZ);
            CurrentDate.Text = formattedDate;
            StartClock();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MinimizeToTray_Click(sender, e);
            }
        }
        public void MinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            // Minimize the window
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
            // Show the TaskbarIcon in the system tray
            taskbarIcon.Visibility = Visibility.Visible;
        }


        private void TaskbarIcon_Click(object sender, RoutedEventArgs e)
        {
            // Restore the window to fullscreen
            WindowState = WindowState.Maximized;
            ShowInTaskbar = false;

            // Hide the TaskbarIcon
            taskbarIcon.Visibility = Visibility.Hidden;

            // Show the context menu on right-click
             
        }
        public void Close_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Close the window
        }
        public void Win_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal; // Turn off fullscreen mode
            Width = 400; // Set the width to 400px
            Height = 200; // Set the height to 200px
            ResizeMode = ResizeMode.CanResize; // Allow resizing

            // Scale down the contents of the window
            var scaleTransform = new ScaleTransform(0.1, 0.1); // Set the scale factor as desired
            foreach (var element in ((Grid)Content).Children)
            {
                if (element is UIElement uiElement)
                {
                    uiElement.RenderTransform = scaleTransform;
                }
            }

            // Enable drag and drop functionality
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
{
    if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
    {
        DragMove();
    }
}


        private void StartGradientAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1; // Initial value for the endpoints
            animation.To = 0; // Final value for the endpoints
            animation.Duration = TimeSpan.FromSeconds(2); // Animation duration (adjust as needed)
            animation.AutoReverse = true; // Repeat the animation in reverse
            animation.RepeatBehavior = RepeatBehavior.Forever; // Repeat the animation indefinitely

            Storyboard.SetTarget(animation, gradientTransform);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
        private void GenerateColorList()
        {
            for (int i = 0; i < _totalColors; i++)
            {
                double hue = (double)i / _totalColors * 360;
                Color color = ColorHelper.FromHsl(hue, 1, 0.5);
                _colorList.Add(color);
            }
        }

        private void StartClock()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            clockText.Text = currentTime.ToString("HH:mm:ss");
            AnimateBackgroundColor();
        }

        private void AnimateBackgroundColor()
        {
            Color startColor = _colorList[_currentColorIndex];
            Color endColor = _colorList[(_currentColorIndex + 1) % _totalColors];

            SolidColorBrush brush = new SolidColorBrush(startColor);
            canvas.Background = brush;
            clockText.Foreground = brush;

            ColorAnimation animation = new ColorAnimation(endColor, TimeSpan.FromSeconds(1));
            animation.Completed += (s, e) =>
            {
                _currentColorIndex = (_currentColorIndex + 1) % _totalColors;
                AnimateBackgroundColor();
            };

            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        } 

    public static class ColorHelper
    {
        public static Color FromHsl(double hue, double saturation, double lightness)
        {
            double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double x = chroma * (1 - Math.Abs(hue / 60 % 2 - 1));
            double m = lightness - chroma / 2;
            double red = 0, green = 0, blue = 0;
            if (hue < 60)
            {
                red = chroma;
                green = x;
            }
            else if (hue < 120)
            {
                red = x;
                green = chroma;
            }
            else if (hue < 180)
            {
                green = chroma;
                blue = x;
            }
            else if (hue < 240)
            {
                green = x;
                blue = chroma;
            }
            else if (hue < 300)
            {
                red = x;
                blue = chroma;
            }
            else if (hue < 360)
            {
                red = chroma;
                blue = x;
            }
            byte r = (byte)((red + m) * 255);
            byte g = (byte)((green + m) * 255);
            byte b = (byte)((blue + m) * 255);
            return Color.FromRgb(r, g, b);
        }
    }
    }
   

}
