using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

namespace ListViewScrollToBug
{
    public partial class ListViewScrollToBugPage : ContentPage
    {
        public ListViewScrollToBugPage()
        {
            InitializeComponent();

            _listView.ItemsSource = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
           
            _entry.Focused += async (sender, e) =>
            {
                _mainGrid.VerticalOptions = LayoutOptions.Start;
                _mainGrid.HeightRequest = _mainGrid.Height - 255;
                await Task.Delay(100);
                ScrollToBottom(false);
            };

            _entry.Unfocused += (sender, e) =>
            {
                _mainGrid.VerticalOptions = LayoutOptions.FillAndExpand;
                _mainGrid.HeightRequest = -1;
                ScrollToBottom(false);
            };
        }

        public void ScrollToBottom(bool isAnimated)
        {
            var items = _listView.ItemsSource as IEnumerable<object>;
            if (items != null)
            {
                var last = items.LastOrDefault();
                if (last != null)
                {
                    _listView.ScrollTo(last, ScrollToPosition.End, isAnimated);
                }
            }
        }
    }
}
