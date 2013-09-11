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

namespace nakam2
{
    public partial class CategoryListPage : PhoneApplicationPage
    {
        List<Result> listItem = new List<Result>();
        public CategoryListPage()
        {
            InitializeComponent();
            this.busyIndicator.IsRunning = true;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string id = "";
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                switch (id)
                {
                    case "IDN": PageTitle.Text = "Indonesian Food"; break;
                    case "CHN": PageTitle.Text = "Chinese Food"; break;
                    case "JPN": PageTitle.Text = "Japanese Food"; break;
                    case "WST": PageTitle.Text = "Western Food"; break;
                    case "FAS": PageTitle.Text = "Fast Food"; break;
                    case "SNK": PageTitle.Text = "Snack"; break;
                    case "CAF": PageTitle.Text = "Café"; break;
                    case "VGT": PageTitle.Text = "Vegetarian"; break;
                    case "SEA": PageTitle.Text = "Sea Food"; break;
                }
                WebClient client = new WebClient();
                client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/category.json?id=" + id));
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
                foreach (var x in jsonResult.result)
                {
                    GeoCoordinate venuePos = new GeoCoordinate(x.latitude, x.longitude);
                    GeoCoordinate myPos = new GeoCoordinate(obj.myPos.Latitude, obj.myPos.Longitude);
                    listItem.Add(new Result(x.id,x.name, x.address, x.avg_rating,x.description, x.image_url, myPos.GetDistanceTo(venuePos),
                              x.latitude, x.longitude, x.open_hour, x.phone, x.start_price,x.last_update,x.source_link));
                }
                this.busyIndicator.IsRunning = false;
                CategoryListBox.Opacity = 1;
                this.CategoryListBox.ItemsSource = listItem;
            }
            catch
            {
                MessageBox.Show("No Internet Connection!");
            }
        }

        private void CategoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryListBox.SelectedIndex == -1)
                return;
            // Navigate to the new page
            var obj = App.Current as App;
            obj.id = listItem[CategoryListBox.SelectedIndex].id;
            obj.nama = listItem[CategoryListBox.SelectedIndex].name;
            obj.location = listItem[CategoryListBox.SelectedIndex].address;
            obj.avg_rating = listItem[CategoryListBox.SelectedIndex].avg_rating;
            obj.description = listItem[CategoryListBox.SelectedIndex].description;
            obj.lat = listItem[CategoryListBox.SelectedIndex].latitude;
            obj.lng = listItem[CategoryListBox.SelectedIndex].longitude;
            obj.phone = listItem[CategoryListBox.SelectedIndex].phone;
            obj.open_hour = listItem[CategoryListBox.SelectedIndex].open_hour;
            obj.start_price = listItem[CategoryListBox.SelectedIndex].start_price;
            obj.last_update = listItem[CategoryListBox.SelectedIndex].last_update;
            obj.image_url = listItem[CategoryListBox.SelectedIndex].image_url;
            obj.source_link = listItem[CategoryListBox.SelectedIndex].source_link;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));

            // Reset selected index to -1 (no selection)
            CategoryListBox.SelectedIndex = -1;
        }
    }
    
}