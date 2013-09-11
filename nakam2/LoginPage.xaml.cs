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
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using Telerik.Windows.Controls;
using System.Device.Location;
using System.Globalization;
using System.Windows.Media;

namespace nakam2
{
    public partial class LoginPage : PhoneApplicationPage
    {
        string user_name;
        GeoCoordinateWatcher watcher;

        public LoginPage()
        {
            InitializeComponent();
        }
       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

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
            client.DownloadStringCompleted += DownloadStringCompleted;
        }

        private void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                if (!jsonResult.result.IsEmpty())
                {

                    foreach (var x in jsonResult.result)
                    {
                        if (x.session == "T")
                        {
                            var obj = App.Current as App;
                            obj.user_id = x.id;
                            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                        }
                    }
                }
                else
                {
                    this.busyIndicator.IsRunning = false;
                }
            }
            catch
            {
                MessageBox.Show("No Internet Connection");
            }
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FacebookLoginPage.xaml", UriKind.Relative));
        }
        private void signUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SignUpPage.xaml", UriKind.Relative));
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            this.busyIndicator.IsRunning = true;
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/userAuth.json?username=" + username.Text + "&password=" + password.Password));
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                foreach (var x in jsonResult.result)
                {
                    if (x.username != "")
                    {
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                        IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream fileStream = isoStore.OpenFile("Cookies.txt", FileMode.Open, FileAccess.Write);
                        using (StreamWriter writer = new StreamWriter(fileStream))
                        {
                            string someTextData = x.username;
                            writer.Write(someTextData);
                            writer.Close();
                        }

                        var obj = App.Current as App;
                        obj.user_id = x.user_id;
                        obj.username = x.username;

                        WebClient client = new WebClient();
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var uri = new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/session.json", UriKind.Absolute);// url yang dituju untuk input data
                        StringBuilder postData = new StringBuilder();
                        postData.AppendFormat("{0}={1}", "username", HttpUtility.UrlEncode(x.username)); // data-data yang akan dismipan dimasukkan ke dalam string
                        client.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
                        client.UploadStringAsync(uri, "POST", postData.ToString());
                    }
                    this.busyIndicator.IsRunning = false;
                    return;
                }
                this.busyIndicator.IsRunning = false;
                MessageBox.Show("invalid username or password");
                password.Password = "";
            }
            catch
            {
                MessageBox.Show("No Internet Connection");
            }
        }

        private void username_GotFocus(object sender, RoutedEventArgs e)
        {
            username.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            password.Background = new SolidColorBrush(Colors.LightGray);
        }

    }
}