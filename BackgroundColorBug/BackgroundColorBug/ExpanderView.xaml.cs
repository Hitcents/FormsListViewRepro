using System;
using Xamarin.Forms;

namespace BackgroundColorBug
{
    public enum ExpanderType
    {
        ChatList, GameResult
    }

    public partial class ExpanderView : Grid
    {
        private const uint ExpandAnimationLength = 400;
        private const string ExpandAnimationKey = "expand_animation";

        private static Color ChatMessageColor = Color.FromRgb(0, 0, 0);
        private static Color ChatBackgroundColor = Color.FromRgb(235, 235, 235);
        private static Color WhiteOpacity20Color = Color.FromRgba(255,255,255, 51);

        private double initalContentHeight;
        private bool isTapped;

        private Label _infoLabel;

        private bool needToCompress;

        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ExpanderView), string.Empty, propertyChanged: (bindable, oldValue, newValue) =>
         {
             var view = bindable as ExpanderView;
             view.SetTitleText();
         });

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(ExpanderView), -1, propertyChanged: (bindable, oldValue, newValue) =>
         {
             var view = bindable as ExpanderView;
             view.SetTitleText();
         });

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static BindableProperty ExpanderContentProperty = BindableProperty.Create(nameof(ExpanderContent), typeof(View), typeof(ExpanderView), null, propertyChanged: (bindable, oldValue, newValue) =>
          {
              var view = bindable as ExpanderView;
              view._content.Content = newValue as View;
          });

        public View ExpanderContent
        {
            get { return (View)GetValue(ExpanderContentProperty); }
            set { SetValue(ExpanderContentProperty, value); }
        }

        public static BindableProperty IsExpandedProperty = BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(ExpanderView), true, propertyChanged: (bindable, oldValue, newValue) =>
         {
             var view = bindable as ExpanderView;
             view.Expand((bool)newValue);
         });

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static BindableProperty ExpanderTypeProperty = BindableProperty.Create(nameof(Type), typeof(ExpanderType), typeof(ExpanderView), ExpanderType.ChatList);

        public ExpanderType Type
        {
            get { return (ExpanderType)GetValue(ExpanderTypeProperty); }
            set { SetValue(ExpanderTypeProperty, value); }
        }

        public ExpanderView()
        {
            InitializeComponent();

            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += (sender, e) =>
            {
                Expand(!IsExpanded);
            };

            _headerGrid.GestureRecognizers.Add(tapRecognizer);

            _infoLabel = new Label();
        }

        private void Expand(bool expand, bool animate = true)
        {
            if (isTapped) return;

            if (initalContentHeight <= 0)
            {
                needToCompress = true;
                return;
            }

            isTapped = true;

            IsExpanded = expand;

            if (!expand)
            {
                initalContentHeight = _content.Height;
            }

            if (animate)
            {
                var animation = new Animation();
                animation.Add(0, 1, new Animation(v => _content.HeightRequest = v, _content.Height, expand ? initalContentHeight : 0));
                animation.Commit(_content, ExpandAnimationKey, length: ExpandAnimationLength, finished: (arg1, arg2) =>
                {
                    isTapped = false;
                });
            }
            else
            {
                _content.HeightRequest = expand ? initalContentHeight : 0;
                isTapped = false;
            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            if (initalContentHeight <= 0 && _content.Height > 0)
            {
                initalContentHeight = _content.Height;

                if (needToCompress)
                {
                    needToCompress = false;
                    Expand(false, false);
                }
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null)
            {
                SetHeaderContent();
            }
        }

        private void SetHeaderContent()
        {
            if (Type == ExpanderType.ChatList)
            {
                SetHeaderForChat();
            }
            else if (Type == ExpanderType.GameResult)
            {
                SetHeaderForGameResult();
            }
        }

        private void SetHeaderForChat()
        {
            _infoLabel.VerticalTextAlignment = TextAlignment.Center;
            _infoLabel.FontSize = 10;
            _infoLabel.TextColor = ChatMessageColor;

            StackLayout stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(15, 0, 0, 0)
            };

            stackLayout.Children.Add(_infoLabel);
            _headerGrid.Children.Add(stackLayout);
            _headerGrid.BackgroundColor = ChatBackgroundColor;
        }

        private void SetHeaderForGameResult()
        {
            _infoLabel.VerticalOptions = LayoutOptions.Center;
            _infoLabel.VerticalTextAlignment = TextAlignment.Center;
            _infoLabel.HorizontalTextAlignment = TextAlignment.Center;
            _infoLabel.FontSize = 16;
            _infoLabel.TextColor = Color.White;

            var bottomBoxView = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = WhiteOpacity20Color,
                VerticalOptions = LayoutOptions.End
            };

            var topBoxView = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = WhiteOpacity20Color,
                VerticalOptions = LayoutOptions.Start
            };

            Grid.SetColumnSpan(bottomBoxView, 2);
            Grid.SetColumnSpan(topBoxView, 2);

            _headerGrid.Children.Add(bottomBoxView);
            _headerGrid.Children.Add(topBoxView);
            _headerGrid.Children.Add(_infoLabel);
            _headerGrid.BackgroundColor = Color.Transparent;
        }

        private void SetTitleText()
        {
            var info = Count > -1 ? string.Format(Text, Count) : Text;

            var animation = new Animation();
            animation.Add(0, 0.7, new Animation(v => _infoLabel.Opacity = v, 1, 0, finished: () =>
            {
                _infoLabel.Text = info;
            }));

            animation.Add(0.7, 1, new Animation(v => _infoLabel.Opacity = v, 0, 1));
            animation.Commit(_infoLabel, "info_animation", length: 800, easing: Easing.CubicOut);
        }
    }
}