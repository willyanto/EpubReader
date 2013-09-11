using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Telerik.Windows.Controls.PhoneTextBox;
using System.Text.RegularExpressions;
using Telerik.Windows.Controls;
using Newtonsoft.Json;
using System.Text;
using System.Globalization;
using System.Windows.Media;

namespace nakam2
{
    public partial class SignUpPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        Boolean valid = false;

        public SignUpPage()
        {
            InitializeComponent();
            this.radDatePicker.DisplayValueFormat = " yyyy-MM-dd";
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                RootObject jsonResult = JsonConvert.DeserializeObject<RootObject>(result);
                if (jsonResult.result.IsEmpty()) valid = true;
                else valid = false;
                if (string.IsNullOrEmpty(this.TextBoxUsername.Text))
                {
                    this.TextBoxUsername.ChangeValidationState(ValidationState.NotValidated, string.Empty);
                    this.isUserNameValid = false;
                    this.OnPropertyChanged("IsButtonEnabled");
                    return;
                }
                else
                {
                    if (valid == false)
                    {
                        this.TextBoxUsername.ChangeValidationState(ValidationState.Invalid, "This username is not available");
                        this.isUserNameValid = false;
                        this.OnPropertyChanged("IsButtonEnabled");
                    }
                    else
                    {
                        this.TextBoxUsername.ChangeValidationState(ValidationState.Valid, "Available username!");
                        this.isUserNameValid = true;
                        this.OnPropertyChanged("IsButtonEnabled");
                    }
                }
            }
            catch
            {
                MessageBox.Show("No Internet Connection");
            }
        
        }
        string bannedUsername = string.Empty;

        private const string MatchEmailPattern = @"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}\b";

        public bool IsButtonEnabled
        {
            get
            {
                return isNameValid && isUserNameValid && isEmailValid && isPasswordValid && isBirthdayValid;
            }
        }

        private bool isNameValid = false;
        private bool isUserNameValid = false;
        private bool isEmailValid = false;
        private bool isPasswordValid = false;
        private bool isBirthdayValid = false;


        private void RadTextBox_ActionButtonTap(object sender, EventArgs e)
        {
            CheckUsernameAvailability();
        }

        private void CheckUsernameAvailability()
        {
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/username.json?username="+TextBoxUsername.Text));
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            return Regex.IsMatch(email, MatchEmailPattern);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //var uri = new Uri("http://dev.nakamnakam.com/api/public/index.php/nakam2/registration.json", UriKind.Absolute);// url yang dituju untuk input data
            var uri = new Uri("http://dev.nakamnakam.com/restserver/index.php", UriKind.Absolute);// url yang dituju untuk input data
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("{0}={1}", "name", HttpUtility.UrlEncode(TextBoxName.Text));
            postData.AppendFormat("&{0}={1}", "username", HttpUtility.UrlEncode(TextBoxUsername.Text));
            postData.AppendFormat("&{0}={1}", "password", HttpUtility.UrlEncode(PasswordBox.Password));
            postData.AppendFormat("&{0}={1}", "email", HttpUtility.UrlEncode(TextBoxEmail.Text));
            postData.AppendFormat("&{0}={1}", "gender", HttpUtility.UrlEncode(ListPickerGender.SelectedItem.ToString()));
            postData.AppendFormat("&{0}={1}", "birthday", HttpUtility.UrlEncode(radDatePicker.ValueString));// data-data yang akan dismipan dimasukkan ke dalam string
            client.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            client.UploadStringCompleted +=client_UploadStringCompleted;
           
            client.UploadStringAsync(uri, "POST", postData.ToString());
            busyIndicator.IsRunning = true;
        }

        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            busyIndicator.IsRunning = false;
            MessageBox.Show("Successful registration! Please open your email for activation and restart this application.");
            NavigationService.GoBack();
        }

        private void TextBoxName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TextBoxName.Text))
            {
                this.isNameValid = true;
                this.OnPropertyChanged("IsButtonEnabled");
            }
            else
            {
                this.isNameValid = false;
                this.OnPropertyChanged("IsButtonEnabled");
            }
        }

        private void TextBoxUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckUsernameAvailability();
        }

        private void TextBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TextBoxEmail.Text))
            {
                TextBoxEmail.ChangeValidationState(ValidationState.NotValidated, string.Empty);
                this.isEmailValid = false;
                this.OnPropertyChanged("IsButtonEnabled");
                return;
            }
            if (!IsEmail(this.TextBoxEmail.Text))
            {
                TextBoxEmail.ChangeValidationState(ValidationState.Invalid, "This is not a valid email!");
                this.isEmailValid = false;
                this.OnPropertyChanged("IsButtonEnabled");
            }
            else
            {
                TextBoxEmail.ChangeValidationState(ValidationState.Valid, "Valid email!");
                this.isEmailValid = true;
                this.OnPropertyChanged("IsButtonEnabled");
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int length = this.PasswordBox.Password.Length;
            if (length >= 6)
            {
                this.PasswordBox.ChangeValidationState(ValidationState.Valid, "Valid password!");
                this.isPasswordValid = true;
                this.OnPropertyChanged("IsButtonEnabled");
                return;
            }
            if (length > 0)
            {
                this.PasswordBox.ChangeValidationState(ValidationState.Invalid, "At least 6 characters!");
                this.isPasswordValid = false;
                this.OnPropertyChanged("IsButtonEnabled");
                return;
            }
            this.PasswordBox.ChangeValidationState(ValidationState.NotValidated, string.Empty);
            this.isPasswordValid = false;
            this.OnPropertyChanged("IsButtonEnabled");
        }

        private void radDatePicker_PopupClosed(object sender, EventArgs e)
        {
            if (radDatePicker.Value == null)
            {
                this.isBirthdayValid = false;
                this.OnPropertyChanged("IsButtonEnabled");
            }
            else
            {
                this.isBirthdayValid = true;
                this.radDatePicker.Background = new SolidColorBrush(Colors.White);
                this.OnPropertyChanged("IsButtonEnabled");
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void TextBoxName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxName.Background = new SolidColorBrush(Colors.White);
        }

        private void TextBoxUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxUsername.Background = new SolidColorBrush(Colors.White);
        }

        private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxEmail.Background = new SolidColorBrush(Colors.White);
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox.Background = new SolidColorBrush(Colors.White);
        }
        
    }
}