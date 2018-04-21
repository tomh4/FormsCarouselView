using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CarouselView.CustomControls;
using CarouselView.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CarouselRenderer))]
namespace CarouselView.Droid.CustomRenderers
{
    public class CarouselRenderer : ScrollViewRenderer
    {
        public CarouselRenderer() : base(Android.App.Application.Context)
        {

        }
        HorizontalScrollView _scrollView;



        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            e.NewElement.PropertyChanged += ElementPropertyChanged;
        }
        

        void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer")
            {
                _scrollView = (HorizontalScrollView)typeof(ScrollViewRenderer)
                    .GetField("_hScrollView", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this);
                _scrollView.HorizontalScrollBarEnabled = false;
                _scrollView.Touch += _scrollView_Touch;

            }

        }

        private async void _scrollView_Touch(object sender, TouchEventArgs e)
        {
            e.Handled = false;
            switch (e.Event.Action)
            {
                case MotionEventActions.Up:
                    await ((CustomScrollView)Element).carouselParent.Snap();
                    break;

            }
        }
    }
}    
