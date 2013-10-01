using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MotionBlurDemo
{
    public class ZoomWindow : Window
    {
        public ZoomWindow()
        {
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
        }

        #region [DP] public bool MotionBlur { get; set; }

        public static readonly DependencyProperty MotionBlurProperty = DependencyProperty<ZoomWindow>.Register(
            window => window.MotionBlur,
            true);

        public bool MotionBlur
        {
            get { return (bool)GetValue(MotionBlurProperty); }
            set { SetValue(MotionBlurProperty, value); }
        }

        #endregion

        #region [DP] public TimeSpan AnimationDuration { get; set; }

        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty<ZoomWindow>.Register(
            window => window.AnimationDuration,
            TimeSpan.FromMilliseconds(500),
            coerceValueCallbackFunc: window => window.CoerceAnimationDurationValue);

        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        private TimeSpan CoerceAnimationDurationValue(TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }
            return value;
        }

        #endregion

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == 0x46)
            {
                var mwp = (WindowPos)Marshal.PtrToStructure(lparam, typeof(WindowPos));

                if ((mwp.flags & 0x0040) != 0)
                {
                    Animate();
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void Animate()
        {
            InvalidateVisual();
            var content = (FrameworkElement)Content;
            var easeFunction = new ElasticEase {EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 10};
            content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var duration = AnimationDuration.TotalMilliseconds;
            var actualWidth = content.DesiredSize.Width;
            var actualHeight = content.DesiredSize.Height;
            var transform = new ScaleTransform(0.0, 0.0);
            transform.CenterX = actualWidth*0.5;
            transform.CenterY = actualHeight*0.5;
            content.RenderTransform = transform;
            ZoomBlurEffect zoomBlurEffect = null;
            if (MotionBlur)
            {
                zoomBlurEffect = new ZoomBlurEffect();
                content.Effect = zoomBlurEffect;
                zoomBlurEffect.Center = new Point(0.5, 0.5);
            }
            else
            {
                content.Effect = null;
            }
//            var rtb = new List<RenderTargetBitmap>();

//            var bitmapImage = new BitmapImage(new Uri("test.png", UriKind.Relative));
//            var visual = new Image {Source = bitmapImage, Width = 708};
//            visual.Measure(new Size(708, 632));
//            visual.Arrange(new Rect(0, 0, 708, 632));

            var t = 0.0;
            Task.Run(() =>
            {
                var animationStartTime = Environment.TickCount;
                var prevEase = 0.0;
                while (true)
                {
                    if (t > 1)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            content.LayoutTransform = null;
                            content.Effect = null;
//                            var foo = new RenderTargetBitmap((int)_container.ActualWidth, (int)_container.ActualHeight, 96, 96, PixelFormats.Pbgra32);
//                            foo.Render(visual);
//                            foo.Render(_container);
//                            rtb.Add(foo);
//                            SetWindowPos((PresentationSource.FromVisual(this) as HwndSource).Handle, 0, 
//                                (int) (Left + 50), (int) (Top + 50), (int) (Width - 100), (int) (Height - 100), 0x108);
                        }, DispatcherPriority.Render);
                        break;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        var ease = easeFunction.Ease(t);
                        var blur = ease - prevEase;
                        transform.ScaleX = ease;
                        transform.ScaleY = ease;
                        if (zoomBlurEffect != null && Math.Abs(blur) > 0)
                        {
                            zoomBlurEffect.BlurAmount = blur;
                        }
                        prevEase = ease;
//                        var foo = new RenderTargetBitmap((int)_container.ActualWidth, (int)_container.ActualHeight, 96, 96, PixelFormats.Pbgra32);
//                        foo.Render(visual);
//                        foo.Render(_container);
//                        rtb.Add(foo);
                    }, DispatcherPriority.Render);
//                    t = t + 0.1;
                    t = (Environment.TickCount - animationStartTime) / duration;
                }
                /*int i = 0;
                foreach (var targetBitmap in rtb)
                {
                    Dispatcher.Invoke(() =>
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                        using (var f = File.Create("_test" + i++ + ".png"))
                        {
                            encoder.Save(f);
                            f.Flush();
                        }
                    });
                }*/
            });
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPos
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        };
    }
}
