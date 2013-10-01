using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MotionBlurDemo
{
    /// <summary>An effect that applies a radial blur to the input.</summary>
    public class ZoomBlurEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof (ZoomBlurEffect), 0);

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof (Point), typeof (ZoomBlurEffect),
            new UIPropertyMetadata(new Point(0.9D, 0.6D), PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty BlurAmountProperty =
            DependencyProperty.Register("BlurAmount", typeof (double), typeof (ZoomBlurEffect),
            new UIPropertyMetadata(0.1D, PixelShaderConstantCallback(1)));

        public ZoomBlurEffect()
        {
            PixelShader = new PixelShader
            {
                UriSource = new Uri(@"pack://application:,,,/ZoomBlurEffect.ps", UriKind.Absolute)
            };

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(CenterProperty);
            UpdateShaderValue(BlurAmountProperty);
        }

        public Brush Input
        {
            get { return ((Brush) (GetValue(InputProperty))); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>The center of the blur.</summary>
        public Point Center
        {
            get { return ((Point) (GetValue(CenterProperty))); }
            set { SetValue(CenterProperty, value); }
        }
        
        /// <summary>The amount of blur.</summary>
        public double BlurAmount
        {
            get { return ((double) (GetValue(BlurAmountProperty))); }
            set { SetValue(BlurAmountProperty, value); }
        }
    }
}