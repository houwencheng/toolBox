using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 用来调用WebAPI
    /// </summary>
    public static class WebAPIHelper
    {
        private static HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
        }

        public static string Get(string url, string token = null)
        {
            var handler = GetHttpClientHandler();
            using (var http = new HttpClient(handler))
            {
                if (token != null)
                {
                    http.DefaultRequestHeaders.Add("access-token", token);
                    http.DefaultRequestHeaders.Add("Authorization", token);
                }

                var request = http.GetAsync(url);
                var response = request.Result;

                var responseString = response.Content.ReadAsStringAsync().Result;
                return responseString;
            }
        }

        public static string Get(string url, dynamic obj, string token = null)
        {
            var handler = GetHttpClientHandler();
            using (var http = new HttpClient(handler))
            {
                if (token != null)
                {
                    http.DefaultRequestHeaders.Add("access-token", token);
                    http.DefaultRequestHeaders.Add("Authorization", token);
                }

                var request = http.GetAsync(url);

                var response = request.Result;

                var responseString = response.Content.ReadAsStringAsync().Result;
                return responseString;
            }
        }


        public static string Post(string url, dynamic obj, string token = null)
        {
            var handler = GetHttpClientHandler();
            using (var http = new HttpClient(handler))
            {
                if (token != null)
                {
                    http.DefaultRequestHeaders.Add("access-token", token);
                    http.DefaultRequestHeaders.Add("Authorization", token);
                }

                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = http.PostAsync(url, content);
                var response = request.Result;

                var responseString = response.Content.ReadAsStringAsync().Result;
                return responseString;
            }
        }

        public static string PostForm<T>(string url, T value, string token = null)
        {
            var modelType = typeof(T);
            var handler = GetHttpClientHandler();
            using (var http = new HttpClient(handler))
            using (var formData = new MultipartFormDataContent())
            {
                foreach (var item in modelType.GetProperties())
                {
                    var obj = item.GetValue(value);
                    if (obj != null)
                    {
                        var objString = obj.ToString();
                        HttpContent content = new StringContent(objString, Encoding.UTF8, "application/json");
                        formData.Add(content, item.Name);
                    }
                }

                if (token != null)
                {
                    http.DefaultRequestHeaders.Add("access-token", token);
                    http.DefaultRequestHeaders.Add("Authorization", token);
                }
                var request = http.PostAsync(url, formData);
                var response = request.Result;

                var responseString = response.Content.ReadAsStringAsync().Result;
                return responseString;
            }
        }

        public static string PostFile(string url, string fileFullName, string token = null)
        {
            var handler = GetHttpClientHandler();
            using (var fileStream = File.OpenRead(fileFullName))
            {
                using (var http = new HttpClient(handler))
                {
                    if (token != null)
                    {
                        http.DefaultRequestHeaders.Add("access-token", token);
                        http.DefaultRequestHeaders.Add("Authorization", token);
                    }

                    using (var content = new MultipartFormDataContent())
                    {
                        var fileContent = new StreamContent(fileStream);
                        var fileName = Path.GetFileName(fileFullName);
                        fileContent.Headers.ContentDisposition
                               = new ContentDispositionHeaderValue("form-data")
                               {
                                   FileName = fileName,
                                   Name = "file"
                               };
                        //= new ContentDispositionHeaderValue("attachment")
                        //{
                        //    FileName = fileName,
                        //    Name = "file"
                        //};
                        content.Add(fileContent);
                        var request = http.PostAsync(url, content);
                        var response = request.Result;

                        var responseString = response.Content.ReadAsStringAsync().Result;
                        return responseString;
                    }
                }
            }
        }
    }
}
