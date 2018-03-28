using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarouselView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestVisualisationPage : ContentPage
	{
		public TestVisualisationPage ()
        {
            InitializeComponent();
            var list = new List<string>()
            {
                "g",
                "g",
                "g",
                "g",
                "g",
                "g",
                "g",
                "g",
                "g"
            };
            carousel.ItemsSource = list;
		}
	}
}