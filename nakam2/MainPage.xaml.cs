using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Reminders;
using Mangopollo.Tiles;
using System.Diagnostics;

namespace nakam2
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<Result> listItem = new List<Result>();
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        ApplicationBarIconButton submitButton, searchButton, refreshButton;
        ApplicationBarMenuItem setting, aboutMenu, profilMenu, logout;
        
        string user_name,myLat,myLng;
        public GeoCoordinateWatcher watcher;
        private static readonly Version _targetedVersion78 = new Version(7, 10, 8858);
        public static bool CanUseLiveTiles
        {
            get { return Environment.OSVersion.Version >= _targetedVersion78; }
        }
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            RadRateApplicationReminder reminder = new RadRateApplicationReminder();
            reminder.RecurrencePerUsageCount = 5;
            reminder.Notify();

            busyIndicator.IsRunning = true;
            //this.BestRatedListBox.ItemsSource = listItem;

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            
                
            #region Application Bar
            ApplicationBar = new ApplicationBar();
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
            ApplicationBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
            searchButton = new ApplicationBarIconButton();
            searchButton.IconUri = new Uri("/Images/appbar.feature.search.rest.png", UriKind.Relative);
            searchButton.Text = "search";
            searchButton.Click += searchButton_Click;
            refreshButton = new ApplicationBarIconButton();
            refreshButton.IconUri = new Uri("/Images/refresh.png", UriKind.Relative);
            refreshButton.Text = "refresh";
            refreshButton.Click += refreshButton_Click;
            submitButton = new ApplicationBarIconButton();
            submitButton.IconUri = new Uri("/Images/check.png", UriKind.Relative);
            submitButton.Text = "submit";
            submitButton.Click += submitButton_Click;

            //profilMenu = new ApplicationBarMenuItem();
            //profilMenu.Text = "profil";
            //profilMenu.Click += profilMenu_Click;
            setting = new ApplicationBarMenuItem();
            setting.Text = "setting";
            setting.Click += setting_Click;
            aboutMenu = new ApplicationBarMenuItem();
            aboutMenu.Text = "about";
            aboutMenu.Click += aboutMenu_Click;
            logout = new ApplicationBarMenuItem();
            logout.Text = "login";
            logout.Click += logout_Click;

            ApplicationBar.Buttons.Add(searchButton);
            ApplicationBar.Buttons.Add(refreshButton);
            //ApplicationBar.MenuItems.Add(profilMenu);
            ApplicationBar.MenuItems.Add(setting);
            ApplicationBar.MenuItems.Add(aboutMenu);
            ApplicationBar.MenuItems.Add(logout);
            #endregion


            List<string> price = new List<string>() { "5000", "10000", "20000","30000", "40000" };
            this.price.ItemsSource = price;

            List<string> countries = new List<string>() { "Indonesian Food", "Chinese Food", 
                "Japanese Food","Western Food", "Fast Food", "Snack", "Cafe", "Sea Food", "Vegetarian"};
            this.category.ItemsSource = countries;
            category.PopupHeader = "category";
            #region Get current postition
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if(appSettings.IsEmpty())
                appSettings.Add("location","on");

            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 20;
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                if ((string)appSettings["location"] == "on")
                    watcher.Start();
            }
            #endregion

            #region favourite
            //if (!isoStore.FileExists("Favourite.txt"))
            //{
            //    IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream("Favourite.txt", FileMode.Create, isoStore);
            //}
            ////try
            //{
            //    IsolatedStorageFileStream isoStream = isoStore.OpenFile("Favourite.txt", FileMode.Open, FileAccess.Read);
            //    using (StreamReader reader = new StreamReader(isoStream))
            //    {    //Visualize the text data in a TextBlock text
            //        var obj = App.Current as App;
            //        favResult = reader.ReadLine();
            //        favlistItem.Clear(); FavouriteListBox.ItemsSource = "";
            //        RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(favResult);
            //        foreach (var x in jsonResult.fav)
            //        {
            //            GeoCoordinate venuePos = new GeoCoordinate(x.latitude, x.longitude);
            //            favlistItem.Add(new Fav(x.id, x.name, x.address, x.avg_rating, x.image_url, x.rate_count,
            //                      x.latitude, x.longitude, x.open_hour, x.phone, x.start_price, x.last_update));
            //        }
            //        this.FavouriteListBox.ItemsSource = favlistItem;
            //    }
            //}
            ////catch { }
            #endregion

            #region FlipData
            if (CanUseLiveTiles)
            {
                var tileId = ShellTile.ActiveTiles.FirstOrDefault();
                if (tileId != null)
                {
                    var tileData = new FlipTileData();
                    tileData.BackgroundImage = new Uri("/icon173.png", UriKind.Relative);
                    tileData.WideBackgroundImage = new Uri("/346x173.png", UriKind.Relative);
                    Debug.WriteLine("Activating live tile: " + Mangopollo.Utils.CanUseLiveTiles);
                    tileId.Update(tileData);
                }
            }
            #endregion
        }
        
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPositionChanged(e));
        }
        void MyPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var obj = App.Current as App;
            obj.myPos = e.Position.Location;
            obj.myLat = e.Position.Location.Latitude.ToString("0.0000000000", CultureInfo.InvariantCulture);
            obj.myLng = e.Position.Location.Longitude.ToString("0.0000000000", CultureInfo.InvariantCulture);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();

            if (!isoStore.FileExists("Cookies.txt"))
            {
                IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream("Cookies.txt", FileMode.Create, isoStore);
            }
            try
            {
                IsolatedStorageFileStream isoStream = isoStore.OpenFile("Cookies.txt", FileMode.Open, FileAccess.Read);
                using (StreamReader reader = new StreamReader(isoStream))
                {    //Visualize the text data in a TextBlock text
                    user_name = reader.ReadLine();
                    var obj = App.Current as App;
                    obj.username = user_name;
                }
            }
            catch { }
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/session.json?username=" + user_name));
            client.DownloadStringCompleted += (o, r) => 
            {
                try
                {
                    string result = r.Result;
                    RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                    if (!jsonResult.result.IsEmpty())
                    {

                        foreach (var x in jsonResult.result)
                        {
                            if (x.session == "T")
                            {
                                var obj = App.Current as App;
                                obj.user_id = x.id;
                                logout.Text = x.username + " (logout)";
                            }
                        }
                    }
                    else
                    {
                        logout.Text = "login";
                    }
                }
                catch
                {
                    MessageBox.Show("No Internet Connection");
                }
            };
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            #region Download data
            var obj = App.Current as App;
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/nearby.json?lat=" + obj.myLat + "&lng=" + obj.myLng + "&r=3"));
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            #endregion
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var obj = App.Current as App;
                listItem.Clear(); NearbyListBox.ItemsSource = "";
               string result = e.Result;
                RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                foreach (var x in jsonResult.result)
                {
                    GeoCoordinate venuePos = new GeoCoordinate(x.latitude, x.longitude);
                    listItem.Add(new Result(x.id,x.name, x.address, x.avg_rating,x.description, x.image_url, obj.myPos.GetDistanceTo(venuePos),
                              x.latitude,x.longitude,x.open_hour,x.phone,x.start_price,x.last_update,x.source_link));
                }
                this.busyIndicator.IsRunning = false;
                NearbyListBox.Opacity = 1;
                this.NearbyListBox.ItemsSource = listItem;
            }
            catch
            {
                MessageBox.Show("No Internet Connection!");
            }
        }
        void setting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
        void profilMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProfilPage.xaml", UriKind.Relative));
        }

        void searchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }
        void submitButton_Click(object sender, EventArgs e)
        {
            var obj = App.Current as App;
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var uri = new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/list.json", UriKind.Absolute);// url yang dituju untuk input data
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("{0}={1}", "name", HttpUtility.UrlEncode(name.Text));
            postData.AppendFormat("&{0}={1}", "address", HttpUtility.UrlEncode(address.Text));
            postData.AppendFormat("&{0}={1}", "phone", HttpUtility.UrlEncode(phone.Text));
            postData.AppendFormat("&{0}={1}", "category", HttpUtility.UrlEncode(category.SelectedItem.ToString()));
            postData.AppendFormat("&{0}={1}", "start_price", HttpUtility.UrlEncode(price.SelectedItem.ToString()));
            postData.AppendFormat("&{0}={1}", "open_hour", HttpUtility.UrlEncode(hour.Text));
            postData.AppendFormat("&{0}={1}", "latitude", HttpUtility.UrlEncode(obj.myLat));
            postData.AppendFormat("&{0}={1}", "longitude", HttpUtility.UrlEncode(obj.myLng)); // data-data yang akan dismipan dimasukkan ke dalam string
            client.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            client.UploadStringCompleted +=client_UploadStringCompleted;
            // jika upload data selesai, maka akan menjalankan event webClient_uploadStringComplete
            client.UploadStringAsync(uri, "POST", postData.ToString());
        }
        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            MessageBox.Show("Your suggestion place has been sent. Admin will check your suggestion.");
            name.Text = "";
            address.Text = "";
            price.SelectedIndex = 1;
            phone.Text = "";
            category.SelectedIndex = 1;
            hour.Text = "";
        }
        void refreshButton_Click(object sender, EventArgs e)
        {
            var obj = App.Current as App;
            this.busyIndicator.IsRunning = true;
            NearbyListBox.Opacity = 0.7;
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/nearby.json?lat=" + obj.myLat + "&lng=" + obj.myLng + "&r=3"));
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }
        private void aboutMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        void logout_Click(object sender, EventArgs e)
        {
            if (logout.Text == "login")
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
            else
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                storage.DeleteFile("Cookies.txt");
                var obj = App.Current as App;
                obj.user_id = "";
                obj.username = "";
                logout.Text = "login";
                //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
        private void myPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myPanorama.SelectedIndex == 0)
            {
                if (!ApplicationBar.Buttons.Contains(searchButton))
                    ApplicationBar.Buttons.Add(searchButton);
                if (!ApplicationBar.Buttons.Contains(refreshButton))
                    ApplicationBar.Buttons.Add(refreshButton);
                ApplicationBar.Buttons.Remove(submitButton);
            }
            //else if (myPanorama.SelectedIndex == 1)
            //{
            //    if (!ApplicationBar.Buttons.Contains(searchButton))
            //        ApplicationBar.Buttons.Add(searchButton);
            //    ApplicationBar.Buttons.Remove(submitButton);
            //    ApplicationBar.Buttons.Remove(refreshButton);
            //}
            else if (myPanorama.SelectedIndex == 1)
            {
                if (!ApplicationBar.Buttons.Contains(searchButton))
                    ApplicationBar.Buttons.Add(searchButton);
                ApplicationBar.Buttons.Remove(refreshButton);
                ApplicationBar.Buttons.Remove(submitButton);
            }
            else if (myPanorama.SelectedIndex == 2)
            {
                ApplicationBar.Buttons.Remove(searchButton);
                ApplicationBar.Buttons.Remove(refreshButton);
                ApplicationBar.Buttons.Add(submitButton);
            }
        }
        
        private void NearbyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NearbyListBox.SelectedIndex == -1)
                return;
            // Navigate to the new page
            var obj = App.Current as App;
            obj.id = listItem[NearbyListBox.SelectedIndex].id;
            obj.nama = listItem[NearbyListBox.SelectedIndex].name;
            obj.location = listItem[NearbyListBox.SelectedIndex].address;
            obj.avg_rating = listItem[NearbyListBox.SelectedIndex].avg_rating;
            obj.description = listItem[NearbyListBox.SelectedIndex].description;
            obj.lat = listItem[NearbyListBox.SelectedIndex].latitude;
            obj.lng = listItem[NearbyListBox.SelectedIndex].longitude;
            obj.phone = listItem[NearbyListBox.SelectedIndex].phone;
            obj.open_hour = listItem[NearbyListBox.SelectedIndex].open_hour;
            obj.start_price = listItem[NearbyListBox.SelectedIndex].start_price;
            obj.last_update = listItem[NearbyListBox.SelectedIndex].last_update;
            obj.image_url = listItem[NearbyListBox.SelectedIndex].image_url;
            obj.source_link = listItem[NearbyListBox.SelectedIndex].source_link;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));

            // Reset selected index to -1 (no selection)
            NearbyListBox.SelectedIndex = -1;
        }
        //private void BestRatedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (BestRatedListBox.SelectedIndex == -1)
        //        return;

        //    NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));

        //    // Reset selected index to -1 (no selection)
        //    BestRatedListBox.SelectedIndex = -1;
        //}

        private void hub1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=IDN", UriKind.Relative));
        }

        private void hub2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=CHN", UriKind.Relative));
        }

        private void hub3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=JPN", UriKind.Relative));
        }

        private void hub4_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=WST", UriKind.Relative));
        }

        private void hub5_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=FAS", UriKind.Relative));
        }

        private void hub6_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=SNK", UriKind.Relative));
        }

        private void hub7_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=CAF", UriKind.Relative));
        }

        private void hub8_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=SEA", UriKind.Relative));
        }

        private void hub9_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryListPage.xaml?id=VGT", UriKind.Relative));
        }
        private void address_GotFocus(object sender, RoutedEventArgs e)
        {
            address.Background = new SolidColorBrush(Colors.LightGray);

        }

        private void price_GotFocus(object sender, RoutedEventArgs e)
        {
            price.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void phone_GotFocus(object sender, RoutedEventArgs e)
        {
            phone.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void category_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            category.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void hour_GotFocus(object sender, RoutedEventArgs e)
        {
            hour.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void name_GotFocus(object sender, RoutedEventArgs e)
        {
            name.Background = new SolidColorBrush(Colors.LightGray);
        }


    }

    public class Result
    {
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string phone { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public string last_update { get; set; }
        public string start_price { get; set; }
        public string open_hour { get; set; }
        public string hashtag { get; set; }
        public string rating { get; set; }
        public string image_url { get; set; }
        public string distance { get; set; }
        public string rate_count { get; set; }
        public string username { get; set; }
        public string user_id { get; set; }
        public string session { get; set; }
        public string comment { get; set; }
        public string date_created { get; set; }
        public string avg_rating { get; set; }
        public string source_link { get; set; }
        public Result(string id, string name, string address, string avg_rating, string description,string image_url, double distance,
            double latitude, double longitude, string open_hour,string phone, string start_price, string last_update,string source_link)
        {
            this.id = id;
            this.name = name;
            this.address = address;
            this.avg_rating = avg_rating != null ? avg_rating.Substring(0, 3) : avg_rating;
            this.description = description;
            this.image_url = image_url;
            this.distance = (distance / 1000).ToString("0.00", CultureInfo.InvariantCulture) + " km";
            this.latitude = latitude;
            this.longitude = longitude;
            this.open_hour = open_hour;
            this.phone = phone;
            this.start_price = start_price;
            this.last_update = last_update;
            this.source_link = source_link;
        }
    }
    public class RootObject
    {
        public List<Result> result { get; set; }
    }
}