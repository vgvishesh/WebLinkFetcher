using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CreateHtmlFakePages
{
	// This class has the code to download all the images from a web page and store it on the local drive
    internal class ImageDownloader
    {
        private string _webAddress;
        string address = "address";

        public void DownloadImages(string htmlData, string url)
        {
            _webAddress = url;
            var pattern = @"<img\s+src\s*=\s*""(?<" + address + @">[^>]+)""";
            var regex = new Regex(pattern);
            var imageAddresses = regex.Matches(htmlData);
            var webClient = new WebClient();

            foreach (Match image in imageAddresses)
            {
                try
                {
                    SafeImagesToDisc(webClient, image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            webClient.Dispose();
        }

        private void SafeImagesToDisc(WebClient webClient, Match image)
        {
            var imageAddress = image.Groups[address].ToString();
            var imageServerAddress = _webAddress + imageAddress;
            var imageName = Path.GetFileName(imageAddress);

            var exeLocation = new Uri(System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase);
            var exeDirectory = new FileInfo(exeLocation.AbsolutePath).Directory;


            var imageDirectory = Path.Combine(exeDirectory.FullName, Path.GetDirectoryName(imageAddress.TrimStart('/')));

            if (!Directory.Exists(imageDirectory))
                Directory.CreateDirectory(imageDirectory);

            webClient.DownloadFile(imageServerAddress, Path.Combine(imageDirectory, imageName));
        }
    }
}