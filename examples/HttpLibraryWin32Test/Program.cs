using HttpLibrary.Platform.Win32;
using HttpLibrary.Requesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLibraryWin32Test
{
    class Program
    {
        static RequestQueue reqQueue;
        static void Main(string[] args)
        {
            reqQueue = new RequestQueue(new HttpLibraryPlatformWin32());

            var task = GetWebResource(args[0]);

            task.Wait();

            Console.WriteLine(task.Result);
        }

        private static async Task<string> GetWebResource(string uri)
        {
            var req = new HtmlRequest(uri);

            await reqQueue.SendRequestAsync(req);

            string content;
            var response = req.Response as HtmlResponse;
            if (response.IsSucceeded)
            {
                content = response.Html;
            }
            else
            {
                content = response.Exception.ToString();
            }

            return content;
            
            
        } 
    }
}
