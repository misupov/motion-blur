using System;
using System.Windows;

namespace MotionBlurDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SmallCatWindow _smallCatCatWindow;
        private BigCatWindow _bigCatWindow;
        private SimpleUiWindow _simpleUiWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowCat1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (_smallCatCatWindow != null)
            {
                _smallCatCatWindow.Close();
            }
            _smallCatCatWindow = new SmallCatWindow();
            _smallCatCatWindow.MotionBlur = UseMotionBlur.IsChecked == true;
            _smallCatCatWindow.AnimationDuration = TimeSpan.FromMilliseconds(DurationScrollBar.Value);
            _smallCatCatWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _smallCatCatWindow.Show();
        }

        private void HideCat1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (_smallCatCatWindow != null)
            {
                _smallCatCatWindow.Close();
                _smallCatCatWindow = null;
            }
        }

        private void ShowCat2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (_bigCatWindow != null)
            {
                _bigCatWindow.Close();
            }
            _bigCatWindow = new BigCatWindow();
            _bigCatWindow.MotionBlur = UseMotionBlur.IsChecked == true;
            _bigCatWindow.AnimationDuration = TimeSpan.FromMilliseconds(DurationScrollBar.Value);
            _bigCatWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _bigCatWindow.Show();
        }

        private void HideCat2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (_bigCatWindow != null)
            {
                _bigCatWindow.Close();
                _bigCatWindow = null;
            }
        }

        private void ShowSimpleUiButtonClick(object sender, RoutedEventArgs e)
        {
            if (_simpleUiWindow != null)
            {
                _simpleUiWindow.Close();
            }
            _simpleUiWindow = new SimpleUiWindow();
            _simpleUiWindow.MotionBlur = UseMotionBlur.IsChecked == true;
            _simpleUiWindow.AnimationDuration = TimeSpan.FromMilliseconds(DurationScrollBar.Value);
            _simpleUiWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _simpleUiWindow.Show();
        }

        private void HideSimpleUiButtonClick(object sender, RoutedEventArgs e)
        {
            if (_simpleUiWindow != null)
            {
                _simpleUiWindow.Close();
                _simpleUiWindow = null;
            }
        }
    }
}
