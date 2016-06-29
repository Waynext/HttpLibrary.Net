using HttpLibrary.Interop;
using HttpLibrary.Requesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HttpLibraryXamarinTest
{
    public partial class WebPage : ContentPage
    {
        public static IHttpLibraryPlatform HttpLibraryPlatform;

        RequestQueue queue;
        public WebPage()
        {
            InitializeComponent();

            queryButton.Clicked += OnClickQuery;

            HttpLibraryPlatform = DependencyService.Get<IHttpLibrarySettings>().HttpLibraryPlaform;
            queue = new RequestQueue(HttpLibraryPlatform);

        }

        private async void OnClickQuery(object sender, EventArgs e)
        {
            var url = textBoxUri.Text;

            var req = new HtmlRequest(url);
            await queue.SendRequestAsync(req);

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
