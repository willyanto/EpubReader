using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.Device.Location;
using Telerik.Windows.Controls.PhoneTextBox;
using Telerik.Windows.Controls;
using System.Windows.Media;

namespace nakam2
{
    public partial class SearchPage : PhoneApplicationPage
    {
        List<Result> listItem = new List<Result>();
        public SearchPage()
        {
            InitializeComponent();
        }

        private void TextBoxSearch_ActionButtonTap(object sender, EventArgs e)
        {
            //searching data
            if (string.IsNullOrEmpty(this.TextBoxSearch.Text))
            {
                TextBoxSearch.ChangeValidationState(ValidationState.Invalid, "insert keyword!");
                return;
            }
            else
            {
                this.TextBoxSearch.ChangeValidationState(ValidationState.NotValidated, string.Empty);
                SearchListBox.ItemsSource = "";
                this.busyIndicator.IsRunning = true;
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/list.json?name=" + TextBoxSearch.Text));
                client.DownloadStringCompleted += client_DownloadStringCompleted;
            }
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                listItem.Clear();
                var obj = App.Current as App;
                string result = e.Result;
                RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                if (!jsonResult.result.IsEmpty())
                {
                    foreach (var x in jsonResult.result)
                    {
                        GeoCoordinate venuePos = new GeoCoordinate(x.latitude, x.longitude);
                        GeoCoordinate myPos = new GeoCoordinate(obj.myPos.Latitude, obj.myPos.Longitude);
                        listItem.Add(new Result(x.id, x.name, x.address, x.avg_rating,x.description, x.image_url, myPos.GetDistanceTo(venuePos),
                                  x.latitude, x.longitude, x.open_hour, x.phone, x.start_price,x.last_update,x.source_link));
                    }
                }
                else
                {
                    MessageBox.Show("Data not found");
                }
                this.busyIndicator.IsRunning = false;
                SearchListBox.Opacity = 1;
                this.SearchListBox.ItemsSource = listItem;
            }
            catch
            {
                MessageBox.Show("No Internet Connection!");
            }
        }

        private void SearchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchListBox.SelectedIndex == -1)
                return;
            // Navigate to the new page
            var obj = App.Current as App;
            obj.id = listItem[SearchListBox.SelectedIndex].id;
            obj.nama = listItem[SearchListBox.SelectedIndex].name;
            obj.location = listItem[SearchListBox.SelectedIndex].address;
            obj.avg_rating = listItem[SearchListBox.SelectedIndex].avg_rating;
            obj.description = listItem[SearchListBox.SelectedIndex].description;
            obj.lat = listItem[SearchListBox.SelectedIndex].latitude;
            obj.lng = listItem[SearchListBox.SelectedIndex].longitude;
            obj.phone = listItem[SearchListBox.SelectedIndex].phone;
            obj.open_hour = listItem[SearchListBox.SelectedIndex].open_hour;
            obj.start_price = listItem[SearchListBox.SelectedIndex].start_price;
            obj.last_update = listItem[SearchListBox.SelectedIndex].last_update;
            obj.image_url = listItem[SearchListBox.SelectedIndex].image_url;
            obj.source_link = listItem[SearchListBox.SelectedIndex].source_link;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));

            // Reset selected index to -1 (no selection)
            SearchListBox.SelectedIndex = -1;
        }

        private void TextBoxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxSearch.Background = new SolidColorBrush(Colors.LightGray);
        }

    }
}