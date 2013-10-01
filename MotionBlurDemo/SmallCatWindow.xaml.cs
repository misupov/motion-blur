using System.Windows.Input;

namespace MotionBlurDemo
{
    /// <summary>
    /// Interaction logic for SmallCatWindow.xaml
    /// </summary>
    public partial class SmallCatWindow
    {
        public SmallCatWindow()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}