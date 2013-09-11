using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using System.Windows.Media;

namespace nakam2
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            //untuk about
            Run myRun = new Run();
            myRun.Text = "NakamNakam is an application for culinary lovers. This application can help you find the nearest restaurant. You can give a review or suggest a new culinary. This application currently runs only in the Malang city.\n" +
                        "Some data in this application were supported by ";

            Hyperlink MyLink = new Hyperlink();
            //MyLink.Foreground = new SolidColorBrush(Colors.Blue);
            MyLink.Inlines.Add("KapanLagi.com™\n");
            MyLink.Foreground = new SolidColorBrush(Colors.White);
            MyLink.NavigateUri = new Uri("http://www.kapanlagi.com/");
            MyLink.TargetName = "_blank";

            Run myRun1 = new Run();
            myRun1.Text = "This app developed by Willyanto Wijaya Sulaiman. If you would like to contact me,\nsend email to ";
            
            Hyperlink MyLink1 = new Hyperlink();
            //MyLink.Foreground = new SolidColorBrush(Colors.Blue);
            MyLink1.Inlines.Add("willyanto.wijaya@gmail.com\n");
            MyLink1.Foreground = new SolidColorBrush(Colors.White);
            MyLink1.NavigateUri = new Uri("mailto:willyanto.wijaya@gmail.com");
            MyLink1.TargetName = "_blank";

            // Create a paragraph and add the Run and hyperlink to it.
            Paragraph myParagraph = new Paragraph();
            myParagraph.Inlines.Add(myRun);
            myParagraph.Inlines.Add(MyLink);
            myParagraph.Inlines.Add(myRun1);
            myParagraph.Inlines.Add(MyLink1);

            Run myRun2 = new Run();
            myRun2.Text = "\nIf you like this app, please rate and share your feedback. Thank you.\n";
            myParagraph.Inlines.Add(myRun2);

            Hyperlink MyLink2 = new Hyperlink();
            //MyLink.Foreground = new SolidColorBrush(Colors.Blue);
            MyLink2.Inlines.Add("\nKapanLagi.com™ Service Terms");
            MyLink2.Foreground = new SolidColorBrush(Colors.White);
            MyLink2.NavigateUri = new Uri("http://company.kapanlagi.com/tos/");
            MyLink2.TargetName = "_blank";

            myParagraph.Inlines.Add(MyLink2);
            // Add the paragraph to the RichTextBox.
            richTextBox1.Blocks.Add(myParagraph);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }
    }
}