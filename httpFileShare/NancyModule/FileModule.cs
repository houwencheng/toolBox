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
            Get("/{name}", x =>
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

                var uri = (string)env["owin.RequestScheme"] + "://" + requestHeaders["Host"].First() +
                  (string)env["owin.RequestPathBase"] + (string)env["owin.RequestPath"];

                if (env["owin.RequestQueryString"] != "")
                    uri += "?" + (string)env["owin.RequestQueryString"];

                var fileName = x["name"];
                var responseBody = (Stream)env["owin.ResponseBody"];
                var responseHeaders = (IDictionary<string, string[]>)env["owin.ResponseHeaders"];
                responseHeaders.Add("Content-Disposition", new string[] { "attachment; filename=" + fileName });
                System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(ShareFloder);
                var floder = System.IO.Path.GetDirectoryName(ShareFloder);
                var fullFileName = System.IO.Path.Combine(floder, fileName);

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

                //Nancy.Responses.GenericFileResponse genericFileResponse = new Nancy.Responses.GenericFileResponse(fullFileName, this.Context);
                //return string.Format("{0} {1} fileName:{2}", requestMethod, uri, fileName);
                return "";
            });
        }
    }
}
