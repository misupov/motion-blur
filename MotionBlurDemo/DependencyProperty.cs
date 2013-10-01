using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows;

namespace MotionBlurDemo
{
    public static class DependencyProperty<T> where T : DependencyObject
    {
        public static DependencyProperty Register<TProperty>(Expression<Func<T, TProperty>> propertyExpression, FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None)
        {
            return Register(propertyExpression, default(TProperty), false, null, null, flags);
        }

        public static DependencyProperty Register<TProperty>(
            Expression<Func<T, TProperty>> propertyExpression,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc,
            FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None)
        {
            return Register(propertyExpression, default(TProperty), false, propertyChangedCallbackFunc, null, flags);
        }

        public static DependencyProperty Register<TProperty>
        (
            Expression<Func<T, TProperty>> propertyExpression,
            TProperty defaultValue,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc = null,
            Func<T, CoerceValueCallback<TProperty>> coerceValueCallbackFunc = null,
            FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None
        )
        {
            return Register(propertyExpression, defaultValue, true, propertyChangedCallbackFunc, coerceValueCallbackFunc, flags);
        }

        public static DependencyPropertyKey RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return RegisterReadOnly(propertyExpression, default(TProperty), false, null, null, 0);
        }

        public static DependencyPropertyKey RegisterReadOnly<TProperty>(
            Expression<Func<T, TProperty>> propertyExpression,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc)
        {
            return RegisterReadOnly(propertyExpression, default(TProperty), false, propertyChangedCallbackFunc, null, 0);
        }

        public static DependencyPropertyKey RegisterReadOnly<TProperty>
        (
            Expression<Func<T, TProperty>> propertyExpression,
            TProperty defaultValue,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc = null,
            Func<T, CoerceValueCallback<TProperty>> coerceValueCallbackFunc = null,
            FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None
        )
        {
            return RegisterReadOnly(propertyExpression, defaultValue, true, propertyChangedCallbackFunc, coerceValueCallbackFunc, flags);
        }

        private static DependencyProperty Register<TProperty>
        (
            Expression<Func<T, TProperty>> propertyExpression,
            TProperty defaultValue,
            bool shouldSetDefaultValue,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc,
            Func<T, CoerceValueCallback<TProperty>> coerceValueCallbackFunc,
            FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None
        )
        {
            var propertyName = RetrieveMemberName(propertyExpression);
            var propertyMetadata = GetPropertyMetadata(defaultValue, shouldSetDefaultValue, propertyChangedCallbackFunc, coerceValueCallbackFunc, flags);
            return DependencyProperty.Register(propertyName, typeof(TProperty), typeof(T), propertyMetadata);
        }

        private static DependencyPropertyKey RegisterReadOnly<TProperty>
        (
            Expression<Func<T, TProperty>> propertyExpression,
            TProperty defaultValue,
            bool shouldSetDefaultValue,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc,
            Func<T, CoerceValueCallback<TProperty>> coerceValueCallbackFunc,
            FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None
        )
        {
            var propertyName = RetrieveMemberName(propertyExpression);
            var propertyMetadata = GetPropertyMetadata(defaultValue, shouldSetDefaultValue, propertyChangedCallbackFunc, coerceValueCallbackFunc, flags);
            return DependencyProperty.RegisterReadOnly(propertyName, typeof(TProperty), typeof(T), propertyMetadata);
        }

        private static PropertyMetadata GetPropertyMetadata<TProperty>(TProperty defaultValue,
            bool shouldSetDefaultValue,
            Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc,
            Func<T, CoerceValueCallback<TProperty>> coerceValueCallbackFunc,
            FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.None)
        {
            var propertyChangedCallback = ConvertPropertyChangedCallback(propertyChangedCallbackFunc);
            var coerceValueCallback = ConvertCoerceValueCallback(coerceValueCallbackFunc);

            var propertyMetadata = new FrameworkPropertyMetadata
            {
                AffectsArrange = flags.HasFlag(FrameworkPropertyMetadataOptions.AffectsArrange),
                AffectsMeasure = flags.HasFlag(FrameworkPropertyMetadataOptions.AffectsMeasure),
                AffectsParentArrange = flags.HasFlag(FrameworkPropertyMetadataOptions.AffectsParentArrange),
                AffectsParentMeasure = flags.HasFlag(FrameworkPropertyMetadataOptions.AffectsParentMeasure),
                AffectsRender = flags.HasFlag(FrameworkPropertyMetadataOptions.AffectsRender),
                BindsTwoWayByDefault = flags.HasFlag(FrameworkPropertyMetadataOptions.BindsTwoWayByDefault),
                Inherits = flags.HasFlag(FrameworkPropertyMetadataOptions.Inherits),
                IsNotDataBindable = flags.HasFlag(FrameworkPropertyMetadataOptions.NotDataBindable),
                Journal = flags.HasFlag(FrameworkPropertyMetadataOptions.Journal),
                OverridesInheritanceBehavior = flags.HasFlag(FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior),
                SubPropertiesDoNotAffectRender = flags.HasFlag(FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender)
            };
            if (shouldSetDefaultValue)
            {
                propertyMetadata.DefaultValue = defaultValue;
            }
            if (propertyChangedCallback != null)
            {
                propertyMetadata.PropertyChangedCallback = propertyChangedCallback;
            }
            if (coerceValueCallback != null)
            {
                propertyMetadata.CoerceValueCallback = coerceValueCallback;
            }
            return propertyMetadata;
        }

        [DebuggerStepThrough]
        private static PropertyChangedCallback ConvertPropertyChangedCallback<TProperty>(Func<T, PropertyChangedCallback<TProperty>> propertyChangedCallbackFunc)
        {
            if (propertyChangedCallbackFunc == null)
            {
                return null;
            }
            return (d, e) =>
            {
                var callback = propertyChangedCallbackFunc((T)d);
                if (callback != null)
                {
                    callback(new DependencyPropertyChangedEventArgs<TProperty>(e));
                }
            };
        }

        [DebuggerStepThrough]
        private static CoerceValueCallback ConvertCoerceValueCallback<TProperty>(Func<T, CoerceValueCallback<TProperty>> coerceValueCallbackFunc)
        {
            if (coerceValueCallbackFunc == null)
            {
                return null;
            }
            return (d, e) =>
            {
                var callback = coerceValueCallbackFunc((T)d);
                if (callback != null)
                {
                    return callback((TProperty)e);
                }
                return e;
            };
        }

        private static string RetrieveMemberName<TRes>(Expression<Func<T, TRes>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = propertyExpression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
            }
            if (memberExpression != null)
            {
                var parameterExpression = memberExpression.Expression as ParameterExpression;
                if (parameterExpression != null && parameterExpression.Name == propertyExpression.Parameters[0].Name)
                {
                    return memberExpression.Member.Name;
                }
            }
            throw new ArgumentException("Invalid expression.", "propertyExpression");
        }
    }

    public delegate void PropertyChangedCallback<TProperty>(DependencyPropertyChangedEventArgs<TProperty> e);

    public delegate TProperty CoerceValueCallback<TProperty>(TProperty value);

    public class DependencyPropertyChangedEventArgs<T> : EventArgs
    {
        public DependencyPropertyChangedEventArgs(DependencyPropertyChangedEventArgs e)
        {
            NewValue = (T)e.NewValue;
            OldValue = (T)e.OldValue;
            Property = e.Property;
        }

        public T NewValue { get; private set; }
        public T OldValue { get; private set; }
        public DependencyProperty Property { get; private set; }
    }
}