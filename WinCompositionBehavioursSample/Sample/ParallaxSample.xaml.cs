using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinCompositionBehavioursSample.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ParallaxSample : Page
    {
        public ParallaxSample()
        {
            this.InitializeComponent();

            var list = new List<string>();
            for (var i = 1; i < 100; i++)
            {
                list.Add(i.ToString());
            }

            ItemsList.ItemsSource = list;
        }
    }
}
