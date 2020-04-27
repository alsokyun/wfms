using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace GTIFramework.Common.Utils.ViewEffect
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty from;
        public static readonly DependencyProperty to;

        static GridLengthAnimation()
        {
            from = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
            to = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.from);
            }
            set
            {
                SetValue(GridLengthAnimation.from, value);
            }
        }

        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.to);
            }
            set
            {
                SetValue(GridLengthAnimation.to, value);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double FromValue = ((GridLength)GetValue(GridLengthAnimation.from)).Value;
            double ToValue = ((GridLength)GetValue(GridLengthAnimation.to)).Value;

            if (FromValue > ToValue)
            {
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (FromValue - ToValue) + ToValue, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
            else
            {
                return new GridLength((animationClock.CurrentProgress.Value) * (ToValue - FromValue) + FromValue, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
        }
    }
}
