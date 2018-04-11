using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CarouselView.CustomControls;

namespace CarouselView
{
    public partial class Carousel : ContentView
    {
        private double carouselWidth, carouselScrollPosition, carouselContentViewSize = 0;
        private DotButtonsLayout dotLayout;
        public TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
        private int carouselPagesCount;
        #region SnappingProperties
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
        #endregion
        #region IndicatorsProperties
        public static readonly BindableProperty IndicatorsProperty =
            BindableProperty.Create(propertyName: "Indicators",
            returnType: typeof(bool),
            declaringType: typeof(Carousel),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);
        public bool Indicators
        {
            get { return (bool)GetValue(IndicatorsProperty); }
            set { SetValue(IndicatorsProperty, value); }
        }

        public static readonly BindableProperty IndicatorsAboveCarouselProperty =
            BindableProperty.Create(propertyName: "IndicatorsAboveCarousel",
            returnType: typeof(bool),
            declaringType: typeof(Carousel),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);
        public bool IndicatorsAboveCarousel
        {
            get { return (bool)GetValue(IndicatorsAboveCarouselProperty); }
            set { SetValue(IndicatorsAboveCarouselProperty, value); }
        }

        public static readonly BindableProperty IndicatorsColorProperty =
            BindableProperty.Create(propertyName: "IndicatorsColor",
            returnType: typeof(Color),
            declaringType: typeof(Carousel),
            defaultValue: Color.White,
            defaultBindingMode: BindingMode.OneWay);
        public Color IndicatorsColor
        {
            get { return (Color)GetValue(IndicatorsColorProperty); }
            set { SetValue(IndicatorsColorProperty, value); }
        }

        public static readonly BindableProperty IndicatorsSizeProperty =
            BindableProperty.Create(propertyName: "IndicatorsSize",
            returnType: typeof(int),
            declaringType: typeof(Carousel),
            defaultValue: 10,
            defaultBindingMode: BindingMode.OneWay);
        public int IndicatorsSize
        {
            get { return (int)GetValue(IndicatorsSizeProperty); }
            set { SetValue(IndicatorsSizeProperty, value); }
        }
        #endregion
        #region ItemSourceProperties
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
            control.Render(oldValue, newValue);
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
        #endregion
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

        public void Render(object oldItemsSource, object newItemsSource, bool completeReRender = false)
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
            //Create indicators and place them abover or below the carousel
            if (Indicators)
            {
                dotLayout = new DotButtonsLayout(carouselPagesCount, IndicatorsColor, IndicatorsSize);
                foreach (DotButton dot in dotLayout.dots)
                    dot.Clicked += DotClicked;
            }
        }
        //The function called by the buttons clicked event
        private async void DotClicked(object sender)
        {
            var button = (DotButton)sender;
            //Get the selected buttons index
            int index = button.index;
            //Set the corresponding page as position of the carousel view
            var snapOffset = carouselScrollView.scrollViewWidth * SnapPosition - carouselContentViewSize / 2;
            if (snapOffset < 0)
            {
                snapOffset = 0;
            }
            var desiredPosition = carouselScrollView.placeHolderOffset + (carouselContentViewSize + carouselScrollView.spacing) * index;
            desiredPosition -= snapOffset;
            await carouselScrollView.ScrollToAsync(desiredPosition, 0, true);
        }

        public async Task Snap()
        {
            if (carouselScrollPosition < carouselScrollView.placeHolderOffset)
            {
                var startPosition = carouselScrollView.scrollViewWidth * SnapPosition - carouselContentViewSize / 2;
                if (startPosition < 0)
                {
                    startPosition = 0;
                }
                await carouselScrollView.ScrollToAsync(carouselScrollView.placeHolderOffset - startPosition, 0, true);
            }
            else
            {
                var endPosition = carouselScrollView.placeHolderOffset;
                endPosition += (carouselPagesCount - 1) * (carouselContentViewSize + carouselScrollView.spacing);
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
                        desiredPosition = (carouselContentViewSize + carouselScrollView.spacing) * Math.Round((desiredPosition / (carouselContentViewSize + carouselScrollView.spacing)));
                        desiredPosition -= snapOffset;
                        await carouselScrollView.ScrollToAsync(carouselScrollView.placeHolderOffset + desiredPosition, 0, true);
                    }
                }
            }
        }
        private void CarouselScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (carouselWidth != carouselScrollView.ContentSize.Width)
            {
                carouselWidth = carouselScrollView.ContentSize.Width;
                carouselContentViewSize = ((carouselWidth - 2 * carouselScrollView.placeHolderOffset - ((carouselPagesCount - 1) * carouselScrollView.spacing)) / (carouselPagesCount));
            }
            var previousPosition = Math.Round((carouselScrollPosition - carouselScrollView.placeHolderOffset) / (carouselContentViewSize + carouselScrollView.spacing));
            carouselScrollPosition = e.ScrollX;
            var currentPosition = Math.Round((carouselScrollPosition - carouselScrollView.placeHolderOffset) / (carouselContentViewSize + carouselScrollView.spacing));
            if (previousPosition != currentPosition)
            {
                for (int i = 0; i < dotLayout.dots.Length; i++)
                    if (currentPosition == i)
                        dotLayout.dots[i].Opacity = 1;
                    else
                        dotLayout.dots[i].Opacity = 0.5;
            }
        }
    }
}

namespace CarouselView.CustomControls { 
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
    public class DotButton : BoxView
    {
        public int index;
        public DotButtonsLayout layout;
        public event ClickHandler Clicked;
        public delegate void ClickHandler(DotButton sender);
        public DotButton()
        {
            var clickCheck = new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (Clicked != null)
                    {
                        Clicked(this);
                    }
                })
            };
            GestureRecognizers.Add(clickCheck);
        }
    }
    public class DotButtonsLayout : StackLayout
    {
        //This array will hold the buttons
        public DotButton[] dots;
        public DotButtonsLayout(int dotCount, Color dotColor, int dotSize)
        {
            //Create as many buttons as desired.
            dots = new DotButton[dotCount];
            //This class inherits from a StackLayout, so we can stack
            //the buttons together from left to right.
            Orientation = StackOrientation.Horizontal;
            VerticalOptions = LayoutOptions.Center;
            HorizontalOptions = LayoutOptions.Center;
            //Here we create the buttons.
            for (int i = 0; i < dotCount; i++)
            {
                dots[i] = new DotButton
                {
                    HeightRequest = dotSize,
                    WidthRequest = dotSize,
                    BackgroundColor = dotColor,
                    //All buttons except the first one will get an opacity
                    //of 0.5 to visualize the first one is selected.
                    Opacity = 0.5
                };
                dots[i].index = i;
                dots[i].layout = this;
                Children.Add(dots[i]);
            }
            dots[0].Opacity = 1;
        }
    }

}
