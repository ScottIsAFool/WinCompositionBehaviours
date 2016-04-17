using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Xaml.Interactivity;

namespace WinCompositionBehaviours
{
    public class BlurBehaviour : Behavior<UIElement>
    {
        private Compositor _compositor;
        private SpriteVisual _blurVisual;
        private CompositionEffectBrush _blurBrush;
        
        public static readonly DependencyProperty BlurValueProperty = DependencyProperty.Register(
            nameof(BlurValue), typeof (float), typeof (BlurBehaviour), new PropertyMetadata(30.0f, OnPropertyChanged));

        public float BlurValue
        {
            get { return (float) GetValue(BlurValueProperty); }
            set { SetValue(BlurValueProperty, value); }
        }

        public static readonly DependencyProperty FadeValueProperty = DependencyProperty.Register(
            nameof(FadeValue), typeof (float), typeof (BlurBehaviour), new PropertyMetadata(1.0f, OnPropertyChanged));

        public float FadeValue
        {
            get { return (float) GetValue(FadeValueProperty); }
            set { SetValue(FadeValueProperty, value); }
        }

        public static readonly DependencyProperty ColorEffectProperty = DependencyProperty.Register(
            nameof(ColorEffect), typeof (Color), typeof (BlurBehaviour), new PropertyMetadata(Colors.Black, OnPropertyChanged));

        public Color ColorEffect
        {
            get { return (Color) GetValue(ColorEffectProperty); }
            set { SetValue(ColorEffectProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BlurBehaviour)?.SetBlur();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            SetBlur();
        }

        private void SetBlur()
        {
            var backingVisual = ElementCompositionPreview.GetElementVisual(AssociatedObject);
            _compositor = backingVisual.Compositor;
            _blurBrush = BuildColoredBlurMixerBrush();
            _blurBrush.SetSourceParameter("source", _compositor.CreateDestinationBrush());

            _blurVisual = _compositor.CreateSpriteVisual();
            _blurVisual.Brush = _blurBrush;

            _blurVisual.Properties.InsertScalar("BlurValue", BlurValue);
            _blurVisual.Properties.InsertScalar("FadeValue", FadeValue);

            var blurAnimator = _compositor.CreateExpressionAnimation();
            blurAnimator.SetReferenceParameter("bluramount", _blurVisual);
            blurAnimator.Expression = "bluramount.BlurValue";
            _blurBrush.StartAnimation("Blur.BlurAmount", blurAnimator);

            var container = _compositor.CreateContainerVisual();
            container.Children.InsertAtTop(_blurVisual);

            ElementCompositionPreview.SetElementChildVisual(AssociatedObject, container);
        }

        private CompositionEffectBrush BuildColoredBlurMixerBrush()
        {
            var arithmeticComposit = new ArithmeticCompositeEffect
            {
                Name = "Mixer",
                Source1Amount = 0.0f,
                Source2Amount = 0.0f,
                MultiplyAmount = 0,
                Source2 = new ColorSourceEffect
                {
                    Name = "ColorSource",
                    Color = ColorEffect
                },
                Source1 = new BlendEffect
                {
                    Mode = BlendEffectMode.Multiply,

                    Foreground = new ColorSourceEffect
                    {
                        Name = "ColorSource2",
                        Color = ColorEffect
                    },
                    Background = new GaussianBlurEffect
                    {
                        Name = "Blur",
                        Source = new CompositionEffectSourceParameter("source"),
                        BlurAmount = 0.0f, //15
                        BorderMode = EffectBorderMode.Hard,
                        Optimization = EffectOptimization.Balanced
                    }
                }
            };

            var factory = _compositor.CreateEffectFactory(arithmeticComposit, new string[] { "Blur.BlurAmount", "Mixer.Source1Amount", "Mixer.Source2Amount" });

            var brush = factory.CreateBrush();

            brush.Properties.InsertScalar("Blur.BlurAmount", 0.0f);
            brush.Properties.InsertScalar("Mixer.Source1Amount", 1.0f);
            brush.Properties.InsertScalar("Mixer.Source2Amount", 0.0f);

            return brush;
        }
    }
}
