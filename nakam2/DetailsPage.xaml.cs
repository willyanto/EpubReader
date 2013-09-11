using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using Microsoft.Phone.Tasks;
using System.Globalization;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Telerik.Windows.Controls;
using Newtonsoft.Json;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;

namespace nakam2
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        ApplicationBarIconButton route, share, flag;
        List<Results> listItem = new List<Results>();
        BingMapsDirectionsTask task;
        string rate_count = "";
        //public string rating { get; set; }
        //public string comment { get; set; }
        //public string date_created { get; set; }
        public DetailsPage()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();
            route = new ApplicationBarIconButton();
            route.IconUri = new Uri("/Images/route.png", UriKind.Relative);
            route.Text = "route";
            route.Click += route_Click;
            share = new ApplicationBarIconButton();
            share.IconUri = new Uri("/Images/share.png", UriKind.Relative);
            share.Text = "share";
            share.Click += share_Click;
            flag = new ApplicationBarIconButton();
            flag.IconUri = new Uri("/Images/flag.png", UriKind.Relative);
            flag.Text = "flag";
            flag.Click += flag_Click;
            ApplicationBar.Buttons.Add(flag);
            ApplicationBar.Buttons.Add(route);
            ApplicationBar.Buttons.Add(share);

            var obj = App.Current as App;
            resto_name.Text = obj.nama;
            TB_ALAMAT.Text = obj.location;
            TB_OPENHOUR.Text = obj.open_hour;
            TB_PHONE.Text = obj.phone;
            TB_PRICE.Text = "IDR " + Convert.ToInt32(obj.start_price).ToString("N2");
            TB_Date.Text = obj.last_update;

            if (obj.source_link != "")
            {
                source.Visibility = Visibility.Visible;
            }

            resto_image.Source = new BitmapImage(new Uri(obj.image_url, UriKind.RelativeOrAbsolute));
            desc.Text = obj.description != "" ? obj.description : "No description";
            

            GeoCoordinate coordinate = new GeoCoordinate();
            coordinate.Latitude = obj.lat;
            coordinate.Longitude = obj.lng;

            // The pushpin to add to the map.
            Pushpin pin = new Pushpin();
            pin.Location = coordinate;
            //pin.Template = (ControlTemplate)(this.Resources["PushpinControlTemplate1"]);
            pin.Background = new SolidColorBrush(Color.FromArgb(255,139,94,59));
            pin.Content = obj.nama;

            // Adds the pushpin to the map.
            map1.Children.Add(pin);
            map1.ZoomLevel = 15;
            map1.Center = coordinate; 

            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/rating.json?id="+obj.id ));
            client.DownloadStringCompleted += client_DownloadStringCompleted;



            string realCulture = Thread.CurrentThread.CurrentCulture.Name;
            try
            {
                task = new BingMapsDirectionsTask
                {
                    End = new LabeledMapLocation(obj.nama, new GeoCoordinate(obj.lat, obj.lng))
                };
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }
            catch
            { }
            finally
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(realCulture);
            }
        }

        void flag_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FlagPage.xaml", UriKind.Relative));
        }

        //void favourite_Click(object sender, EventArgs e)
        //{
        //    var obj = App.Current as App;
        //    Fav fav = new Fav(obj.id, obj.nama, obj.location, obj.avg_rating, obj.image_url,rate_count,obj.lat, obj.lng, obj.open_hour, obj.phone, obj.start_price, obj.last_update);
        //    List<Fav> result = new List<Fav>();
        //    result.Add(fav);
        //    RootObject1 results = new RootObject1();
        //    results.fav = result;
        //    string customer = Newtonsoft.Json.JsonConvert.SerializeObject(results);

        //    IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        //    if (!isoStore.FileExists("Favourite.txt"))
        //    {
        //        IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream("Favourite.txt", FileMode.Create, isoStore);
        //    }
        //    try
        //    {

        //        IsolatedStorageFileStream fileStream = isoStore.OpenFile("Favourite.txt", FileMode.Open, FileAccess.Write);
        //        using (StreamWriter writer = new StreamWriter(fileStream))
        //        {
        //            writer.Write(customer);
        //            writer.Close();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        void route_Click(object sender, EventArgs e)
        {
            task.Show();
        }

        void share_Click(object sender, EventArgs e)
        {
            var obj = App.Current as App;
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = obj.nama + " is amazing place!";

            shareStatusTask.Show();
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string rate_count = "";
                var obj = App.Current as App;
                string result = e.Result;
                RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                foreach (var x in jsonResult.result)
                {
                    rate_count = x.rate_count;
                    if (x.username == obj.username || x.username == null)
                        giveRating.Visibility = Visibility.Collapsed;
                    if (x.username != null)
                    {
                        tbrating.Value = Convert.ToDouble(obj.avg_rating, new CultureInfo("en-US"));
                        tbrating1.Value = Convert.ToDouble(obj.avg_rating, new CultureInfo("en-US"));
                        listItem.Add(new Results(x.rating, x.rate_count, x.comment, x.date_created, x.username, 5));
                    }
                }
                rate_count = rate_count == "" ? "0" : rate_count;
                TB_RATING.Text = rate_count + " rating";
                TB_RATING1.Text = rate_count + " rating";
                busyIndicator.IsRunning = false;
                this.ReviewListBox.ItemsSource = listItem;
            }catch{}
        }

        private void Pivot1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivot1.SelectedIndex == 0)
            {
                ApplicationBar.IsVisible = true;
            }
            else{
                ApplicationBar.IsVisible = false;
            }
        }

        private void giveRating_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var obj = App.Current as App;
            if (obj.username != null && obj.username!="")
            {
                NavigationService.Navigate(new Uri("/ReviewPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
        }

        private void TB_PHONE_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //var obj = App.Current as App;

            //PhoneCallTask phoneCallTask = new PhoneCallTask();
            //phoneCallTask.DisplayName = obj.nama;
            //phoneCallTask.PhoneNumber = obj.phone;

            //phoneCallTask.Show();
        }

        private void source_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //website trave.kapalangi.com/malang belum support di mobile
            var obj = App.Current as App;
            var wbt = new WebBrowserTask();
            //wbt.URL = obj.source_link;
            wbt.URL = "http://www.kapanlagi.com/travel";
            wbt.Show();
        }

        

    }
    public class Results
    {
        public string rating { get; set; }
        public double avg_rating { get; set; }
        public string username { get; set; }
        public string rate_count { get; set; }
        public string comment { get; set; }
        public string date_created { get; set; }
        public int itemCount { get; set; }
        public Results(string rating, string rate_count, string comment, string date_created, string username,int itemCount)
        {
            this.rating = rating;
            this.rate_count = rate_count;
            this.comment = comment;
            this.date_created = date_created;
            this.username = username;
            this.itemCount = itemCount;
        }
    }
}