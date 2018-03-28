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
        private double previousScrollPosition = 0d;
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
            var desiredPosition = (carouselWidth / carouselPagesCount) * Math.Round(carouselScrollPosition / (carouselWidth / carouselPagesCount));
            if (Snapping)
            {
                await carouselScrollView.ScrollToAsync(desiredPosition, 0, true);                
            }
        }
        private void CarouselScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (Snapping)
            {
                carouselWidth = ((ScrollView)sender).Width;
                carouselScrollPosition = e.ScrollX;
            }
            previousScrollPosition = e.ScrollX;
        }
    }
    public class CustomScrollView : ScrollView
    {
        public Carousel carouselParent;
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
