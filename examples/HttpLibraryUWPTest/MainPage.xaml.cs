﻿using HttpLibrary.Platform.UWP;
using HttpLibrary.Requesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HttpLibraryUWPTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static HttpLibraryPlatformUWP HttpLibPlatform = new HttpLibraryPlatformUWP();

        RequestQueue reqQueue;
        public MainPage()
        {
            this.InitializeComponent();

            reqQueue = new RequestQueue(HttpLibPlatform);

        }

        private async void OnClickQuery(object sender, RoutedEventArgs e)
        {
            var url = textBoxUri.Text;

            var req = new HtmlRequest(url);
            await reqQueue.SendRequestAsync(req);

            var response = req.Response as HtmlResponse;
            if (response.IsSucceeded)
            {
                webTextBlock.Text = response.Html;
            }
            else
            {
                webTextBlock.Text = response.Exception.ToString();
            }
        }
    }
}
