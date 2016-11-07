using Xamarin.Forms;

namespace BackgroundColorBug
{
    public partial class BackgroundColorBugPage : ContentPage
    {
        public BackgroundColorBugPage()
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                _stackLayout.IsVisible = true;
            };
            _contentView.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}
