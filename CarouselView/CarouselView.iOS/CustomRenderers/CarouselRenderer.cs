using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarouselView;
using CarouselView.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using CarouselView.CustomControls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CarouselRenderer))]
namespace CarouselView.iOS.CustomRenderers
{
    public class CarouselRenderer : ViewRenderer
    {
        private static float SlowDownThreshold = 2f;
        UIScrollView _scrollView;
        bool isCurrentlyTouched = false;
        bool hasSnapped = true;
        bool isScrolling = false;
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            _scrollView = (UIScrollView)NativeView;
            _scrollView.ShowsHorizontalScrollIndicator = false;
            _scrollView.Scrolled += _scrollView_Scrolled; ;
            _scrollView.
        }
    }
}