using Nancy.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace httpFileShare.NancyModule
{
    public class FileModule : Nancy.NancyModule
    {
        public static string ShareFloder { get; set; }

        public FileModule() : base("/file")
        {
            Get("/", x =>
            {
                var env = this.Context.GetOwinEnvironment();

                var requestBody = (Stream)env["owin.RequestBody"];
                var requestHeaders = (IDictionary<string, string[]>)env["owin.RequestHeaders"];
                var requestMethod = (string)env["owin.RequestMethod"];
                var requestPath = (string)env["owin.RequestPath"];
                var requestPathBase = (string)env["owin.RequestPathBase"];
                var requestProtocol = (string)env["owin.RequestProtocol"];
                var requestQueryString = (string)env["owin.RequestQueryString"];
                var requestScheme = (string)env["owin.RequestScheme"];
                var owinVersion = (string)env["owin.Version"];
                var cancellationToken = (System.Threading.CancellationToken)env["owin.CallCancelled"];

                var uri = requestScheme + "://" + requestHeaders["Host"].First() +
                  requestPathBase + requestPath;

                if (requestQueryString != "")
                    uri += "?" + requestQueryString;
                Console.WriteLine(uri);

                var fileName = Uri.UnescapeDataString(requestQueryString);
                var responseBody = (Stream)env["owin.ResponseBody"];
                var responseHeaders = (IDictionary<string, string[]>)env["owin.ResponseHeaders"];
                var fullFileName = System.IO.Path.Combine(ShareFloder, fileName);

                if (File.Exists(fullFileName))
                {
                    responseHeaders.Add("Content-Disposition", new string[] { "attachment; filename=" + fileName });
                    responseHeaders.Add("Content-Type", new string[] { "application/octet-stream" });
                    using (var fs = System.IO.File.OpenRead(fullFileName))
                    {
                        var fileLength = fs.Length;
                        var bufferLength = 1024 * 1024 * 10;
                        var buffer = new byte[bufferLength];

                        while (true)
                        {
                            var readCount = fs.Read(buffer, 0, buffer.Length);
                            if (readCount == 0) break;
                            responseBody.Write(buffer, 0, readCount);
                        }
                    }
                }
                else
                {
                    var notExist = @"
<html>
<head>
<meta charset='utf-8'/>
</head>
<body>
notExist 中文
</body>
</html>";

                    var buffer = UTF8Encoding.UTF8.GetBytes(notExist);
                    responseBody.Write(buffer, 0, buffer.Length);
                }

                return "";
            });
        }
    }
}
