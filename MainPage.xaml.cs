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
using System.IO.IsolatedStorage;
using System.IO;

namespace EpubProject
{
    public partial class MainPage : PhoneApplicationPage
    {
        WebClient _webClient; // Used for downloading mp3
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _webClient = new WebClient();
            _webClient.OpenReadCompleted += (s1, e1) =>
            {

                if (e1.Error == null)
                {

                    try
                    {

                        string fileName = "epub_example.epub";

                        bool isSpaceAvailable = IsSpaceIsAvailable(e1.Result.Length);



                        if (isSpaceAvailable)
                        {

                            // Save mp3 to Isolated Storage

                            using (var isfs = new IsolatedStorageFileStream(fileName,

                                                FileMode.CreateNew,

                                                IsolatedStorageFile.GetUserStoreForApplication()))
                            {

                                long fileLen = e1.Result.Length;

                                byte[] b = new byte[fileLen];

                                e1.Result.Read(b, 0, b.Length);

                                isfs.Write(b, 0, b.Length);

                                isfs.Flush();

                            }
                        }

                        else
                        {

                            MessageBox.Show("Not enough to save space available to download mp3.");

                        }



                    }

                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);

                    }

                }

                else
                {

                    MessageBox.Show(e1.Error.Message);

                }

            };

            _webClient.OpenReadAsync(new Uri("http://nakamnakam.com/dev/epub_example.epub"));
            _webClient.OpenReadCompleted += _webClient_OpenReadCompleted;



            IsolatedStorageFileStream isfs1;
            using (IsolatedStorageFile isf1 = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    isfs1 = isf1.OpenFile("epub_example.epub", FileMode.Open);
                }
                catch
                {
                    return;
                }
            }

            EPubViewer.Source = isfs1;
            //EPubViewer.State = EPubReader.State.Toc;
            //EPubViewer.State = EPubReader.State.Cover;
            //EPubViewer.State = EPubReader.State.Normal;
        }

        void _webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            MessageBox.Show("Complete");
        }


        private bool IsSpaceIsAvailable(long spaceReq)
        {

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {

                long spaceAvail = store.AvailableFreeSpace;

                if (spaceReq > spaceAvail)
                {

                    return false;

                }

                return true;

            }

        }



        //EPubViewer.Source = Application.GetResourceStream(new Uri("epub_example.epub", UriKind.Relative)).Stream;


        private void EPubViewer_Tap(object sender, GestureEventArgs e)
        {
            if (EPubViewer.State != EPubReader.State.Normal)
                return;

            if (e.GetPosition(EPubViewer).X <= 160 && EPubViewer.CurrentLocation > 1)
                EPubViewer.CurrentLocation--;
            else if (e.GetPosition(EPubViewer).X >= 320 && EPubViewer.CurrentLocation < EPubViewer.FurthestLocation - 1)
                EPubViewer.CurrentLocation++;
        }

    }
}
