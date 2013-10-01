using System.Windows.Input;

namespace MotionBlurDemo
{
    /// <summary>
    /// Interaction logic for BigCatWindow.xaml
    /// </summary>
    public partial class BigCatWindow
    {
        public BigCatWindow()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}