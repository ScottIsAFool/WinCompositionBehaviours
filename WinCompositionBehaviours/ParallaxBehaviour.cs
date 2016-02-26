using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace WinCompositionBehaviours
{
    public class ParallaxBehaviour : Behavior<UIElement>
    {
        public static readonly DependencyProperty ScrollerProperty = DependencyProperty.Register(
            "Scroller", typeof(FrameworkElement), typeof(ParallaxBehaviour), new PropertyMetadata(default(FrameworkElement), PropertyChanged));

        public FrameworkElement Scroller
        {
            get { return (FrameworkElement)GetValue(ScrollerProperty); }
            set { SetValue(ScrollerProperty, value); }
        }

        public static readonly DependencyProperty MultiplierProperty = DependencyProperty.Register(
            "Multiplier", typeof(float), typeof(ParallaxBehaviour), new PropertyMetadata(0.3f, PropertyChanged));

        public float Multiplier
        {
            get { return (float)GetValue(MultiplierProperty); }
            set { SetValue(MultiplierProperty, value); }
        }

        public static readonly DependencyProperty IsHorizontalEffectProperty = DependencyProperty.Register(
            "IsHorizontalEffect", typeof(bool), typeof(ParallaxBehaviour), new PropertyMetadata(default(bool), PropertyChanged));

        public bool IsHorizontalEffect
        {
            get { return (bool)GetValue(IsHorizontalEffectProperty); }
            set { SetValue(IsHorizontalEffectProperty, value); }
        }

        private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ParallaxBehaviour)sender)?.SetBehavior();
        }

        private void SetBehavior()
        {
            if (Scroller == default(FrameworkElement))
            {
                return;
            }

            var scroller = Scroller as ScrollViewer;
            if (scroller == null)
            {
                scroller = GetChildOfType<ScrollViewer>(Scroller);
                if (scroller == null)
                {
                    return;
                }
            }

            CompositionPropertySet scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroller);

            Compositor compositor = scrollerViewerManipulation.Compositor;

            var manipulationProperty = IsHorizontalEffect ? "X" : "Y";
            ExpressionAnimation expression = compositor.CreateExpressionAnimation($"ScrollManipululation.Translation.{manipulationProperty} * ParallaxMultiplier");

            expression.SetScalarParameter("ParallaxMultiplier", Multiplier);
            expression.SetReferenceParameter("ScrollManipululation", scrollerViewerManipulation);

            Visual textVisual = ElementCompositionPreview.GetElementVisual((UIElement)AssociatedObject);
            textVisual.StartAnimation($"Offset.{manipulationProperty}", expression);
        }

        private static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }

            return null;
        }
    }
}
