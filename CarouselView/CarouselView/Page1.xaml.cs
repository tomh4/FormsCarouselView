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
	public partial class Page1 : ContentPage
	{
		public Page1 ()
		{
			InitializeComponent ();
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