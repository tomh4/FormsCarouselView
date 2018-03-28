using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarouselView;
using CarouselView.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CarouselView.Carousel), typeof(CarouselRenderer))]
namespace CarouselView.iOS.CustomRenderers
{
    public class CarouselRenderer : ViewRenderer
    {
        UILongPressGestureRecognizer longPressGestureRecognizer;
        UISwipeGestureRecognizer swipeGestureRecognizer;
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            longPressGestureRecognizer = new UILongPressGestureRecognizer(() => Console.WriteLine("Long Press"));
            swipeGestureRecognizer = new UISwipeGestureRecognizer()
            
            if (e.NewElement == null)
            {
                if (longPressGestureRecognizer != null)
                {
                    this.RemoveGestureRecognizer(longPressGestureRecognizer);
                }
               if (swipeGestureRecognizer != null)
                {
                    this.RemoveGestureRecognizer(swipeGestureRecognizer);
                }                
            }

            if (e.OldElement == null)
            {
                this.AddGestureRecognizer(longPressGestureRecognizer);
                this.AddGestureRecognizer(swipeGestureRecognizer);
            }
        }

    }
}