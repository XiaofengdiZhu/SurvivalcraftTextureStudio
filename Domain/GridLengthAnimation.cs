using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SurvivalcraftTextureStudio
{
    internal class GridLengthAnimation : AnimationTimeline
    {
        static GridLengthAnimation()
        {
            FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
            ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        }

        public static readonly DependencyProperty FromProperty;

        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        public static readonly DependencyProperty ToProperty;

        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal, toVal;
            GridUnitType fromType, toType;
            if (From.IsAuto)
            {
                GridLength defaultFrom = (GridLength)defaultOriginValue;
                fromVal = defaultFrom.Value;
                fromType = defaultFrom.GridUnitType;
            }
            else
            {
                fromVal = From.Value;
                fromType = From.GridUnitType;
            }
            if (To.IsAuto)
            {
                GridLength defaultTo = (GridLength)defaultDestinationValue;
                toVal = defaultTo.Value;
                toType = defaultTo.GridUnitType;
            }
            else
            {
                toVal = To.Value;
                toType = To.GridUnitType;
            }
            if (fromVal > toVal)
            {
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal,
                    fromType);
            }
            else
                return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal,
                    toType);
        }

        /// <summary>
        /// The <see cref="EasingFunction" /> dependency property's name.
        /// </summary>
        public const string EasingFunctionPropertyName = "EasingFunction";

        /// <summary>
        /// Gets or sets the value of the <see cref="EasingFunction" />
        /// property. This is a dependency property.
        /// </summary>
        public IEasingFunction EasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(EasingFunctionProperty);
            }
            set
            {
                SetValue(EasingFunctionProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="EasingFunction" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register(
            EasingFunctionPropertyName,
            typeof(IEasingFunction),
            typeof(GridLengthAnimation),
            new UIPropertyMetadata(null));
    }
}