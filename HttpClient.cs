using System;
using System.IO;
using System.Net;
using System.Text;

namespace CreateHtmlFakePages
{
    public class HttpClient
    {
        private HttpWebRequest _httpRequest;
        private HttpWebResponse _httpResponce;
        private string _webAddress;

        public string FetchHtmlData(string url)
        {
            _webAddress = url;

            try
            {
                OpenConnection();

                if (_httpResponce.StatusCode != HttpStatusCode.OK)
                {
                    return string.Empty;
                }

                Stream receiveStream = _httpResponce.GetResponseStream();
                StreamReader readStream = null;

                if (_httpResponce.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(_httpResponce.CharacterSet));
                }

                var data = readStream.ReadToEnd();
                readStream.Close();

                CloseConnection();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private void OpenConnection()
        {
            _httpRequest = WebRequest.Create(_webAddress) as HttpWebRequest;
            _httpResponce = _httpRequest.GetResponse() as HttpWebResponse;            
        }

        private void CloseConnection()
        {
            _httpResponce.Close();
        }
    }
}
