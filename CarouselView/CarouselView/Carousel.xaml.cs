using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarouselView
{
	public partial class Carousel : ContentView
	{
        private double carouselWidth, carouselScrollPosition;
        public TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
        private int carouselPagesCount;
        public static readonly BindableProperty SnappingProperty =
            BindableProperty.Create(propertyName: "Snapping",
            returnType: typeof(bool),
            declaringType: typeof(Carousel),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);

        public bool Snapping
        {
            get { return (bool)GetValue(SnappingProperty); }
            set { SetValue(SnappingProperty, value); }
        }
        public static readonly BindableProperty SnapPositionProperty =
           BindableProperty.Create(propertyName: "SnapPosition",
           returnType: typeof(float),
           declaringType: typeof(Carousel),
           defaultValue: 0f,
           defaultBindingMode: BindingMode.OneWay);

        public float SnapPosition
        {
            get { return (float)GetValue(SnapPositionProperty); }
            set { SetValue(SnapPositionProperty, (value < 0) ? 0 : (value > 1f) ? 1f : value); }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(propertyName: "ItemsSource",
            returnType: typeof(IEnumerable),
            declaringType: typeof(Carousel),
            defaultValue: default(IEnumerable),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnItemSourceChanged);

        private static void OnItemSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Carousel)bindable;
            control.Render(oldValue,newValue);
        }
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(propertyName: "ItemTemplate",
            returnType: typeof(DataTemplate),
            declaringType: typeof(Carousel),
            defaultValue: default(DataTemplate),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnItemTemplateChanged);
        private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Carousel)bindable;
            control.Render(null, null, true);
        }
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public Carousel()
		{
			InitializeComponent();
            carouselScrollView.carouselParent = this;
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
        }
        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            if (child is View childview)
            {
                childview.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
        }

        public void Render(object oldItemsSource, object newItemsSource,bool completeReRender = false)
        {
            if (this.ItemTemplate == null || this.ItemsSource == null)
                return;

            var layout = new CustomStackLayout();
            layout.carouselParent = this;
            layout.Orientation = StackOrientation.Horizontal;
            carouselPagesCount = 0;
            carouselWidth = 0;
            carouselScrollPosition = 0;
            foreach (var item in this.ItemsSource)
            {
                carouselPagesCount++;
                var viewCell = this.ItemTemplate.CreateContent() as ViewCell;
                viewCell.View.BindingContext = item;
                layout.Children.Add(viewCell.View);
            }
            carouselScrollView.Content = layout;
            carouselScrollView.Scrolled += CarouselScrollView_Scrolled;
        }
        public async Task Snap()
        {
            var carouselContentViewSize = ((carouselWidth - 2*carouselScrollView.placeHolderOffset - ((carouselPagesCount-1)*carouselScrollView.spacing)) / (carouselPagesCount));
            if (carouselScrollPosition < carouselScrollView.placeHolderOffset)
            {
                var startPosition = carouselScrollView.scrollViewWidth * SnapPosition - carouselContentViewSize / 2;
                if (startPosition < 0)
                {
                    startPosition = 0;
                }
                await carouselScrollView.ScrollToAsync(carouselScrollView.placeHolderOffset-startPosition, 0, true);
            }
            else
            {
                var endPosition = carouselScrollView.placeHolderOffset;
                endPosition += (carouselPagesCount - 1) * (carouselContentViewSize+carouselScrollView.spacing);
                var snapOffset = carouselScrollView.scrollViewWidth * SnapPosition - carouselContentViewSize / 2;
                if (snapOffset < 0)
                {
                    snapOffset = 0;
                }
                endPosition -= snapOffset;
                if (carouselScrollPosition > endPosition)
                {
                    await carouselScrollView.ScrollToAsync(endPosition, 0, true);
                }
                else
                {
                    if (Snapping)
                    {
                        snapOffset = carouselScrollView.scrollViewWidth * SnapPosition - carouselContentViewSize / 2;
                        if (snapOffset < 0)
                        {
                            snapOffset = 0;
                        }
                        var desiredPosition = carouselScrollPosition - carouselScrollView.placeHolderOffset + snapOffset;
                        desiredPosition = (carouselContentViewSize+ carouselScrollView.spacing) * Math.Round((desiredPosition / (carouselContentViewSize+ carouselScrollView.spacing)));
                        desiredPosition -= snapOffset;
                        await carouselScrollView.ScrollToAsync(carouselScrollView.placeHolderOffset + desiredPosition, 0, true);
                    }
                }
            }            
        }
        private void CarouselScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            carouselScrollPosition = e.ScrollX;
            carouselWidth = carouselScrollView.ContentSize.Width;
        }
    }
    public class CustomScrollView : ScrollView
    {
        public Carousel carouselParent;
        public double placeHolderOffset,spacing;
        public double scrollViewWidth;
        private bool initialised = false;
        private async void InitScrollView()
        {
            var layout = (CustomStackLayout)Content;
            var childViewSize = layout.Children[0].Width;
            spacing = layout.Spacing;
            placeHolderOffset = 0;
            var placeHolderCount = (int)Math.Ceiling(scrollViewWidth / childViewSize);
            for(int i = 0; i < placeHolderCount; i++)
            {
                placeHolderOffset += (childViewSize + spacing);
                var startPlaceHolder = carouselParent.ItemTemplate.CreateContent() as ViewCell;
                startPlaceHolder.View.Opacity = 0;
                startPlaceHolder.View.InputTransparent = true;
                var endPlaceHolder = carouselParent.ItemTemplate.CreateContent() as ViewCell;
                endPlaceHolder.View.Opacity = 0;
                endPlaceHolder.View.InputTransparent = true;
                layout.Children.Insert(0, startPlaceHolder.View);
                layout.Children.Add(endPlaceHolder.View);
            }
            var startPosition = scrollViewWidth * carouselParent.SnapPosition - childViewSize / 2;
            if (startPosition < 0)
            {
                startPosition = 0;
            }
            await ScrollToAsync(placeHolderOffset - startPosition, 0, false);
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(width > 0 && !initialised)
            {
                initialised = true;
                scrollViewWidth = width;
                InitScrollView();
            }
        }
    }
    public class CustomStackLayout : StackLayout
    {
        public Carousel carouselParent;
        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            if (child is View childview)
            {
                childview.GestureRecognizers.Add(carouselParent.tapGestureRecognizer);
            }
        }
    }
}
