using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;

namespace Draft.Http
{
    public class HttpHelper
    {
        private static string FormatParameters(Dictionary<string, string> parameters)
        {
            string result = "";

            foreach (KeyValuePair<string, string> parameter in parameters)
                result += String.Format("{0}={1}&", parameter.Key, System.Web.HttpUtility.UrlEncode(parameter.Value));

            if (result.Length > 1)
                result = result.Substring(0, result.Length - 1);

            return result;
        }
        public static string Get(string url, CookieContainer cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";
            request.CookieContainer = cookies;
            request.Method = "GET";
                       
            WebResponse response = request.GetResponse();

            var dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
        public static string Post(string url, Dictionary<string, string> parameters, CookieContainer cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";
            request.CookieContainer = cookies;
            request.Method = "POST";

            string postData;

            if (parameters != null)
                postData = FormatParameters(parameters);
            else
                postData = "";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            //request.Timeout = 10000;
            WebResponse response = request.GetResponse();

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();


            return responseFromServer;
        }
        public static Bitmap DownloadPicture(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            return new Bitmap(responseStream);
        }
    }
}
