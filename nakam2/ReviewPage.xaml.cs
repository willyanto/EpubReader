using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.Globalization;

namespace nakam2
{
    public partial class ReviewPage : PhoneApplicationPage
    {
        ApplicationBarIconButton submitButton;
        public ReviewPage()
        {
            InitializeComponent();
            var obj = App.Current as App;
            titlePage.Text = obj.nama;

            ApplicationBar = new ApplicationBar();
            submitButton = new ApplicationBarIconButton();
            submitButton.IconUri = new Uri("/Images/check.png", UriKind.Relative);
            submitButton.Text = "submit";
            submitButton.Click += submitButton_Click;

        }

        void submitButton_Click(object sender, EventArgs e)
        {
            //post data
            var obj = App.Current as App;
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var uri = new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/rating.json", UriKind.Absolute);// url yang dituju untuk input data
            StringBuilder postData = new StringBuilder();
            string x = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            postData.AppendFormat("{0}={1}", "user_id", HttpUtility.UrlEncode(obj.user_id));
            postData.AppendFormat("&{0}={1}", "resto_id", HttpUtility.UrlEncode(obj.id));
            postData.AppendFormat("&{0}={1}", "rate", HttpUtility.UrlEncode(tbrating.Value.ToString()));
            postData.AppendFormat("&{0}={1}", "comment", HttpUtility.UrlEncode(review.Text));
            postData.AppendFormat("&{0}={1}", "date_created", HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))); // data-data yang akan dismipan dimasukkan ke dalam string
            client.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            client.UploadStringAsync(uri, "POST", postData.ToString());
            client.UploadStringCompleted += (o, f) => { NavigationService.GoBack(); };
        }


        private void tbrating_ValueChanged(object sender, Telerik.Windows.Controls.ValueChangedEventArgs<object> e)
        {
            if (review.IsReadOnly)
            {
                review.IsReadOnly = false;
                review.Text = "";
            }
            if (!ApplicationBar.Buttons.Contains(submitButton))
                ApplicationBar.Buttons.Add(submitButton);
        }
    }
}