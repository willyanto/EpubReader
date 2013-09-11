using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using nakam2;
using System.Text;

namespace NakamNakam
{
    public partial class FlagPage : PhoneApplicationPage
    {
        ApplicationBarIconButton submitButton;
        public FlagPage()
        {
            InitializeComponent();
            var obj = App.Current as App;
            title.Text = obj.nama;
            List<string> flag = new List<string>() { "spam", "mislocated", "closed", "misinformed"};
            this.flag.ItemsSource = flag;

            ApplicationBar = new ApplicationBar();
            submitButton = new ApplicationBarIconButton();
            submitButton.IconUri = new Uri("/Images/check.png", UriKind.Relative);
            submitButton.Text = "submit";
            submitButton.Click += submitButton_Click;
            ApplicationBar.Buttons.Add(submitButton);
        }

        void submitButton_Click(object sender, EventArgs e)
        {
            //post data
            var obj = App.Current as App;
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var uri = new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/flag.json", UriKind.Absolute);// url yang dituju untuk input data
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("{0}={1}", "resto_id", HttpUtility.UrlEncode(obj.id));
            postData.AppendFormat("&{0}={1}", "resto_name", HttpUtility.UrlEncode(obj.nama));
            postData.AppendFormat("&{0}={1}", "flag", HttpUtility.UrlEncode(flag.SelectedItem.ToString()));
            postData.AppendFormat("&{0}={1}", "comment", HttpUtility.UrlEncode(comment.Text));
            client.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            client.UploadStringAsync(uri, "POST", postData.ToString());
            client.UploadStringCompleted += (o, f) => {
                MessageBox.Show("Your report has been sent to the server.");
                NavigationService.GoBack(); 
            };
        }
    }
}