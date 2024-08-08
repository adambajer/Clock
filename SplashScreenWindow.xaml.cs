using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Clock
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
            StartFadeIn();
        }

        private async void StartFadeIn()
        {
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            fadeInAnimation.Completed += async (s, e) =>
            {
                await Task.Delay(2000); // Wait for 2 seconds
                StartCrossFade();
            };

            BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private async void StartCrossFade()
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1)
            };

            BeginAnimation(OpacityProperty, fadeOutAnimation);

            // Add a blur effect to the SplashScreenWindow
            var blurEffect = new BlurEffect();
            Effect = blurEffect;

            await Task.Delay(1000); // Wait for fade-out animation to complete

            // Create and show the MainWindow
            var mainWindow = new MainWindow();
            mainWindow.WindowStyle = WindowStyle.None;
            mainWindow.AllowsTransparency = true;
            mainWindow.Background = Brushes.Transparent;
            mainWindow.Loaded += async (s, e) =>
            {
                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(1)
                };

                mainWindow.BeginAnimation(OpacityProperty, fadeInAnimation);

                await Task.Delay(1000); // Wait for fade-in animation to complete

                // Remove the blur effect from the SplashScreenWindow
                Effect = null;

                Close(); // Close the SplashScreenWindow
            };

            mainWindow.Show();
        }
    }
}
