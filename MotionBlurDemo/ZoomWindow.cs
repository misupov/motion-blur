using System;
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
            if (msg == 0x0018) // WM_SHOWWINDOW
            {
                if (wparam.ToInt32() != 0)
                {
                    Animate();
                    handled = true;
                }
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
            var transform = new ScaleTransform(0.0, 0.0, content.DesiredSize.Width*0.5, content.DesiredSize.Height*0.5);
            content.RenderTransform = transform;
            ZoomBlurEffect zoomBlurEffect = null;
            if (MotionBlur)
            {
                zoomBlurEffect = new ZoomBlurEffect();
                zoomBlurEffect.Center = new Point(0.5, 0.5);
            }
            content.Effect = zoomBlurEffect;

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
                    }, DispatcherPriority.Render);
                    t = (Environment.TickCount - animationStartTime) / duration;
                }
            });
        }
    }
}
