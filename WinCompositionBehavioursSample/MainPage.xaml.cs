using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinCompositionBehavioursSample.Sample;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinCompositionBehavioursSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (ParallaxSample));
        }

        private void BlurButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (BlurSample));
        }
    }
}
