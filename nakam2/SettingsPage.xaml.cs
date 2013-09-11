using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace nakam2
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        public SettingsPage()
        {
            InitializeComponent();
            toggleSwtich.BorderBrush = new SolidColorBrush(Colors.White);
            if ((string)appSettings["location"] == "on")
                toggleSwtich.IsChecked = true;
            else
                toggleSwtich.IsChecked = false;
        }

        private void toggleSwtich_CheckedChanged(object sender, Telerik.Windows.Controls.CheckedChangedEventArgs e)
        {
            MainPage mainPage = new MainPage();
            if (e.NewState)
            {
                appSettings["location"] = "on";
                mainPage.watcher.Start();
            }
            else
            {
                appSettings["location"] = "off";
                mainPage.watcher.Stop();
            }
        }


    }
}